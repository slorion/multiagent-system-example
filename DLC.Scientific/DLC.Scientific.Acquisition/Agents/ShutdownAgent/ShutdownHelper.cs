using System;
using System.Runtime.InteropServices;

namespace DLC.Scientific.Acquisition.Agents.ShutdownAgent
{
	internal static class ShutdownHelper
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct TokPriv1Luid
		{
			public int Count;
			public long Luid;
			public int Attr;
		}

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool ExitWindowsEx(int flg, int rea);

		private const int SE_PRIVILEGE_ENABLED = 0x00000002;
		private const int TOKEN_QUERY = 0x00000008;
		private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
		private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
		private const int EWX_LOGOFF = 0x00000000;
		private const int EWX_SHUTDOWN = 0x00000001;
		private const int EWX_REBOOT = 0x00000002;
		private const int EWX_FORCE = 0x00000004;
		private const int EWX_POWEROFF = 0x00000008;
		private const int EWX_FORCEIFHUNG = 0x00000010;
		private const int EWX_FORCEREBOOT = EWX_REBOOT | EWX_FORCE;
		private const int EWX_FORCEIFHUNGREBOOT = EWX_REBOOT | EWX_FORCEIFHUNG;
		private const int EWX_FORCESHUTDOWN = EWX_SHUTDOWN | EWX_FORCE;
		private const int EWX_FORCEIFHUNGSHUTDOWN = EWX_SHUTDOWN | EWX_FORCEIFHUNG;
		private const int EWX_FORCEPOWEROFF = EWX_POWEROFF | EWX_FORCE;
		private const int EWX_FORCEIFHUNGPOWEROFF = EWX_POWEROFF | EWX_FORCEIFHUNG;
		private const int EWX_FORCELOGOFF = EWX_LOGOFF | EWX_FORCE;
		private const int EWX_FORCEIFHUNGLOGOFF = EWX_LOGOFF | EWX_FORCEIFHUNG;

		private static void ExitSystem(int actionFlag)
		{

			IntPtr hproc = GetCurrentProcess();
			IntPtr htok = IntPtr.Zero;

			bool bOK = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);

			TokPriv1Luid tp = new TokPriv1Luid();
			tp.Count = 1;
			tp.Luid = 0;
			tp.Attr = SE_PRIVILEGE_ENABLED;

			bOK = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);

			bOK = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

			bOK = ExitWindowsEx(actionFlag, 0);
		}

		public static void SystemReboot()
		{
			ExitSystem(EWX_REBOOT);
		}

		public static void SystemForceReboot()
		{
			ExitSystem(EWX_FORCEREBOOT);
		}

		public static void SystemForceIfHungReboot()
		{
			ExitSystem(EWX_FORCEIFHUNGREBOOT);
		}

		public static void SystemShutdown()
		{
			ExitSystem(EWX_SHUTDOWN);
		}

		public static void SystemForceShutdown()
		{
			ExitSystem(EWX_FORCESHUTDOWN);
		}

		public static void SystemForceIfHungShutdown()
		{
			ExitSystem(EWX_FORCEIFHUNGSHUTDOWN);
		}

		public static void SystemLogOff()
		{
			ExitSystem(EWX_LOGOFF);
		}

		public static void SystemForceLogOff()
		{
			ExitSystem(EWX_FORCELOGOFF);
		}

		public static void SystemForceIfHungLogOff()
		{
			ExitSystem(EWX_FORCEIFHUNGLOGOFF);
		}

		public static void SystemForcePowerOff()
		{
			ExitSystem(EWX_FORCEPOWEROFF);
		}

		public static void SystemForceIfHungPowerOff()
		{
			ExitSystem(EWX_FORCEIFHUNGPOWEROFF);
		}
	}
}