using System;

namespace DLC.Framework
{
	public class EventArgs<T>
		: EventArgs
	{
		public T Value { get; set; }
	}
}