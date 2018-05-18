using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Framework.Reactive
{
	public static class ObservableExtensions
	{
		private static readonly object @true = new object();
		private static readonly object @false = null;

		public static IObservable<TOut> SkipIfProcessing<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> action, Action<TIn> skipAction = null)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (action == null) throw new ArgumentNullException("action");

			object isProcessing = @false;

			return source
				.SelectMany(
					async data =>
					{
						if (Interlocked.CompareExchange(ref isProcessing, @true, @false) == @true)
						{
							Log.Trace().Message("SKIP: '{0}'", data).Write();

							if (skipAction != null)
								skipAction(data);

							return Tuple.Create(true, default(TOut));
						}
						else
						{
							try
							{
								return Tuple.Create(false, await action(data).ConfigureAwait(false));
							}
							finally
							{
								Interlocked.CompareExchange(ref isProcessing, @false, @true);
							}
						}
					})
				.Where(a => !a.Item1)
				.Select(a => a.Item2);
		}

		public static IObservable<TResult> CancellableUsing<TResult, TResource>(Func<TResource> resourceFactory, Func<TResource, CancellationToken, IObservable<TResult>> observableFactory)
			where TResource : IDisposable
		{
			if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
			if (observableFactory == null) throw new ArgumentNullException("observableFactory");

			return Observable.Using(
				ct => Task.Run(resourceFactory, ct),
				(resource, ct) => Task.Run(() => observableFactory(resource, ct), ct));
		}

		public static IObservable<Tuple<TSource, TSource>> WithPrevious<TSource>(this IObservable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Scan(
				Tuple.Create(default(TSource), default(TSource)),
				(previous, current) => Tuple.Create(previous.Item2, current));
		}

		//TODO: should probably return IObservable<IGroupedObservable<TKey, T>>
		public static IObservable<Tuple<TKey, IEnumerable<T>>> GroupByUntilChanged<TKey, T>(this IObservable<T> source, Func<T, TKey> keySelector)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (keySelector == null) throw new ArgumentNullException("keySelector");

			var currentSet = new List<T>();
			TKey currentKey = default(TKey);

			return source
				.Select(
					data =>
					{
						if (currentSet.Count == 0 || object.Equals(keySelector(currentSet[currentSet.Count - 1]), keySelector(data)))
						{
							currentSet.Add(data);
							return null;
						}
						else
						{
							var set = currentSet;
							currentSet = new List<T>();

							return Tuple.Create(currentKey, (IEnumerable<T>) set);
						}
					})
				.Merge(source.LastOrDefaultAsync().Select(data => object.Equals(data, default(T)) ? null : Tuple.Create(keySelector(data), (IEnumerable<T>) currentSet)))
				.Where(t => t != null && t.Item2.Any());
		}
	}
}