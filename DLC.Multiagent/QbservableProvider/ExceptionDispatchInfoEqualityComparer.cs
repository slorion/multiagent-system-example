using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace QbservableProvider
{
	internal sealed class ExceptionDispatchInfoEqualityComparer : IEqualityComparer<ExceptionDispatchInfo>
	{
		public static readonly ExceptionDispatchInfoEqualityComparer Instance = new ExceptionDispatchInfoEqualityComparer();

		private ExceptionDispatchInfoEqualityComparer()
		{
		}

		public bool Equals(ExceptionDispatchInfo x, ExceptionDispatchInfo y)
		{
			return x == null
					 ? y == null
					 : y != null && x.SourceException == y.SourceException;
		}

		public int GetHashCode(ExceptionDispatchInfo obj)
		{
			return obj == null ? 0 : obj.SourceException.GetHashCode();
		}
	}
}