using System.ComponentModel.DataAnnotations;
using flitter.Data;
namespace flitter.Tests.Data;

public class FlitterContextTests
{
	[Fact]
	public async Task PerformDatabaseTests()
	{
		const string filename = "flitter.db";

		// setup in-memory database
		await using FlitterContext ctx = new();
		ctx.RegisterEntity<TestEntity>();
		await ctx.CreateTableAsync<TestEntity>();
		await ctx.InsertEntityAsync<TestEntity>(new TestEntity("!@#$%"));
		// save to new file
		File.Delete(filename);
		await ctx.Save(filename);
		// load file
		await using var ctx2 = new FlitterContext($"Data Source={filename}");
		var events = await ctx.GetEntitiesAsync<TestEntity>();

		Assert.True(File.Exists(filename));
		Assert.NotEmpty(events);
		Assert.All(events, @event => Assert.True(@event.Id > 0));
	}

	internal class TestEntity
	{
		[Key, Required, AutoIncrement]
		public int Id { get; init; }

		public string? Message { get; init; }

		public TestEntity() : this(null) { }

		public TestEntity(string? message)
		{
			Id = -1;
			Message = message;
		}
	}
}