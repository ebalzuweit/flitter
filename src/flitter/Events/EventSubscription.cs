namespace flitter.Events;

public sealed class EventSubscription
{
	public SubscriptionToken Token { get; init; }
	public Func<IEvent, Task> Handler { get; init; }
	public Func<IEvent, bool> Predicate { get; init; }

	public EventSubscription(Func<IEvent, Task> handler, Func<IEvent, bool>? predicate = null)
	{
		Handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
		Predicate = predicate ?? DefaultPredicate;
		Token = SubscriptionToken.New();
	}

	private bool DefaultPredicate(IEvent @event) => true;
}