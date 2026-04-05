using ADaxer.MvvmNav.Abstractions.Dialogs;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Provides convenience overloads for tuple-based navigation and dialog parameters.
/// </summary>
public static class INavigationServiceExtensions
{
    /// <summary>
    /// Navigates to the specified target using tuple-based parameters.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The target view model type.
    /// </typeparam>
    /// <param name="navigationService">
    /// The navigation service.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the target.
    /// </param>
    public static Task NavigateAsync<TTarget>(
        this INavigationService navigationService,
        params (string key, object? value)[] parameters)
        where TTarget : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);

        return navigationService.NavigateAsync<TTarget>(
            new NavigationParameters(parameters));
    }

    /// <summary>
    /// Navigates to the specified target using tuple-based parameters and explicit options.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The target view model type.
    /// </typeparam>
    /// <param name="navigationService">
    /// The navigation service.
    /// </param>
    /// <param name="options">
    /// The navigation options.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the target.
    /// </param>
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

    /// <summary>
    /// Shows the specified dialog using tuple-based parameters.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog view model type.
    /// </typeparam>
    /// <param name="navigationService">
    /// The navigation service.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the dialog.
    /// </param>
    public static Task<DialogResult> ShowDialogAsync<TDialog>(
        this INavigationService navigationService,
        params (string key, object? value)[] parameters)
        where TDialog : class
    {
        ArgumentNullException.ThrowIfNull(navigationService);

        return navigationService.ShowDialogAsync<TDialog>(
            new NavigationParameters(parameters));
    }

    /// <summary>
    /// Shows the specified dialog using tuple-based parameters and returns a typed result.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog view model type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The dialog payload type.
    /// </typeparam>
    /// <param name="navigationService">
    /// The navigation service.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the dialog.
    /// </param>
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
