using System;
using System.Runtime.InteropServices;

namespace DLC.Framework.IO.Interop
{
	internal static class NtfsHelper
	{
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

		public static bool CreateHardLink(string existingFilePath, string newFilePath)
		{
			if (string.IsNullOrEmpty(existingFilePath)) throw new ArgumentNullException("existingFilePath");
			if (string.IsNullOrEmpty(newFilePath)) throw new ArgumentNullException("newFilePath");

			return CreateHardLink(newFilePath, existingFilePath, IntPtr.Zero);
		}
	}
}