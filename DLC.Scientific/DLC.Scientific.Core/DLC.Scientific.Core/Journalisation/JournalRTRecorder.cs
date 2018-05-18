using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DLC.Scientific.Core.Journalisation
{
	/// <summary>
	/// Records an EventJournal to a stream in real lstTime by listening to the journal events
	/// </summary>
	public class JournalRTRecorder
		: XmlTextWriter
	{
		private readonly IJournal _journal;

		public JournalRTRecorder(Stream stream, IJournal journal, Encoding encoding)
			: this(stream, journal, encoding, false)
		{
		}

		public JournalRTRecorder(Stream stream, IJournal journal, Encoding encoding, bool forceFlush)
			: base(stream, encoding)
		{
			ForceFlush = forceFlush;

			_journal = journal;
			_journal.AddingNew += new AddingNewEventHandler(_journal_AddingNew);

			WriteStartDocument();
			Formatting = Formatting.Indented;

			WriteStartElement("EventJournal");
			WriteAttributeString("Type", _journal.GetType().AssemblyQualifiedName);
			WriteAttributeString("Version", Assembly.GetAssembly(_journal.GetType()).GetName().Version.ToString());

			ToXml(this, _journal.EventJournalHeader, "Header");

			WriteStartElement("Entries");
		}

		public bool ForceFlush { get; set; }

		public override void Close()
		{
			WriteEndElement(); // EventJournalEntryList

			_journal.Close();
			ToXml(this, _journal.EventJournalFooter, "Footer");

			WriteEndElement(); // EventJournalOfT
			base.Close();
		}

		private void _journal_AddingNew(object sender, AddingNewEventArgs e)
		{
			JournalRTRecorder.ToXml(this, e.NewObject, "Entry");
			if (ForceFlush) this.Flush();
		}

		internal static void ToXml(XmlWriter writer, object data)
		{
			ToXml(writer, data,null );
		}

		internal static void ToXml(XmlWriter writer, object data, string xmlName)
		{
			var serializer = XmlSerializerCache.GetOrAdd(data.GetType(), xmlName);
			serializer.Serialize(writer, data, XmlSerializerCache.BlankNamespace);
		}
	}
}