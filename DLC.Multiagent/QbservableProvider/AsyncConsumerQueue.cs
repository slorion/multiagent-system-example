using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QbservableProvider
{
	internal sealed class AsyncConsumerQueue<T>
	{
		private readonly ConcurrentQueue<Tuple<Func<Task<T>>, TaskCompletionSource<T>>> q = new ConcurrentQueue<Tuple<Func<Task<T>>, TaskCompletionSource<T>>>();
		private int isDequeueing;

		public Task<T> EnqueueAsync(Func<Task<T>> actionAsync)
		{
			var task = new TaskCompletionSource<T>();

			q.Enqueue(Tuple.Create(actionAsync, task));

#pragma warning disable 4014
			EnsureDequeueing();
#pragma warning restore 4014

			return task.Task;
		}

		private async Task EnsureDequeueing()
		{
			while (q.Count > 0 && Interlocked.CompareExchange(ref isDequeueing, 1, 0) == 0)
			{
				Tuple<Func<Task<T>>, TaskCompletionSource<T>> data;

				if (q.TryDequeue(out data))
				{
					try
					{
						var result = await data.Item1().ConfigureAwait(false);
						data.Item2.SetResult(result);
					}
					catch (OperationCanceledException)
					{
						data.Item2.SetCanceled();
						continue;
					}
					catch (Exception ex)
					{
						data.Item2.SetException(ex);
						continue;
					}
				}

				isDequeueing = 0;
			}
		}
	}
}