using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;

namespace QbservableProvider
{
	internal static class QbservableProviderDiagnostics
	{
		private static PropertyInfo debugView;

		[Conditional("DEBUG")]
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static void DebugPrint(Expression expression, string category)
		{
			if (debugView == null)
			{
				debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance);
			}

			var value = debugView.GetValue(expression);

			Debug.WriteLine(Environment.NewLine + value, category);
		}
	}
}