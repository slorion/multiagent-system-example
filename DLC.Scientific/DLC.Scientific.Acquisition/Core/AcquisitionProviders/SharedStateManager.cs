using System;
using System.Collections.Generic;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public static class SharedStateManager<TState>
		where TState : IDisposable
	{
		private static readonly Dictionary<Type, Tuple<IDisposable, int>> _stateRefCounts = new Dictionary<Type, Tuple<IDisposable, int>>();

		public static TState RegisterProvider(Func<TState> createState)
		{
			if (createState == null) throw new ArgumentNullException("createState");

			lock (_stateRefCounts)
			{
				Tuple<IDisposable, int> stateRefCount;
				if (!_stateRefCounts.TryGetValue(typeof(TState), out stateRefCount))
					stateRefCount = Tuple.Create((IDisposable) createState(), 1);
				else
					stateRefCount = Tuple.Create(stateRefCount.Item1, stateRefCount.Item2 + 1);
				_stateRefCounts[typeof(TState)] = stateRefCount;

				return (TState) stateRefCount.Item1;
			}
		}

		public static void UnregisterProvider()
		{
			lock (_stateRefCounts)
			{
				Tuple<IDisposable, int> stateRefCount;
				if (!_stateRefCounts.TryGetValue(typeof(TState), out stateRefCount))
					throw new InvalidOperationException(string.Format("No state registered for type '{0}'.", typeof(TState)));
				else
				{
					stateRefCount = Tuple.Create(stateRefCount.Item1, stateRefCount.Item2 - 1);
					_stateRefCounts[typeof(TState)] = stateRefCount;

					if (stateRefCount.Item2 == 0)
						stateRefCount.Item1.Dispose();
				}
			}
		}
	}
}