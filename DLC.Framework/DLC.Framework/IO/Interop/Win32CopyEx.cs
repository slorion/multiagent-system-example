using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Framework.IO.Interop
{
	/// <summary>
	/// PInvoke wrapper for CopyEx
	/// http://msdn.microsoft.com/en-us/library/windows/desktop/aa363852.aspx
	/// </summary>
	internal static class Win32CopyEx
	{
		public static Task Copy(string source, string destination, OverwriteMode overwriteMode, CopyOptions options, CancellationToken? cancellationToken = null, Action<long, long> progressCallback = null)
		{
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
			if (string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");

			var copyFileFlags = CopyFileFlags.COPY_FILE_RESTARTABLE;

			if (overwriteMode != OverwriteMode.AlwaysOverwrite)
				copyFileFlags |= CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

			if (options.HasFlag(CopyOptions.DisableBuffering))
				copyFileFlags |= CopyFileFlags.COPY_FILE_NO_BUFFERING;

			int isCancelled = 0;
			var ct = cancellationToken ?? CancellationToken.None;

			CopyProgressRoutine progressRoutine =
				(total, transferred, streamSize, streamByteTrans, dwStreamNumber, reason, hSourceFile, hDestinationFile, lpData) =>
				{
					if (progressCallback != null && reason == CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED)
						progressCallback(transferred, total);

					if (ct.IsCancellationRequested)
						return CopyProgressResult.PROGRESS_CANCEL;
					else
						return CopyProgressResult.PROGRESS_CONTINUE;
				};

			return Task.Run(
				() =>
				{
					if (!CopyFileEx(source, destination, progressRoutine, IntPtr.Zero, ref isCancelled, copyFileFlags))
					{
						int errorCode = Marshal.GetLastWin32Error();

						// https://msdn.microsoft.com/en-us/library/cc231199.aspx
						// ERROR_FILE_EXISTS = 0x00000050;
						// ERROR_ALREADY_EXISTS = 0x000000B7
						// ERROR_OBJECT_ALREADY_EXISTS = 0x00001392
						// ERROR_OBJECT_NAME_EXISTS = 0x000002BA
						if (errorCode == 0x00000050 || errorCode == 0x000000B7 || errorCode == 0x00001392 || errorCode == 0x000002BA)
						{
							if (overwriteMode == OverwriteMode.OverwriteIfDifferent)
							{
								if (IOHelper.AreSameFile(source, destination))
								{
									return;
								}
								else
								{
									copyFileFlags &= ~CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

									if (CopyFileEx(source, destination, progressRoutine, IntPtr.Zero, ref isCancelled, copyFileFlags))
										return;
								}
							}
						}

						throw new Win32Exception(errorCode);
					}
				});
		}

		#region PInvoke

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, CopyFileFlags dwCopyFlags);

		private delegate CopyProgressResult CopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

		private enum CopyProgressResult
		{
			PROGRESS_CONTINUE = 0,
			PROGRESS_CANCEL = 1,
			PROGRESS_STOP = 2,
			PROGRESS_QUIET = 3
		}

		private enum CopyProgressCallbackReason
		{
			CALLBACK_CHUNK_FINISHED = 0x00000000,
			CALLBACK_STREAM_SWITCH = 0x00000001
		}

		[Flags]
		private enum CopyFileFlags
		{
			COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
			COPY_FILE_NO_BUFFERING = 0x00001000,
			COPY_FILE_RESTARTABLE = 0x00000002,
			COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
			COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
		}

		#endregion
	}
}