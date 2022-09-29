using flitter.Data;
using flitter.Events;

namespace flitter.Tests;

public class FlitterContextTests
{
	[Fact]
	public async Task PerformDatabaseTests()
	{
		var @event = new Event();
		var filename = "flitter.db";

		// setup in-memory database
		FlitterContext ctx = new();
		await ctx.ExecuteAsync(new CreateDatabaseCommand());
		await ctx.ExecuteAsync(new InsertEventCommand(@event));
		// save to new file
		File.Delete(filename);
		await ctx.ExecuteAsync(new SaveDatabaseToFileCommand(filename));
		// load file
		var ctx2 = new FlitterContext($"Data Source={filename}");
		var events = await ctx.ExecuteAsync(new GetEventsCommand());

		Assert.True(File.Exists(filename));
		Assert.NotEmpty(events);
	}
}