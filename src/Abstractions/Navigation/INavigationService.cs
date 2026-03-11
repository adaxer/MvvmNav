namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a service responsible for navigation between view models
/// in an MVVM-based application.
/// </summary>
/// <remarks>
/// Implementations typically resolve target view models from a dependency
/// injection container and delegate the actual presentation to an
/// <see cref="INavigationHost"/>. Navigation parameters can be supplied
/// through a <see cref="NavigationContext"/> instance.
/// </remarks>
public interface INavigationService
{
    /// <summary>
    /// Determines whether the navigation service can navigate back
    /// to a previously visited target.
    /// </summary>
    /// <returns>
    /// <c>true</c> if a previous navigation entry exists in the
    /// navigation stack; otherwise, <c>false</c>.
    /// </returns>
    bool CanGoBack();

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The target type to navigate to. This is typically a view model.
    /// </typeparam>
    /// <param name="context">
    /// Optional navigation parameters passed to the target.
    /// </param>
    /// <param name="options">
    /// Optional navigation options that control behavior such as
    /// back stack handling.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous navigation operation.
    /// </returns>
    Task NavigateAsync<TTarget>(
        NavigationContext? context = null,
        NavigationOptions? options = null)
        where TTarget : class;

    /// <summary>
    /// Navigates to the specified target type.
    /// </summary>
    /// <param name="targetType">
    /// The target type to navigate to. This is typically a view model type
    /// that will be resolved from the dependency injection container.
    /// </param>
    /// <param name="context">
    /// Optional navigation parameters passed to the target.
    /// </param>
    /// <param name="options">
    /// Optional navigation options that control behavior such as
    /// back stack handling.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous navigation operation.
    /// </returns>
    Task NavigateAsync(
        Type targetType,
        NavigationContext? context = null,
        NavigationOptions? options = null);

    /// <summary>
    /// Navigates back to the previous entry in the navigation stack.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous navigation operation.
    /// </returns>
    Task GoBackAsync();

    /// <summary>
    /// Displays a dialog for the specified dialog type.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog type to display. This is typically a dialog view model.
    /// </typeparam>
    /// <param name="context">
    /// Optional parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult"/> describing whether the dialog
    /// was confirmed or cancelled.
    /// </returns>
    Task<DialogResult> ShowDialogAsync<TDialog>(
        NavigationContext? context = null)
        where TDialog : class;

    /// <summary>
    /// Displays a dialog for the specified dialog type and returns
    /// a typed result.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The dialog type to display.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the value returned by the dialog.
    /// </typeparam>
    /// <param name="context">
    /// Optional parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult{TResult}"/> containing the confirmation
    /// state and an optional result value.
    /// </returns>
    Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationContext? context = null)
        where TDialog : class;
}
