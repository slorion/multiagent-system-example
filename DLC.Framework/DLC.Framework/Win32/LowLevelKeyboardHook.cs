using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DLC.Framework.Win32
{
	/// <summary>
	/// Classe permettant de recevoir les événements du clavier sans que l'application ait besoin d'avoir le focus.
	/// </summary>
	public class LowLevelKeyboardHook
	{
		/// <summary>
		/// Événment déclenché quand l'utilisateur enfonce une touche du clavier
		/// </summary>
		public event EventHandler<LowLevelKeyEventArgs> KeyDown;

		/// <summary>
		/// Déclenche l'événement KeyDown
		/// </summary>
		/// <param name="key"></param>
		protected void OnKeyDown(int key)
		{
			if (KeyDown != null)
			{
				LowLevelKeyEventArgs args = new LowLevelKeyEventArgs(key, _shiftState, _ctrlState, _altState, _winState);
				KeyDown.Invoke(this, args);
			}
		}

		private void OnKeyDownCallback(object e)
		{
			LowLevelKeyEventArgs args = (LowLevelKeyEventArgs) ((IAsyncResult) e).AsyncState;
		}

		/// <summary>
		/// Événement permettant de stopper la chaîne d'appels de la touche appuyée. Ceci permet de "voler" 
		/// le contrôle de la touche à une autre application.
		/// </summary>
		public event EventHandler<LowLevelKeyCancelOpportunityEventArgs> KeyDownCancelOpportunity;

		/// <summary>
		/// Déclenche l'énévement KeyDownCancelOpportinity
		/// </summary>
		/// <param name="key">Le Virtual Key Code de la touche enfoncée</param>
		/// <returns>Vrai s'il faut stopper la chaîne d'appels, faux si on laisse continuer</returns>
		protected bool OnKeyDownCancelOpportunity(int key)
		{
			LowLevelKeyCancelOpportunityEventArgs args = new LowLevelKeyCancelOpportunityEventArgs(false, key, _shiftState, _ctrlState, _altState, _winState);

			if (this.KeyDownCancelOpportunity != null)
				KeyDownCancelOpportunity(this, args);

			return args.Cancel;
		}

		private SafeWinHookHandle _handle;
		private IntPtr _ptr;
		private DLC.Framework.Win32.Win32Native.KBDLLHookProc _hookCallback;
		private bool _shiftState;
		private bool _ctrlState;
		private bool _altState;
		private bool _winState;

		/// <summary>
		/// Démmarre l'écoute des événements du clavier
		/// </summary>
		public void Hook()
		{
			if ((int) _ptr != 0) return;
			_hookCallback = HookCallback;
			using (Process curProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule curModule = curProcess.MainModule)
				{
					_ptr = Win32Native.SetWindowsHookEx(
						 Win32Native.HookType.WH_KEYBOARD_LL,
						 _hookCallback,
						 Win32Native.GetModuleHandle(curModule.ModuleName), 0);

					_handle = new SafeWinHookHandle(_ptr, true);

				}
			}
		}

		/// <summary>
		/// Stoppe l'écoute des événements du clavier
		/// </summary>
		public void UnHook()
		{
			Win32Native.UnhookWindowsHookEx(_ptr);
			_ptr = (IntPtr) 0;
		}

		private IntPtr HookCallback(int nCode, Win32Native.WindowsMessage wParam, Win32Native.KBDLLHOOKSTRUCT lParam)
		{
			bool cancel = false;

			if (nCode >= 0)
			{
				if (wParam == Win32Native.WindowsMessage.WM_KEYDOWN)
					cancel = HandleKeyDown(lParam.vkCode);

				if (wParam == Win32Native.WindowsMessage.WM_SYSKEYDOWN)
					cancel = HandleKeyDown(lParam.vkCode);

				if (wParam == Win32Native.WindowsMessage.WM_KEYUP)
					HandleKeyUp(lParam.vkCode);

				if (wParam == Win32Native.WindowsMessage.WM_SYSKEYUP)
					HandleKeyUp(lParam.vkCode);
			}

			return cancel ? (IntPtr) (-1) : Win32Native.CallNextHookEx(_ptr, nCode, wParam, lParam);
		}

		private bool HandleKeyDown(int param)
		{
			Keys key = (Keys) param;
			switch (key)
			{
				case Keys.LShiftKey:
				case Keys.RShiftKey:
				case Keys.Shift:
				case Keys.ShiftKey:
					_shiftState = true;
					break;

				case Keys.LControlKey:
				case Keys.RControlKey:
				case Keys.Control:
				case Keys.ControlKey:
					_ctrlState = true;
					break;

				case Keys.Alt:
				case Keys.LMenu:
				case Keys.RMenu:
					_altState = true;
					break;

				case Keys.LWin:
				case Keys.RWin:
					_winState = true;
					break;
			}

			OnKeyDown((int) key);
			return OnKeyDownCancelOpportunity((int) key);
		}

		private void HandleKeyUp(int param)
		{
			Keys vkCode = (Keys) param;

			switch (vkCode)
			{
				case Keys.LShiftKey:
				case Keys.RShiftKey:
				case Keys.Shift:
				case Keys.ShiftKey:
					_shiftState = false;
					break;

				case Keys.LControlKey:
				case Keys.RControlKey:
				case Keys.Control:
				case Keys.ControlKey:
					_ctrlState = false;
					break;

				case Keys.Alt:
					_altState = false;
					break;

				case Keys.LWin:
				case Keys.RWin:
					_winState = false;
					break;
			}
		}
	}
}
