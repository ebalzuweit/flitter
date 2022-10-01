using System.ComponentModel.DataAnnotations;
using Dapper;
using flitter.Data;
using Microsoft.Data.Sqlite;

namespace flitter.Tests.Data;

public class SqliteQueryBuilderTests
{
	[Fact]
	public async Task CreateTableScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();

		var createTableScript = queryBuilder.CreateTableScript();
		await ctx.ExecuteAsync(new TestCommand(createTableScript));

		Assert.Equal("CREATE TABLE IF NOT EXISTS TestEntity (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Message TEXT)", createTableScript);
	}

	[Fact]
	public async Task InsertEntityScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.CreateTableScript()));

		var insertEntityScript = queryBuilder.InsertEntityScript();
		await ctx.ExecuteAsync(new TestCommand(insertEntityScript, new TestEntity()));

		Assert.Equal("INSERT INTO TestEntity (Message) VALUES (:Message)", insertEntityScript);
	}

	[Fact]
	public async Task SelectEntitiesScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.CreateTableScript()));

		var selectEntitiesScript = queryBuilder.SelectEntitiesScript();
		await ctx.ExecuteAsync(new TestCommand(selectEntitiesScript));

		Assert.Equal("SELECT * FROM TestEntity", selectEntitiesScript);
	}

	[Fact]
	public async Task SelectEntityByPrimaryKeyScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.CreateTableScript()));
		// await ctx.ExecuteAsync(new TestCommand(queryBuilder.InsertEntityScript(), new TestEntity()));

		var selectEntityByIdScript = queryBuilder.SelectEntityByPrimaryKeyScript();
		await ctx.ExecuteAsync(new TestCommand(selectEntityByIdScript, new TestEntity()));

		Assert.Equal("SELECT * FROM TestEntity WHERE Id = :Id", selectEntityByIdScript);
	}

	[Fact]
	public async Task UpdateEntityByPrimaryKeyScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.CreateTableScript()));
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.InsertEntityScript(), new TestEntity()));

		var updateEntityByIdScript = queryBuilder.UpdateEntityByPrimaryKeyScript();
		await ctx.ExecuteAsync(new TestCommand(updateEntityByIdScript, new TestEntity()));

		Assert.Equal("UPDATE TestEntity SET Message = :Message WHERE Id = :Id", updateEntityByIdScript);
	}

	[Fact]
	public async Task DeleteEntityByPrimaryKeyScript_ProducesValidSQL()
	{
		var ctx = new FlitterContext();
		var queryBuilder = new SqliteQueryBuilder<TestEntity>();
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.CreateTableScript()));
		await ctx.ExecuteAsync(new TestCommand(queryBuilder.InsertEntityScript(), new TestEntity()));

		var deleteEntityByIdScript = queryBuilder.DeleteEntityByPrimaryKeyScript();
		await ctx.ExecuteAsync(new TestCommand(deleteEntityByIdScript, new TestEntity()));

		Assert.Equal("DELETE FROM TestEntity WHERE Id = :Id", deleteEntityByIdScript);
	}

	internal class TestEntity
	{
		[Key, Required, AutoIncrement]
		public int Id { get; init; }

		public string Message { get; init; } = string.Empty;
	}

	internal class TestCommand : ICommand
	{
		public string Text { get; init; }
		public object? Parameters { get; init; }

		public TestCommand(string commandText, object? parameters = null)
		{
			Text = commandText;
			Parameters = parameters;
		}

		public async Task ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
		{
			await connection.ExecuteAsync(Text, param: Parameters);
		}
	}
}