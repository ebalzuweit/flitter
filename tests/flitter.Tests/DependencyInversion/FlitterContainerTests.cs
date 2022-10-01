using flitter.DependencyInversion;

namespace flitter.Tests.DependencyInversion;

public class FlitterContainerTests
{
	[Fact]
	public void Ctor_Test()
	{
		var container = new FlitterContainer();

		Assert.NotNull(container);
	}

	[Fact]
	public void Get_Test()
	{
		var container = new FlitterContainer();

		container.Register<TestDependency>();
		var dependency = container.Get<TestDependency>();

		Assert.NotNull(dependency);
	}

	internal class TestDependency { }
}