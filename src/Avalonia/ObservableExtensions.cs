using Avalonia.Reactive;

namespace ADaxer.MvvmNav.Avalonia;

public static class ObservableExtensions
{
    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
    {
        return observable.Subscribe(new AnonymousObserver<T>(onNext));
    }
}
