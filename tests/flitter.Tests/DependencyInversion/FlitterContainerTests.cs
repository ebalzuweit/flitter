using flitter.DependencyInversion;

namespace flitter.Tests.DependencyInversion;

public class FlitterContainerTests
{
	[Fact]
	public void Ctor_Test()
	{
		var container = new FlitterContainer();

		Assert.NotNull(container);
		Assert.NotNull(container.Provider);
	}

	[Fact]
	public void Get_ReturnsNull_IfFailToResolve()
	{
		var container = new FlitterContainer();

		var resolved = container.Get<TestDependency>();
		
		Assert.Null(resolved);
	}

	[Fact]
	public void Get_ResolvesService_IfRegistered()
	{
		var container = new FlitterContainer()
			.Register<TestDependency>();

		var resolved = container.Get<TestDependency>();

		Assert.NotNull(resolved);
		Assert.IsType<TestDependency>(resolved);
	}

	[Fact]
	public void Get_ResolvesInstance_IfInstanceRegistered()
	{
		var inst = new TestDependency() { IntParam = 123 };
		var container = new FlitterContainer()
			.Register(inst);

		var resolved = container.Get<TestDependency>();
		
		Assert.NotNull(resolved);
		Assert.Equal(inst, resolved);
	}

	[Fact]
	public void Get_ResolvesImplService_IfImplRegistered()
	{
		var container = new FlitterContainer()
			.Register<ITestInterface, TestDependency>();

		var resolved = container.Get<ITestInterface>();
		
		Assert.NotNull(resolved);
		Assert.IsType<TestDependency>(resolved);
	}
}