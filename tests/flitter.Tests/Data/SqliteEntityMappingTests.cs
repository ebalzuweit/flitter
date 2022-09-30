using flitter.Data;

namespace flitter.Tests.Data;

public class SqliteEntityMappingTests
{
	[Theory]
	[InlineData(typeof(int))]
	[InlineData(typeof(bool))]
	public void Ctor_ThrowsArgumentException_IfValueType(Type type)
	{
		var act = () => new SqliteEntityMapping(type);

		var excp = Assert.Throws<ArgumentException>(act);
		Assert.Contains("Type must be a reference type.", excp.Message);
	}

	[Fact]
	public void Ctor_ThrowsArgumentException_IfAbstract()
	{
		var act = () => new SqliteEntityMapping(typeof(AbstractTestEntity));

		var excp = Assert.Throws<ArgumentException>(act);
		Assert.Contains("Type must be concrete.", excp.Message);
	}

	[Fact]
	public void Ctor_ThrowsArgumentException_IfNoEmptyCtor()
	{
		var act = () => new SqliteEntityMapping(typeof(TestEntity));

		var excp = Assert.Throws<ArgumentException>(act);
		Assert.Contains("Type must have a public, empty constructor.", excp.Message);
	}

	internal abstract class AbstractTestEntity { }

	internal class TestEntity
	{
		public TestEntity(int id) { }
	}
}