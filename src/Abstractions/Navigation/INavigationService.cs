namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a service responsible for navigation between view models
/// in an MVVM-based application.
/// </summary>
public interface INavigationService
{
    ///// <summary>
    ///// Gets the currently active navigation target.
    ///// </summary>
    //object? Current { get; }

    ///// <summary>
    ///// Occurs when the current navigation target changes.
    ///// </summary>
    //event EventHandler<object?>? CurrentChanged;

    /// <summary>
    /// Determines whether the navigation service can navigate back
    /// to a previously visited target.
    /// </summary>
    bool CanGoBack();

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    Task NavigateAsync<TTarget>(
        NavigationParameters? context = null,
        NavigationOptions? options = null)
        where TTarget : class;

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    Task NavigateAsync(
        Type targetType,
        NavigationParameters? context = null,
        NavigationOptions? options = null);

    /// <summary>
    /// Navigates back to the previous entry in the navigation stack.
    /// </summary>
    Task GoBackAsync();

    /// <summary>
    /// Displays a dialog for the specified dialog type.
    /// </summary>
    Task<DialogResult> ShowDialogAsync<TDialog>(
        NavigationParameters? context = null)
        where TDialog : class;

    /// <summary>
    /// Displays a dialog for the specified dialog type and returns
    /// a typed result.
    /// </summary>
    Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationParameters? context = null)
        where TDialog : class;
}
