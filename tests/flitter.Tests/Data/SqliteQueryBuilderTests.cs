using System.ComponentModel.DataAnnotations;
using flitter.Data;

namespace flitter.Tests.Data;

public class SqliteQueryBuilderTests
{
	[Fact]
	public void CreateTableScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var createTableScript = queryBuilder.CreateTableScript();

		Assert.Equal("CREATE TABLE IF NOT EXISTS TestEntity (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Message TEXT)", createTableScript);
	}

	[Fact]
	public void InsertEntityScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var insertEntityScript = queryBuilder.InsertEntityScript();

		Assert.Equal("INSERT INTO TestEntity (Message) VALUES (:Message)", insertEntityScript);
	}

	[Fact]
	public void SelectEntitiesScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var selectEntitiesScript = queryBuilder.SelectEntitiesScript();

		Assert.Equal("SELECT * FROM TestEntity", selectEntitiesScript);
	}

	[Fact]
	public void SelectEntityByPrimaryKeyScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var selectEntityByIdScript = queryBuilder.SelectEntityByPrimaryKeyScript();

		Assert.Equal("SELECT * FROM TestEntity WHERE Id = :Id", selectEntityByIdScript);
	}

	[Fact]
	public void UpdateEntityByPrimaryKeyScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var updateEntityByIdScript = queryBuilder.UpdateEntityByPrimaryKeyScript();

		Assert.Equal("UPDATE TestEntity SET Message = :Message WHERE Id = :Id", updateEntityByIdScript);
	}

	[Fact]
	public void DeleteEntityByPrimaryKeyScript_ReturnsSQLiteScript()
	{
		var queryBuilder = new SqliteQueryBuilder(typeof(TestEntity));

		var deleteEntityByIdScript = queryBuilder.DeleteEntityByPrimaryKeyScript();

		Assert.Equal("DELETE FROM TestEntity WHERE Id = :Id", deleteEntityByIdScript);
	}

	internal class TestEntity
	{
		[Key, Required, AutoIncrement]
		public int Id { get; init; }

		public string Message { get; init; } = string.Empty;
	}
}