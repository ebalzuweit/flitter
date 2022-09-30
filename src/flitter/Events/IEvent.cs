namespace flitter.Events;

public interface IEvent
{
	Guid Guid { get; }
	DateTimeOffset CreatedAt { get; }
}