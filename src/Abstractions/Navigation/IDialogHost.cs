namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a platform-specific component responsible for displaying
/// modal dialogs.
/// </summary>
/// <remarks>
/// Implementations integrate with the UI framework's dialog or window
/// system and are used by the <see cref="INavigationService"/> to show
/// dialogs defined by view models.
/// </remarks>
public interface IDialogHost
{
    /// <summary>
    /// Displays the specified dialog.
    /// </summary>
    /// <param name="dialog">
    /// The dialog instance to display. This is typically a dialog
    /// view model resolved from the dependency injection container.
    /// </param>
    /// <param name="context">
    /// The navigation context containing parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult"/> describing whether the dialog
    /// was confirmed or cancelled.
    /// </returns>
    Task<DialogResult> ShowDialogAsync(
        object dialog,
        NavigationContext context);

    /// <summary>
    /// Displays the specified dialog and returns a typed result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the value returned by the dialog.
    /// </typeparam>
    /// <param name="dialog">
    /// The dialog instance to display.
    /// </param>
    /// <param name="context">
    /// The navigation context containing parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult{TResult}"/> describing the outcome of the dialog
    /// and optionally containing a result value.
    /// </returns>
    Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        object dialog,
        NavigationContext context);
}
