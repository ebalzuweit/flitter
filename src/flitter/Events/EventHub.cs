namespace flitter.Events;

public sealed class EventHub
{
	private readonly List<EventSubscription> _subscriptions = new();

	public async Task Publish(Event @event)
	{
		var tasks = _subscriptions.Select(x => x.Handler(@event));
		await Task.WhenAll(tasks);
	}

	public SubscriptionToken Subscribe(Func<Event, Task> handler)
	{
		var subscription = new EventSubscription(handler);
		_subscriptions.Add(subscription);
		return subscription.Token;
	}

	public bool Unsubscribe(SubscriptionToken token)
	{
		var removed = _subscriptions.RemoveAll(subscription => subscription.Token.Value == token.Value);
		return removed > 0;
	}
}