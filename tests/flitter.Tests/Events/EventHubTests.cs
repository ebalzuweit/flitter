using flitter.Events;

namespace flitter.Tests.Events;

public class EventHubTests
{
	[Fact]
	public void Ctor()
	{
		var hub = new EventHub<Event>();
	}

	[Fact]
	public async Task README_Test()
	{
		bool eventHandled = false;

		// Create an EventHub
		var hub = new EventHub<Event>();
		// Create a subscription
		var token = hub.Subscribe(
			handler: @event => { eventHandled = true; return Task.CompletedTask; },
			predicate: @event => @event is Event);
		// Publish an event
		await hub.Publish(new Event());
		// Unsubscribe with token
		hub.Unsubscribe(token);

		Assert.True(eventHandled, "Event not handled.");
	}

	internal class Event
	{
		public Event() { }
	}
}