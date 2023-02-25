namespace flitter.DependencyInjection;

/// <summary>
/// Resolves services from the <see cref="FlitterContainer"/>.
/// </summary>
public sealed class ServiceProvider
{
    private readonly FlitterContainer _container;

    public ServiceProvider(FlitterContainer container)
        => _container = container ?? throw new ArgumentNullException(nameof(container));

    /// <inheritdoc cref="FlitterContainer.Get{T}()"/>
    public T? GetService<T>() where T : class => _container.Get<T>();
    
    /// <returns>The resolved service.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no type is registered for the service.</exception>
    /// <inheritdoc cref="GetService{T}"/>
    public T GetRequiredService<T>() where T : class
        => GetService<T>() ??
           throw new InvalidOperationException($"There is no service registered of type {nameof(T)}.");
}