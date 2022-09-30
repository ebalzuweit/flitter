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

## FlutterContext

A SQLite context using a command-handler pattern.

``` csharp
using flitter.Data;
using flitter.Events;

const string filename = "flitter.db";

// setup in-memory database
FlitterContext ctx = new();
await ctx.ExecuteAsync(new CreateDatabaseCommand());
await ctx.ExecuteAsync(new InsertEventCommand(new Event()));
// save to new file
File.Delete(filename);
await ctx.ExecuteAsync(new SaveDatabaseToFileCommand(filename));
// load file
var ctx2 = new FlitterContext($"Data Source={filename}");
var events = await ctx.ExecuteAsync(new GetEventsCommand());
```