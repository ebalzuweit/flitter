namespace flitter.Events;

public sealed class EventSubscription
{
	public SubscriptionToken Token { get; init; }
	public Func<Event, Task> Handler { get; init; }

	public EventSubscription(Func<Event, Task> handler)
	{
		Handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
		Token = SubscriptionToken.New();
	}
}