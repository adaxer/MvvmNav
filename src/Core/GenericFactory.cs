namespace ADaxer.MvvmNav.Core;

using ADaxer.MvvmNav.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="serviceProvider"/> is <c>null</c>.
    /// </exception>
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
