using ADaxer.MvvmNav.Abstractions.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Core.ViewModels;

/// <summary>
/// Simple dialog view model used for built-in confirmation messages.
/// </summary>
public partial class MessageViewModel : DialogViewModelBase, IDialogExchange
{
    /// <summary>
    /// Gets or sets the message shown in the confirmation dialog.
    /// </summary>
    [ObservableProperty]
    private string _message = string.Empty;

    /// <summary>
    /// CommandInfos handed in from outside, to determine, which buttons should be shown in the dialog. If not set, the views default is used.
    /// </summary>
    public IEnumerable<DialogCommandInfo> CommandInfos { get; set; } = Array.Empty<DialogCommandInfo>();

    /// <inheritdoc/>
    public DialogExchangeInfo DialogExchange => new(CommandInfos.ToList());
}
