using flitter.Events;

namespace flitter.Tests.Events;

public class EventHubTests
{
	[Fact]
	public void Ctor_Test()
	{
		var hub = new TestEventHub();
		
		Assert.NotNull(hub);
	}

	[Fact]
	public void Subscribe_ReturnsNonEmptySubscriptionToken()
	{
		var hub = new TestEventHub();

		var token = hub.Subscribe(@event => Task.CompletedTask);
		
		Assert.NotEqual(Guid.Empty, token.Value);
	}

	[Fact]
	public void Unsubscribe_ReturnsFalse_IfInvalidToken()
	{
		var hub = new TestEventHub();
		_ = hub.Subscribe(@event => Task.CompletedTask);

		var result = hub.Unsubscribe(new SubscriptionToken(Guid.Empty));
		
		Assert.False(result);
	}
	
	[Fact]
	public void Unsubscribe_ReturnsTrue_IfValidToken()
	{
		var hub = new TestEventHub();
		var token = hub.Subscribe(@event => Task.CompletedTask);

		var result = hub.Unsubscribe(token);
		
		Assert.True(result);
	}

	[Fact]
	public async Task README_Test()
	{
		bool eventHandled = false;

		// Create an EventHub
		var hub = new EventHub<Event>();
		// Create a subscription
		var token = hub.Subscribe(
			handler: @event => { eventHandled = true; return Task.CompletedTask; });
		// Publish an event
		await hub.Publish(new Event());
		// Unsubscribe with token
		hub.Unsubscribe(token);

		Assert.True(eventHandled, "Event not handled.");
	}

	private record Event();

	private sealed class TestEventHub : EventHub<Event> { }
}