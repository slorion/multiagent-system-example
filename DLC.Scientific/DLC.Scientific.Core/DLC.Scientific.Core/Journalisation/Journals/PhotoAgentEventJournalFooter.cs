using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class PhotoAgentEventJournalFooter
		: EventJournalFooter
	{
		public string Name { get; set; }
		public int NbImagesCapturees { get; set; }
		public double Fps { get; set; }

		/// <summary>
		/// The number of frames captured since the start of imaging.
		/// </summary>
		public int NbFrameDelivered { get; set; }

		/// <summary>
		/// Number of frames dropped by the streaming engine due to missing packets.
		/// </summary>
		public int NbFrameDropped { get; set; }

		/// <summary>
		/// Number of frames successfully delivered by the streaming engine after having had missing packets.
		/// </summary>
		public int NbFrameRescued { get; set; }

		/// <summary>
		/// Number of frames dropped because a following frame was completed before.
		/// </summary>
		public int NbFrameShoved { get; set; }

		/// <summary>
		/// Number of frames missed due to the non-availability of a user supplied buffer.
		/// </summary>
		public int NbFrameUnderrun { get; set; }

		/// <summary>
		/// The number of improperly formed packets. If this number is non-zero, it suggests a possible cable or camera hardware failure.
		/// </summary>
		public int NbPacketErrors { get; set; }

		/// <summary>
		/// The number of packets missed since the start of imaging.
		/// <remarks>If everything is configured correctly, this number should remain zero, or at least very low compared to StatPacketReceived.</remarks>
		/// </summary>
		public int NbPacketMissed { get; set; }

		/// <summary>
		/// When an expected packet is not received by the driver, it is recognized as missing and the driver requests the camera to resend it. 
		/// The resend mechanism ensures very high data integrity.
		/// <remarks>If everything is configured correctly, this number should remain zero, or at least very low compared to StatPacketReceived.</remarks>
		/// </summary>
		public int NbPacketRequested { get; set; }

		/// <summary>
		/// Indicates the number of packets received by the driver since the start of imaging, this number should grow steadily during continuous acquisition.
		/// </summary>
		public long NbPacketReceived { get; set; }
	}
}