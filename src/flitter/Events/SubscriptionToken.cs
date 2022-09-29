namespace flitter.Events;

public struct SubscriptionToken
{
	public Guid Value { get; init; }

	public SubscriptionToken(Guid value)
	{
		Value = value;
	}

	public static SubscriptionToken New() => new SubscriptionToken(Guid.NewGuid());
}