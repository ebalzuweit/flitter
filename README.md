# flitter

an event streaming library

## EventHub

A simple, in-memory event hub.

``` csharp
using flitter.Events;

// Create an EventHub
var hub = new EventHub();
// Create a subscription
var token = hub.Subscribe(
	handler: @event => Task.CompletedTask,
	predicate: @event => @event is Event);
// Publish an event
await hub.Publish(new Event());
// Unsubscribe with token
hub.Unsubscribe(token);
```