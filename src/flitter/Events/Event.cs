namespace flitter.Events;

public record Event : IEvent
{
	public Guid Guid { get; init; }
	public DateTimeOffset CreatedAt { get; init; }

	public Event()
	{
		Guid = Guid.NewGuid();
		CreatedAt = DateTimeOffset.Now;
	}
}