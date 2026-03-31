namespace ADaxer.MvvmNav.Core;

using ADaxer.MvvmNav.Abstractions;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Default DI-backed implementation of <see cref="IFactory{T}"/>.
/// </summary>
/// <typeparam name="T">
/// The type to resolve.
/// </typeparam>
public sealed class GenericFactory<T> : IFactory<T>
    where T : class
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericFactory{T}"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider used to resolve instances.
    /// </param>
    public GenericFactory(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public T Create()
    {
        return (T)_serviceProvider.GetRequiredService(typeof(T));
    }
}
