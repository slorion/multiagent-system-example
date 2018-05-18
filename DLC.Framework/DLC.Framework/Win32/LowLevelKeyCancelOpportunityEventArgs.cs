using System;

namespace DLC.Framework.Win32
{
	/// <summary>
	/// Argument d'une méthode LowLevelKeyCancelOpportunityEventHandler
	/// </summary>
	public class LowLevelKeyCancelOpportunityEventArgs : LowLevelKeyEventArgs
	{
		/// <summary>
		/// Constructeur
		/// </summary>
		public LowLevelKeyCancelOpportunityEventArgs(bool cancel) { Cancel = cancel; }

		/// <summary>
		/// Constructeur
		/// </summary>
		public LowLevelKeyCancelOpportunityEventArgs(bool cancel, int key, bool shift, bool ctrl, bool alt, bool win)
		{
			Cancel = cancel;
			KeyCode = key;
			KeyShift = shift;
			KeyCtrl = ctrl;
			KeyAlt = alt;
			KeyWin = win;
		}

		/// <summary>
		/// Annule le traitement de la chaîne d'appels
		/// </summary>
		public bool Cancel { get; set; }
	}
}