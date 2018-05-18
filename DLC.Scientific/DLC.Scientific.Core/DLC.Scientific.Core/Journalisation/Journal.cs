using DLC.Framework;
using HashLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DLC.Scientific.Core.Journalisation
{
	[Serializable]
	public abstract class Journal<THeader, TEntry, TFooter>
		: IEnumerable<TEntry>, IJournal, IJournalInternal
		where THeader : JournalHeader, new()
		where TEntry : JournalEntry, new()
		where TFooter : JournalFooter, new()
	{
		public event AddingNewEventHandler AddingNew;

		public static readonly string DefaultExtension = ".ejx";

		private readonly object _lockAccess = new object();
		private volatile bool _isOpened;

		public Journal()
		{
			lock (_lockAccess)
			{
				JournalHeader = new THeader() {
					CreationDateTime = DateTimePrecise.Now,
					CreationSource = "",
					Id = ""
				};

				JournalEntryList = new List<TEntry>();

				JournalFooter = new TFooter() {
					CloseDateTime = DateTimePrecise.Now,
					NumberOfEntry = 0
				};

				_isOpened = true;
			}
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="creationSource"></param>
		/// <param name="id"></param>
		public Journal(string creationSource, string id)
			: this()
		{

			JournalHeader.CreationSource = creationSource;
			JournalHeader.Id = id;
		}

		public virtual string Extension
		{
			get
			{
				string ext = DefaultExtension;
				Func<Type, string> recur = null;

				recur = (type) =>
				{
					if (type == typeof(object)) return ext;

					FieldInfo fi = type.GetField("DefaultExtension");
					if (fi == null) return recur(type.BaseType);
					else return (string) fi.GetValue(null);
				};

				return recur(this.GetType());
			}
		}

		public static string GetChecksum(string file)
		{
			IHash crc64 = HashFactory.Checksum.CreateCRC64_ECMA();
			HashResult r = crc64.ComputeFile(file);
			return r.ToString();
		}

		protected virtual void OnAddingNew(AddingNewEventArgs e)
		{
			if (this.AddingNew != null)
			{
				this.AddingNew(this, e);
			}
		}

		Type IJournalInternal.JournalHeaderType
		{
			get
			{
				return typeof(THeader);
			}
		}

		Type IJournalInternal.JournalEntryType
		{
			get
			{
				return typeof(TEntry);
			}
		}

		Type IJournalInternal.JournalFooterType
		{
			get
			{
				return typeof(TFooter);
			}
		}

		public THeader JournalHeader { get; set; }
		private List<TEntry> JournalEntryList { get; set; }
		public TFooter JournalFooter { get; set; }

		JournalHeader IJournal.EventJournalHeader
		{
			get { return (JournalHeader) JournalHeader; }
		}

		JournalFooter IJournal.EventJournalFooter
		{
			get { return (JournalFooter) JournalFooter; }
		}

		JournalHeader IJournalInternal.JournalHeader
		{
			set { JournalHeader = (THeader) value; }
		}

		JournalFooter IJournalInternal.JournalFooter
		{
			set { JournalFooter = (TFooter) value; }
		}

		void IJournal.Add(JournalEntry entry)
		{
			this.Add((TEntry) entry);
		}

		/// <summary>
		/// Méthode qui ajoute une entrée au journal
		/// </summary>
		/// <param name="entry"></param>
		/// <exception cref="System.InvalidOperationException">en cas d'impossibilité d'ajouter une entrée</exception>
		public void Add(TEntry entry)
		{
			if (_isOpened)
			{
				lock (_lockAccess)
				{
					JournalEntryList.Add(entry);
					JournalFooter.NumberOfEntry++;
				}
				OnAddingNew(new AddingNewEventArgs(entry));
			}
			else
			{
				throw new InvalidOperationException("Impossible d'ajouter un élément, le journal est fermé!");
			}
		}

		public void AddRange(IEnumerable<JournalEntry> entries)
		{
			if (entries == null)
				return;

			foreach (var entry in entries)
				this.Add((TEntry) entry);
		}

		/// <summary>
		/// Méthode qui ajoute une entrée au journal
		/// </summary>
		/// <param name="comment"></param>
		public void Add(string comment)
		{
			Add(new TEntry() {
				Comment = comment,
				DateTime = DateTimePrecise.Now,
			});
		}

		public TEntry this[int index]
		{
			get
			{
				return JournalEntryList[index];
			}
			set
			{
				JournalEntryList[index] = value;
			}
		}

		public int Count
		{
			get { return JournalEntryList.Count; }
		}

		public IEnumerator<TEntry> GetEnumerator()
		{
			return JournalEntryList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Méthode qui termine le journal
		/// </summary>
		public void Close()
		{
			lock (_lockAccess)
			{
				_isOpened = false;
				JournalFooter.CloseDateTime = DateTimePrecise.Now;
			}
		}
	}

	public abstract class Journal<TEntry>
		: Journal<JournalHeader, TEntry, JournalFooter>
		where TEntry : JournalEntry, new()
	{
		public Journal() : base() { }

		public Journal(string creationSource, string id)
			: base(creationSource, id)
		{
		}
	}

	public abstract class Journal<THeader, TEntry>
		: Journal<THeader, TEntry, JournalFooter>
		where THeader : JournalHeader, new()
		where TEntry : JournalEntry, new()
	{
		public Journal() : base() { }

		public Journal(string creationSource, string id)
			: base(creationSource, id)
		{
		}
	}

	/// <summary>
	/// Generic journal for representing generic data from any Journal, can't be inherited
	/// </summary>
	public sealed class Journal
		: Journal<JournalHeader, JournalEntry, JournalFooter>
	{
	}
}