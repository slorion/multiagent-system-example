using Microsoft.Win32.SafeHandles;
using System;
using System.Security.Permissions;

namespace DLC.Framework.Win32
{
	/// <summary>
	/// A <see cref="SafeHandle"/> implementation for a <c>Windows Hook Handle</c>.
	/// </summary>
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class SafeWinHookHandle
		: SafeHandleMinusOneIsInvalid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SafeWinHookHandle"/> class.
		/// </summary>
		private SafeWinHookHandle() : base(true) { }

		public SafeWinHookHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		/// <summary>
		/// Executes the code required to free the handle.
		/// </summary>
		/// <returns>
		/// true if the handle is released successfully; otherwise, false.
		/// </returns>
		protected override bool ReleaseHandle()
		{
			return Win32Native.UnhookWindowsHookEx(handle);
		}
	}
}
