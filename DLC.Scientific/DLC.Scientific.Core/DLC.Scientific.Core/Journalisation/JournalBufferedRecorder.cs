using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DLC.Scientific.Core.Journalisation
{
	public class JournalBufferedRecorder
		: XmlTextWriter
	{
		private readonly BlockingCollection<object> _buffer = new BlockingCollection<object>();
		private readonly IJournal _journal;
		private readonly Task _addEntryTask;

		public JournalBufferedRecorder(string fileName, IJournal journal, bool forceFlush = false)
			: base(fileName, Encoding.Default)
		{
			if (journal == null) throw new ArgumentNullException("journal");

			this.ForceFlush = forceFlush;
			this.Formatting = Formatting.Indented;

			_journal = journal;
			_journal.AddingNew += _journal_AddingNew;

			WriteStartDocument();

			WriteStartElement("EventJournal");
			WriteAttributeString("Type", _journal.GetType().AssemblyQualifiedName);
			WriteAttributeString("Version", Assembly.GetAssembly(_journal.GetType()).GetName().Version.ToString());

			AddItem("Header", _journal.EventJournalHeader);

			WriteStartElement("Entries");

			_addEntryTask = Task.Run(
				() => {
					foreach (var item in _buffer.GetConsumingEnumerable())
					{
						AddItem("Entry", item);
						if (this.ForceFlush)
							Flush();
					}
				});
		}

		public bool ForceFlush { get; set; }
		public bool IsClosed { get { return this.WriteState == WriteState.Closed || this.WriteState == WriteState.Error; } }
		public int BufferCount { get { return _buffer.Count; } }
		public bool IsEmpty { get { return this.BufferCount <= 0; } }

		public override void Close()
		{
			_journal.AddingNew -= _journal_AddingNew;
			_buffer.CompleteAdding();

			_addEntryTask.Wait(TimeSpan.FromSeconds(30));

			WriteEndElement(); // Entries
			AddItem("Footer", _journal.EventJournalFooter);
			WriteEndElement(); // EventJournal
			Flush();

			base.Close();
		}

		private void _journal_AddingNew(object sender, AddingNewEventArgs e)
		{
			_buffer.Add(e.NewObject);
		}

		private void AddItem(string section, object item)
		{
			var serializer = XmlSerializerCache.GetOrAdd(item.GetType(), section);
			serializer.Serialize(this, item, XmlSerializerCache.BlankNamespace);
		}
	}
}