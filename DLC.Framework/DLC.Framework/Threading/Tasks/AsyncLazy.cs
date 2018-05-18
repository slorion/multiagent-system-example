﻿using System;
using System.Threading.Tasks;

namespace DLC.Framework.Threading.Tasks
{
	public class AsyncLazy<T>
		: Lazy<Task<T>>
	{
		public AsyncLazy(Func<T> valueFactory)
			: base(() => Task.Factory.StartNew(valueFactory))
		{
		}

		public AsyncLazy(Func<Task<T>> taskFactory)
			: base(() => Task.Factory.StartNew(taskFactory).Unwrap())
		{
		}
	}
}