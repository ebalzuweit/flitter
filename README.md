# flitter

simple, in-memory tools

## EventHub

An event hub.

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

## FlitterContext

An object-database mapper

``` csharp
using System.ComponentModel.DataAnnotations;
using flitter.Data;

const string filename = "flitter.db";

class Person
{
	[Key, Required, AutoIncrement] public int Id { get; }
	[Required] public string Name { get; init; }
	public string? Surname { get; init; }

	public Person() : this(string.Empty, null) { }

	public Person(string name, string? surname)
	{
		Id = -1;
		Name = name;
		Surname = surname;
	}
}

// setup in-memory database
await using FlitterContext ctx = new();
ctx.RegisterEntity<Person>();
// create table and entity
await ctx.CreateTableAsync<Person>();
await ctx.InsertEntityAsync<Person>(new Person("John", "Smith"));
// save to file
await ctx.Save(filename);
// load file
await using var ctx2 = new FlitterContext($"Data Source={filename}");
ctx2.RegisterEntity<Person>();
var people = await ctx2.GetEntitiesAsync<Person>();
```

### ICommand

The `ICommand` interface is used to interact with the database.
`FlitterContext` creates a new connection when a command is executed, and will cleanup after execution.

``` csharp
using Dapper;
using flitter.Data;
using Microsoft.Data.Sqlite;

class ExampleCommand : ICommand<int>
{
	public async Task<int> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
		=> await connection.ExecuteScalarAsync<int>("SELECT 0 FROM sqlite_master LIMIT 1");
}

await using FlitterContext ctx = new();
var command = new ExampleCommand();
var result = await ctx.ExecuteAsync(command);
```