namespace flitter.Events;

public sealed class EventHub<T>
{
	private readonly List<EventSubscription<T>> _subscriptions = new();

	public Task Publish(T @event)
	{
		var tasks = _subscriptions.Where(x => x.Predicate(@event))
			.Select(x => x.Handler(@event));
		return Task.WhenAll(tasks);
	}

	public SubscriptionToken Subscribe(Func<T, Task> handler, Func<T, bool>? predicate = null)
	{
		var subscription = new EventSubscription<T>(handler, predicate);
		_subscriptions.Add(subscription);
		return subscription.Token;
	}

	public bool Unsubscribe(SubscriptionToken token)
	{
		var removed = _subscriptions.RemoveAll(subscription => subscription.Token.Value == token.Value);
		return removed > 0;
	}
}