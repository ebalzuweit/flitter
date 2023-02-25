using flitter.DependencyInjection;
using Moq;

namespace flitter.Tests.DependencyInjection;

public class ServiceProviderTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mock = new Mock<FlitterContainer>();

        var provider = new ServiceProvider(mock.Object);
        
        Assert.NotNull(provider);
    }

    [Fact]
    public void GetRequiredService_ThrowsInvalidOperationException_IfServiceNull()
    {
        var mock = new Mock<FlitterContainer>();
        var provider = new ServiceProvider(mock.Object);

        TestDependency Action() => provider.GetRequiredService<TestDependency>();

        var excp = Assert.Throws<InvalidOperationException>((Func<TestDependency>)Action);
        Assert.Contains("no service registered", excp.Message);
    }
}