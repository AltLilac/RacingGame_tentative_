using System;

public interface IEventTrigger<T>
{
	IObservable<T> OnBeginEvent
	{
		get;
	}
}
