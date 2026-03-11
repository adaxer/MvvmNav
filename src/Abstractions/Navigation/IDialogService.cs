namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a service responsible for displaying modal dialogs.
/// </summary>
/// <remarks>
/// Implementations integrate with the UI framework to present dialogs
/// associated with dialog view models.
/// 
/// The navigation infrastructure uses this service indirectly through
/// <see cref="IDialogHost"/>.
/// </remarks>
public interface IDialogService
{
    /// <summary>
    /// Displays a dialog for the specified dialog instance.
    /// </summary>
    /// <param name="dialog">
    /// The dialog instance to display. This is typically a dialog view model.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// A tuple describing the dialog outcome.
    /// </returns>
    /// <remarks>
    /// The returned boolean indicates whether the dialog was confirmed.
    /// The returned dialog instance can be used to retrieve result data
    /// from the dialog view model.
    /// </remarks>
    Task<(bool result, object? dialog)> ShowDialogAsync(
        object dialog,
        IDictionary<string, object?> parameters);
}
