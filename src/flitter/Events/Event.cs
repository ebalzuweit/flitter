namespace flitter.Events;

public record Event : IEvent
{
	public Guid Guid { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public string Type { get; init; }
	public string? Data { get; init; }

	public Event()
	{
		Guid = Guid.NewGuid();
		CreatedAt = DateTimeOffset.Now;
		Type = string.Empty;
		Data = null;
	}

	public Event(string type, string? data = null) : base()
	{
		Type = type;
		Data = data;
	}
}