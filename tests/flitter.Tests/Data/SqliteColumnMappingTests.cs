using System.ComponentModel.DataAnnotations;
using flitter.Data;
using flitter.Data.Internal;

namespace flitter.Tests.Data.Internal;

public class SqliteColumnMappingTests
{
	[Fact]
	public void FromPropertyInfo_MapsIdProperty()
	{
		var idPropInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.Id));

		var mapping = SqliteColumnMapping.FromPropertyInfo(idPropInfo!);

		Assert.NotNull(mapping);
		Assert.Equal(nameof(TestEntity.Id), mapping.Name);
		Assert.Equal(typeof(int), mapping.Type);
		Assert.Equal("INTEGER", mapping.SqliteType);
		Assert.True(mapping.NotNull);
		Assert.True(mapping.IsPrimaryKey);
		Assert.True(mapping.AutoIncrement);
	}

	[Fact]
	public void FromPropertyInfo_MapsMessageProperty()
	{
		var messagePropInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.Message));

		var mapping = SqliteColumnMapping.FromPropertyInfo(messagePropInfo!);

		Assert.NotNull(mapping);
		Assert.Equal(nameof(TestEntity.Message), mapping.Name);
		Assert.Equal(typeof(string), mapping.Type);
		Assert.Equal("TEXT", mapping.SqliteType);
		Assert.True(mapping.NotNull);
		Assert.False(mapping.IsPrimaryKey);
		Assert.False(mapping.AutoIncrement);
	}

	[Fact]
	public void FromPropertyInfo_MapsValueProperty()
	{
		var valuePropInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.Value));

		var mapping = SqliteColumnMapping.FromPropertyInfo(valuePropInfo!);

		Assert.NotNull(mapping);
		Assert.Equal(nameof(TestEntity.Value), mapping.Name);
		Assert.Equal(typeof(float?), mapping.Type);
		Assert.Equal("REAL", mapping.SqliteType);
		Assert.False(mapping.NotNull);
		Assert.False(mapping.IsPrimaryKey);
		Assert.False(mapping.AutoIncrement);
	}

	[Fact]
	public void Ctor_ThrowsArgumentException_IfAutoIncrementOnNonInteger()
	{
		var act = () => new SqliteColumnMapping("!@#$%", typeof(bool), autoIncrement: true);

		var excp = Assert.Throws<ArgumentException>(act);
		Assert.Contains("Auto-increment requires an integral, numeric type.", excp.Message);
	}

	[Fact]
	public void Ctor_ThrowsNotSupportedException_IfByteArray()
	{
		var mapping = new SqliteColumnMapping("!@#$%", typeof(byte[]));

		var act = () => mapping.SqliteType;

		var excp = Assert.Throws<NotSupportedException>(act);
		Assert.Contains("BLOB data type is not supported yet.", excp.Message);
	}

	internal class TestEntity
	{
		[Key, Required, AutoIncrement]
		public int Id { get; init; }

		[Required]
		public string Message { get; init; } = string.Empty;

		public float? Value { get; init; }
	}
}