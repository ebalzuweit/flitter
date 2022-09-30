using flitter.Data;
using flitter.Data.Commands;
using flitter.Events;

namespace flitter.Tests;

public class FlitterContextTests
{
	[Fact]
	public async Task PerformDatabaseTests()
	{
		const string filename = "flitter.db";
		var @event = new Event();

		// setup in-memory database
		await using FlitterContext ctx = new();
		await ctx.ExecuteAsync(new CreateDatabaseCommand());
		await ctx.ExecuteAsync(new InsertEventCommand(@event));
		// save to new file
		File.Delete(filename);
		await ctx.ExecuteAsync(new SaveDatabaseToFileCommand(filename));
		// load file
		await using var ctx2 = new FlitterContext($"Data Source={filename}");
		var events = await ctx.ExecuteAsync(new GetEventsCommand());

		Assert.True(File.Exists(filename));
		Assert.NotEmpty(events);
		Assert.Contains(@event, events);
	}
}