using System.Collections.ObjectModel;
using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Avalonia.Views;

/// <summary>
/// Default Avalonia overlay host for dialogs shown inside a shell.
/// </summary>
/// <remarks>
/// The view relies on Avalonia data templates to resolve the actual content view
/// from the assigned dialog view model.
///
/// The command bar is populated from <see cref="IDialogExchange"/> when available.
/// If the bound dialog view model does not provide dialog exchange metadata,
/// a default single OK command is used.
/// </remarks>
public class AvaloniaDialogView : TemplatedControl
{
    private static readonly IReadOnlyList<DialogCommandInfo> DefaultCommands =
    [
        new DialogCommandInfo("OK", DialogResult.True) { IsPrimary = true }
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaDialogView"/> class.
    /// </summary>
    public AvaloniaDialogView()
    {
        ExecuteDialogCommand = new AsyncRelayCommand<DialogCommandInfo?>(ExecuteCommandAsync);

        EffectiveCommands = new ReadOnlyCollection<DialogCommandInfo>(DefaultCommands.ToList());

        DataContextChanged += (_, _) =>
        {
            EffectiveCommands = ResolveCommands();
        };
    }

    /// <summary>
    /// Identifies the <see cref="EffectiveCommands"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<DialogCommandInfo>> EffectiveCommandsProperty =
        AvaloniaProperty.Register<AvaloniaDialogView, IReadOnlyList<DialogCommandInfo>>(
            nameof(EffectiveCommands),
            DefaultCommands);

    /// <summary>
    /// Gets the commands currently shown by the dialog host.
    /// </summary>
    public IReadOnlyList<DialogCommandInfo> EffectiveCommands
    {
        get => GetValue(EffectiveCommandsProperty);
        private set => SetValue(EffectiveCommandsProperty, value);
    }

    /// <summary>
    /// Gets the command used by the dialog command buttons.
    /// </summary>
    public ICommand ExecuteDialogCommand { get; }

    private IReadOnlyList<DialogCommandInfo> ResolveCommands()
    {
        if (DataContext is IDialogExchange exchange &&
            exchange.DialogExchange.Commands.Count > 0)
        {
            return exchange.DialogExchange.Commands;
        }

        return DefaultCommands;
    }

    private async Task ExecuteCommandAsync(DialogCommandInfo? commandInfo)
    {
        if (commandInfo is null ||
            DataContext is not IDialogController dialogController)
        {
            return;
        }

        if (DataContext is IDialogExchange exchange &&
            exchange.DialogExchange.ContinueAsync is not null)
        {
            var shouldClose = await exchange.DialogExchange.ContinueAsync(
                commandInfo.Result,
                CancellationToken.None);

            if (!shouldClose)
            {
                return;
            }
        }

        dialogController.CloseDialog(commandInfo.Result);
    }
}
