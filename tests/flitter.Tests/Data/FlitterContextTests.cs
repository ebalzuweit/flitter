using System.ComponentModel.DataAnnotations;
using Dapper;
using flitter.Data;
using Microsoft.Data.Sqlite;

namespace flitter.Tests.Data;

public class FlitterContextTests
{
	[Fact]
	public async Task Ctor_CreatesSharedInMemoryDb()
	{
		// setup in-memory database
		await using FlitterContext ctx = new();
		ctx.RegisterEntity<TestEntity>();
		await ctx.CreateTableAsync<TestEntity>();
		await ctx.InsertEntityAsync<TestEntity>(new TestEntity("!@#$%"));

		// load another context
		await using FlitterContext ctx2 = new();
		ctx2.RegisterEntity<TestEntity>();
		var entities = await ctx2.GetEntitiesAsync<TestEntity>();

		Assert.NotEmpty(entities);
		Assert.All(entities, entity => Assert.True(entity.Id > 0));
	}

	[Fact]
	public async Task README_ContextTest()
	{
		const string filename = "flitter.db";

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
	}

	[Fact]
	public async Task README_CommandTest()
	{
		await using FlitterContext ctx = new();
		var command = new TestCommand();
		var result = await ctx.ExecuteAsync(command);

		Assert.Equal(0, result);
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

	internal class Person
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

	internal class TestCommand : ICommand<int>
	{
		public async Task<int> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
			=> await connection.ExecuteScalarAsync<int>("SELECT 0 FROM sqlite_master LIMIT 1");
	}
}