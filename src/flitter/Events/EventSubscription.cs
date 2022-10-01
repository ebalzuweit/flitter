namespace flitter.Events;

public sealed class EventSubscription<T>
{
	public SubscriptionToken Token { get; init; }
	public Func<T, Task> Handler { get; init; }
	public Func<T, bool> Predicate { get; init; }

	public EventSubscription(Func<T, Task> handler, Func<T, bool>? predicate = null)
	{
		Handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
		Predicate = predicate ?? DefaultPredicate;
		Token = SubscriptionToken.New();
	}

	private bool DefaultPredicate(T @event) => true;
}