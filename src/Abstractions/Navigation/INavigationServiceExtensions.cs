using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

public static class INavigationServiceExtensions
{
    public static Task NavigateAsync<TTarget>(
        this INavigationService navigationService,
        params (string key, object? value)[] parameters)
        where TTarget : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);

        return navigationService.NavigateAsync<TTarget>(
            new NavigationParameters(parameters));
    }

    public static Task NavigateAsync<TTarget>(
        this INavigationService navigationService,
        NavigationOptions options,
        params (string key, object? value)[] parameters)
        where TTarget : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        ArgumentNullException.ThrowIfNull(options);

        return navigationService.NavigateAsync<TTarget>(
            new NavigationParameters(parameters),
            options);
    }

    public static Task ShowDialogAsync<TDialog>(
        this INavigationService navigationService,
        params (string key, object? value)[] parameters)
        where TDialog : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);

        return navigationService.ShowDialogAsync<TDialog>(
            new NavigationParameters(parameters));
    }

    public static Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        this INavigationService navigationService,
        params (string key, object? value)[] parameters)
        where TDialog : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);

        return navigationService.ShowDialogAsync<TDialog, TResult>(
            new NavigationParameters(parameters));
    }
}
