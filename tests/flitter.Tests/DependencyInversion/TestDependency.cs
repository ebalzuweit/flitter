namespace flitter.Tests.DependencyInversion;

public record TestDependency() : ITestInterface
{
    public int IntParam { get; set; }
};