using ADaxer.MvvmNav.Abstractions.Dialogs;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the application's main shell view model.
/// </summary>
/// <remarks>
/// The shell view model acts as the root of the application's visual
/// composition and hosts both the currently active navigation target
/// and the currently active dialog.
/// 
/// UI frameworks typically bind the shell view to this view model and
/// display the current module using a content control or similar
/// mechanism.
/// 
/// The current dialog may be hosted in different ways depending on the
/// platform:
/// <list type="bullet">
/// <item><description>
/// As an overlay within the main UI (typical for MAUI and modern desktop applications)
/// </description></item>
/// <item><description>
/// As a separate window (traditional desktop behavior, e.g. classic WPF dialogs)
/// </description></item>
/// </list>
/// 
/// In MvvmNav, dialogs are typically rendered as an overlay using a
/// <c>ContentView</c>-based host that resolves the dialog view from the
/// dialog view model, although you can fall back to the more traditional way.
/// </remarks>
public interface IShellViewModel : IModuleHost, IDialogHost
{
}
