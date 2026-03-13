namespace ADaxer.MvvmNav.Abstractions;

/// <summary>
/// Defines a factory that creates instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type to create.
/// </typeparam>
public interface IFactory<out T>
    where T : class
{
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A resolved instance of <typeparamref name="T"/>.
    /// </returns>
    T Create();
}
