namespace flitter.Events;

public sealed class EventHub
{
	private readonly List<EventSubscription> _subscriptions = new();

	public Task Publish(IEvent @event)
	{
		var tasks = _subscriptions.Where(x => x.Predicate(@event))
			.Select(x => x.Handler(@event));
		return Task.WhenAll(tasks);
	}

	public SubscriptionToken Subscribe(Func<IEvent, Task> handler, Func<IEvent, bool>? predicate = null)
	{
		var subscription = new EventSubscription(handler, predicate);
		_subscriptions.Add(subscription);
		return subscription.Token;
	}

	public bool Unsubscribe(SubscriptionToken token)
	{
		var removed = _subscriptions.RemoveAll(subscription => subscription.Token.Value == token.Value);
		return removed > 0;
	}

	private bool DefaultPredicate(Event @event) => true;
}