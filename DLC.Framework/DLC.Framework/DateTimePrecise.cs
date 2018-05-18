using System;
using System.Runtime.InteropServices;

namespace DLC.Framework
{
	/// <summary>
	/// Return the most precise DateTime available on the current platform.
	/// For a precision of 1ms or less on Windows, the CPU architecture should be x64 and Windows version 8 or later.
	/// </summary>
	public static class DateTimePrecise
	{
		// https://msdn.microsoft.com/en-us/library/windows/desktop/hh706895%28v=vs.85%29.aspx
		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
		private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

		private static readonly bool _isKernelTimeAvailable;

		static DateTimePrecise()
		{
			// detect if kernel function is available (Windows 8/2012 or later)

			try
			{
				long filetime;
				GetSystemTimePreciseAsFileTime(out filetime);
				_isKernelTimeAvailable = true;
			}
			catch (EntryPointNotFoundException)
			{
				_isKernelTimeAvailable = false;
			}
		}

		public static DateTime Now
		{
			get
			{
				if (_isKernelTimeAvailable)
				{
					long filetime;
					GetSystemTimePreciseAsFileTime(out filetime);
					return DateTime.FromFileTimeUtc(filetime);
				}
				else
				{
					return DateTime.Now;
				}
			}
		}
	}
}