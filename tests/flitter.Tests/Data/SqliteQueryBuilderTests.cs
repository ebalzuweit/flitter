using System.ComponentModel.DataAnnotations;
using Dapper;
using flitter.Data;
using flitter.Data.Commands;
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

		Assert.Equal("CREATE TABLE IF NOT EXISTS TestEntity (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)", createTableScript);
	}

	internal class TestEntity
	{
		[Key, Required, AutoIncrement]
		public int Id { get; init; }
	}

	internal class TestCommand : ICommand
	{
		public string Text { get; init; }

		public TestCommand(string commandText)
		{
			Text = commandText;
		}

		public async Task ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
		{
			await connection.ExecuteAsync(Text);
		}
	}
}