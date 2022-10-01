using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace flitter.DependencyInversion;

public class FlitterContainer : IDisposable
{
	private readonly WindsorContainer _container;

	public FlitterContainer()
	{
		_container = BuildContainer();
	}

	public WindsorContainer BuildContainer()
	{
		var assembly = Assembly.GetAssembly(typeof(FlitterContainer));
		var container = new WindsorContainer();

		return container;
	}

	public T Get<T>() => _container.Resolve<T>();

	public FlitterContainer Register<T>()
		where T : class
	{
		_container.Register(Component.For<T>());
		return this;
	}

	public FlitterContainer Register<T>(T instance)
		where T : class
	{
		_container.Register(Component.For<T>().Instance(instance));
		return this;
	}

	public FlitterContainer Register<TAbs, TImp>()
		where TAbs : class
		where TImp : class, TAbs
	{
		_container.Register(Component.For<TAbs>().ImplementedBy<TImp>());
		return this;
	}

	public void Dispose()
	{
		_container?.Dispose();
		GC.SuppressFinalize(this);
	}
}