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
		FlitterContext ctx = new();

		await ctx.ExecuteAsync(new CreateDatabaseCommand());
		await ctx.ExecuteAsync(new InsertEventCommand(@event));
		File.Delete(filename);
		await ctx.ExecuteAsync(new SaveDatabaseToFileCommand(filename));

		var ctx2 = new FlitterContext($"Data Source={filename}");
		var events = await ctx.ExecuteAsync(new GetEventsCommand());

		Assert.True(File.Exists(filename));
		Assert.NotEmpty(events);
	}
}