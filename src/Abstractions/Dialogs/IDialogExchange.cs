namespace ADaxer.MvvmNav.Abstractions.Dialogs;

/// <summary>
/// Provides dialog interaction metadata and behavior for a dialog view model.
/// </summary>
/// <remarks>
/// Implement this interface on a dialog view model to describe how the dialog
/// host should render interaction elements (such as command buttons) and how
/// user actions should be processed.
/// 
/// This enables a view model to:
/// <list type="bullet">
/// <item><description>define available dialog commands</description></item>
/// <item><description>control whether the dialog should close</description></item>
/// <item><description>perform validation or intermediate actions before closing</description></item>
/// </list>
/// 
/// If this interface is not implemented, the dialog host will typically fall
/// back to a default behavior (e.g. a single "OK" command).
/// </remarks>
public interface IDialogExchange
{
    /// <summary>
    /// Gets the dialog exchange information used by the dialog host.
    /// </summary>
    DialogExchangeInfo DialogExchange { get; }
}
