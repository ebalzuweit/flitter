using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace flitter.DependencyInversion;

/// <summary>
/// Dependency-inversion container for type registration.
/// </summary>
public sealed class FlitterContainer : IDisposable
{
	private readonly ServiceProvider _provider;
	private readonly IWindsorContainer _container;

	public ServiceProvider Provider => _provider;

	public FlitterContainer()
	{
		_provider = new ServiceProvider(this);
		_container = new WindsorContainer()
			.Register(Component.For<ServiceProvider>().Instance(_provider));
	}

	/// <summary>
	/// Register a transient service of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type of the service being registered.</typeparam>
	/// <returns>This container, for call-chaining.</returns>
	public FlitterContainer Register<T>()
		where T : class
	{
		_container.Register(Component.For<T>().LifestyleTransient());
		return this;
	}

	/// <summary>
	/// Register a service of type <typeparamref name="T"/> that resolves to the provided instance.
	/// </summary>
	/// <param name="instance">The instance to resolve.</param>
	/// <inheritdoc cref="Register{T}()"/>
	public FlitterContainer Register<T>(T instance)
		where T : class
	{
		_container.Register(Component.For<T>().Instance(instance));
		return this;
	}

	/// <summary>
	/// Register a transient service of type <typeparamref name="TAbs"/> that will be resolved by type <typeparamref name="TImp"/>.
	/// </summary>
	/// <typeparam name="TAbs">Type of the service being registered.</typeparam>
	/// <typeparam name="TImp">Type of the service implementation.</typeparam>
	/// <returns>This container, for call-chaining.</returns>
	public FlitterContainer Register<TAbs, TImp>()
		where TAbs : class
		where TImp : class, TAbs
	{
		_container.Register(Component.For<TAbs>().ImplementedBy<TImp>().LifestyleTransient());
		return this;
	}

	public void Dispose()
	{
		_container?.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Resolve a service of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type of the service to resolve.</typeparam>
	/// <returns>The service if resolved; otherwise, <c>null</c>.</returns>
	internal T? Get<T>() where T : class
	{
		try
		{
			return _container.Resolve<T>();
		}
		catch (ComponentNotFoundException)
		{
			return null;
		}
	}
}