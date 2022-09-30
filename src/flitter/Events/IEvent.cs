namespace flitter.Events;

public interface IEvent
{
	Guid Guid { get; }
	DateTimeOffset CreatedAt { get; }
	string Type { get; }
	string? Data { get; }
}