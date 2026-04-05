using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Wpf.Views;

/// <summary>
/// Default WPF overlay host for dialogs shown inside a shell.
/// </summary>
/// <remarks>
/// The view relies on WPF data templates to resolve the actual content view
/// from the assigned dialog view model.
///
/// The command bar is populated from <see cref="IDialogExchange"/> when available.
/// If the bound dialog view model does not provide dialog exchange metadata,
/// a default single OK command is used.
/// </remarks>
public class WpfDialogView : ContentControl
{
    private static readonly IReadOnlyList<DialogCommandInfo> DefaultCommands =
    [
        new DialogCommandInfo("OK", DialogResult.True) { IsPrimary = true }
    ];

    static WpfDialogView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(WpfDialogView),
            new FrameworkPropertyMetadata(typeof(WpfDialogView)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfDialogView"/> class.
    /// </summary>
    public WpfDialogView()
    {
        ExecuteDialogCommand = new AsyncRelayCommand<DialogCommandInfo>(ExecuteCommandAsync);

        DataContextChanged += (_, _) =>
        {
            EffectiveCommands = ResolveCommands();
        };
    }

    /// <summary>
    /// Gets the commands currently shown by the dialog host.
    /// </summary>
    public IReadOnlyList<DialogCommandInfo> EffectiveCommands
    {
        get => (IReadOnlyList<DialogCommandInfo>)GetValue(EffectiveCommandsProperty);
        private set => SetValue(EffectiveCommandsPropertyKey, value);
    }

    private static readonly DependencyPropertyKey EffectiveCommandsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(EffectiveCommands),
            typeof(IReadOnlyList<DialogCommandInfo>),
            typeof(WpfDialogView),
            new PropertyMetadata(DefaultCommands));

    /// <summary>
    /// Identifies the <see cref="EffectiveCommands"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EffectiveCommandsProperty =
        EffectiveCommandsPropertyKey.DependencyProperty;

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
