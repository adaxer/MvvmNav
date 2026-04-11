using Avalonia.Reactive;

namespace ADaxer.MvvmNav.Avalonia.Extensions;

/// <summary>
/// Provides convenience helpers for subscribing to observables.
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// Subscribes to the observable and forwards each value to the provided callback.
    /// </summary>
    /// <typeparam name="T">
    /// The type produced by the observable.
    /// </typeparam>
    /// <param name="observable">
    /// The observable sequence to subscribe to.
    /// </param>
    /// <param name="onNext">
    /// The callback invoked for each published value.
    /// </param>
    /// <returns>
    /// A disposable that cancels the subscription.
    /// </returns>
    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
    {
        return observable.Subscribe(new AnonymousObserver<T>(onNext));
    }
}
