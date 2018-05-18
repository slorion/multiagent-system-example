namespace DLC.Scientific.Core.Journalisation
{
	public class EventJournal<TEventJournalHeader,TEventJournalEntry, TEventJournalFooter>
		: Journal<TEventJournalHeader, TEventJournalEntry, TEventJournalFooter>,
		IEventJournal
		where TEventJournalHeader : EventJournalHeader, new()
		where TEventJournalEntry : EventJournalEntry, new()
		where TEventJournalFooter : EventJournalFooter, new()
	{
		public static readonly new string DefaultExtension = ".ejx";

		EventJournalHeader IEventJournal.EventJournalHeader
		{
			get { return this.JournalHeader; }
		}

		EventJournalFooter IEventJournal.EventJournalFooter
		{
			get { return this.JournalFooter; }
		}
	}

	public class EventJournal<THeader, TEntry>
		: EventJournal<THeader, TEntry, EventJournalFooter>
		where THeader : EventJournalHeader, new()
		where TEntry : EventJournalEntry, new()
	{

	}

	public class EventJournal<TEntry>
		: EventJournal<EventJournalHeader, TEntry, EventJournalFooter>
		where TEntry : EventJournalEntry, new()
	{

	}
}