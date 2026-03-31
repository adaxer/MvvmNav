namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a service responsible for navigation between view models
/// in an MVVM-based application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Occurs when the navigation state changes.
    /// </summary>
    /// <remarks>
    /// This event is raised after:
    /// <list type="bullet">
    /// <item><description>a successful navigation</description></item>
    /// <item><description>a successful back navigation</description></item>
    /// </list>
    /// It is not raised for dialogs or blocked/cancelled navigation attempts.
    /// </remarks>
    event EventHandler? NavigationStateChanged;

    /// <summary>
    /// Determines whether the navigation service can navigate back
    /// to a previously visited target.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a previous navigation entry exists;
    /// otherwise <see langword="false"/>.
    /// </returns>
    bool CanGoBack();

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The target view model type.
    /// </typeparam>
    /// <param name="parameters">
    /// Optional navigation parameters.
    /// </param>
    /// <param name="options">
    /// Optional navigation behavior settings.
    /// </param>
    Task NavigateAsync<TTarget>(
        NavigationParameters? parameters = null,
        NavigationOptions? options = null)
        where TTarget : class;

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    /// <param name="targetType">
    /// The target view model type.
    /// </param>
    /// <param name="parameters">
    /// Optional navigation parameters.
    /// </param>
    /// <param name="options">
    /// Optional navigation behavior settings.
    /// </param>
    Task NavigateAsync(
        Type targetType,
        NavigationParameters? parameters = null,
        NavigationOptions? options = null);

    /// <summary>
    /// Navigates back to the previous entry in the navigation stack.
    /// </summary>
    Task GoBackAsync();

    /// <summary>
    /// Displays a dialog for the specified dialog type.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog view model type.
    /// </typeparam>
    /// <param name="parameters">
    /// Optional dialog parameters.
    /// </param>
    /// <returns>
    /// The dialog result.
    /// </returns>
    Task<DialogResult> ShowDialogAsync<TDialog>(
        NavigationParameters? parameters = null)
        where TDialog : class;

    /// <summary>
    /// Displays a dialog for the specified dialog type.
    /// </summary>
    /// <param name="dialogType">
    /// The dialog view model type.
    /// </param>
    /// <param name="parameters">
    /// Optional dialog parameters.
    /// </param>
    /// <returns>
    /// The dialog result.
    /// </returns>
    Task<DialogResult> ShowDialogAsync(
        Type dialogType,
        NavigationParameters? parameters = null);

    /// <summary>
    /// Displays a dialog for the specified dialog type and returns
    /// a typed result.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog view model type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The dialog payload type.
    /// </typeparam>
    /// <param name="parameters">
    /// Optional dialog parameters.
    /// </param>
    /// <returns>
    /// The typed dialog result.
    /// </returns>
    Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationParameters? parameters = null)
        where TDialog : class;

    /// <summary>
    /// Displays a dialog for the specified dialog type and returns
    /// a typed result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The dialog payload type.
    /// </typeparam>
    /// <param name="dialogType">
    /// The dialog view model type.
    /// </param>
    /// <param name="parameters">
    /// Optional dialog parameters.
    /// </param>
    /// <returns>
    /// The typed dialog result.
    /// </returns>
    Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        Type dialogType,
        NavigationParameters? parameters = null);
}
