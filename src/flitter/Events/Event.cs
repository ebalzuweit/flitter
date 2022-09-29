namespace flitter.Events;

public record Event
{
	public long Id { get; init; }
	public Guid Guid { get; init; }
	public DateTime CreatedAt { get; init; }
	public string? EventType { get; init; }
	public string? EventMessage { get; init; }
}