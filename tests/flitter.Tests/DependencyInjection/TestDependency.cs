namespace flitter.Tests.DependencyInjection;

public record TestDependency() : ITestInterface
{
    public int IntParam { get; set; }
};