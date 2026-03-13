namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a service responsible for displaying modal dialogs.
/// </summary>
/// <remarks>
/// Implementations integrate with the UI framework to present dialogs
/// associated with dialog view models.
/// </remarks>
public interface IDialogService
{
    /// <summary>
    /// Displays a dialog for the specified dialog instance.
    /// </summary>
    /// <param name="dialogContent">
    /// The dialog instance to display. This is typically a dialog view model.
    /// It is required to implement the <see cref="IDialogAware"/> interface.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult"/> describing the outcome.
    /// </returns>
    Task<DialogResult> ShowDialogAsync(
        IDialogAware dialogContent,
        NavigationParameters parameters);

    /// <summary>
    /// Displays a dialog for the specified dialog instance and returns a typed result.
    /// </summary>
    /// <typeparam name="TResult">The type of the dialog result payload.</typeparam>
    /// <param name="dialogContent">The dialog instance to display.</param>
    /// <param name="parameters">Optional parameters passed to the dialog.</param>
    /// <returns>
    /// A <see cref="DialogResult{TResult}"/> describing the outcome.
    /// </returns>
    Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        IDialogAware dialogContent,
        NavigationParameters parameters);

    /// <summary>
    /// Displays a simple confirmation dialog.
    /// </summary>
    /// <param name="message">The message shown to the user.</param>
    /// <param name="title">An optional title.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the user confirms; otherwise <c>false</c>.</returns>
    Task<bool> ConfirmAsync(
        string message,
        string? title = null,
        CancellationToken cancellationToken = default);
}
