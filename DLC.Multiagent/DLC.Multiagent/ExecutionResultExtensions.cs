using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	public static class ExecutionResultExtensions
	{
		public static async Task<T> GetValueOrDefault<T>(this Task<ExecutionResult<T>> resultTask, T defaultValue = default(T))
		{
			if (resultTask == null) throw new ArgumentNullException("resultTask");

			var result = await resultTask.ConfigureAwait(false);
			return result.IsSuccessful ? result.Result : defaultValue;
		}

		public static IEnumerable<Task<T>> GetValueOrDefault<T>(this IEnumerable<Task<ExecutionResult<T>>> resultTasks, T defaultValue = default(T))
		{
			if (resultTasks == null) throw new ArgumentNullException("resultTasks");

			foreach (var task in resultTasks)
				yield return task.GetValueOrDefault(defaultValue);
		}

		public static async Task ThrowOnError(this Task<ExecutionResult> resultTask)
		{
			if (resultTask == null) throw new ArgumentNullException("resultTask");

			var result = await resultTask.ConfigureAwait(false);

			if (!result.IsSuccessful)
				throw result.Exception ?? new OperationCanceledException();
		}

		public static IEnumerable<Task> ThrowOnError(this IEnumerable<Task<ExecutionResult>> resultTasks)
		{
			if (resultTasks == null) throw new ArgumentNullException("resultTasks");

			foreach (var task in resultTasks)
				yield return task.ThrowOnError();
		}

		public static async Task<T> GetOrThrow<T>(this Task<ExecutionResult<T>> resultTask)
		{
			if (resultTask == null) throw new ArgumentNullException("resultTask");

			var result = await resultTask.ConfigureAwait(false);

			if (result.IsSuccessful)
				return result.Result;
			else if (result.Exception != null)
				throw result.Exception;
			else
				throw new OperationCanceledException();
		}

		public static IEnumerable<Task<T>> GetOrThrow<T>(this IEnumerable<Task<ExecutionResult<T>>> resultTasks)
		{
			if (resultTasks == null) throw new ArgumentNullException("resultTasks");

			foreach (var task in resultTasks)
				yield return task.GetOrThrow();
		}
	}
}