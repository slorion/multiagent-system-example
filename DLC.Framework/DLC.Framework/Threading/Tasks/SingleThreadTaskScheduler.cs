using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Framework.Threading.Tasks
{
	public class SingleThreadTaskScheduler
		: TaskScheduler
	{
		private readonly Thread _thread;
		private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();

		public SingleThreadTaskScheduler(string name = null, ThreadPriority priority = ThreadPriority.Normal, ApartmentState apartmentState = ApartmentState.MTA)
		{
			_thread = new Thread(
				() =>
				{
					foreach (var task in _tasks.GetConsumingEnumerable())
						TryExecuteTask(task);
				});

			_thread.IsBackground = true;
			_thread.Name = name;
			_thread.Priority = priority;
			_thread.SetApartmentState(apartmentState);

			_thread.Start();
		}

		public override int MaximumConcurrencyLevel { get { return 1; } }

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _tasks.ToArray();
		}

		protected override void QueueTask(Task task)
		{
			if (task == null) throw new ArgumentNullException("task");

			if (!_thread.IsAlive)
				throw new InvalidOperationException(string.Format("The underlying thread (ManagedThreadId = '{0}') is no longer alive.", _thread.ManagedThreadId));

			lock (_tasks)
			{
				_tasks.Add(task);
			}
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (task == null) throw new ArgumentNullException("task");

			return false;
		}
	}
}