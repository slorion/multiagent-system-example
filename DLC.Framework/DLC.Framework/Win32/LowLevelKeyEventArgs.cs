using System;

namespace DLC.Framework.Win32
{
	/// <summary>
	/// Argument d'une méthode LowLevelKeyEventHandler
	/// </summary>
	public class LowLevelKeyEventArgs
		: EventArgs
	{
		/// <summary>
		/// Constructeur
		/// </summary>
		public LowLevelKeyEventArgs() { }

		/// <summary>
		/// Constructeur
		/// </summary>
		public LowLevelKeyEventArgs(int keyCode, bool shift, bool ctrl, bool alt, bool win) { KeyCode = keyCode; KeyShift = shift; KeyCtrl = ctrl; KeyAlt = alt; KeyWin = win; }

		/// <summary>
		/// Virtual Key Code
		/// </summary>
		public int KeyCode { get; set; }

		/// <summary>
		/// Indique si la touche "Shift" est enfoncée
		/// </summary>
		public bool KeyShift { get; set; }

		/// <summary>
		/// Indique si la touche "Control" est enfoncée
		/// </summary>
		public bool KeyCtrl { get; set; }

		/// <summary>
		/// Indique si la touche "Alt" est enfoncée
		/// </summary>
		public bool KeyAlt { get; set; }

		/// <summary>
		/// Indique si la touche "Windows" est enfoncée
		/// </summary>
		public bool KeyWin { get; set; }
	}
}