using System.Windows;
using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Wpf.Views;

/// <summary>
/// Default WPF host window for dialogs shown by MvvmNav.
/// </summary>
/// <remarks>
/// The dialog relies on WPF data templates to resolve the actual content view
/// from the assigned dialog view model.
///
/// The command bar is populated from <see cref="IDialogExchange"/> when available.
/// If the bound dialog view model does not provide dialog exchange metadata,
/// a default single OK command is used.
/// </remarks>
public class WpfDialogWindow : Window
{
    private static readonly IReadOnlyList<DialogCommandInfo> DefaultCommands =
    [
        new DialogCommandInfo("OK", ADaxer.MvvmNav.Abstractions.Dialogs.DialogResult.True) { IsPrimary = true }
    ];

    /// <summary>
    /// Initializes static members of the <see cref="WpfDialogWindow"/> class.
    /// </summary>
    static WpfDialogWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfDialogWindow), new FrameworkPropertyMetadata(typeof(WpfDialogWindow)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfDialogWindow"/> class.
    /// </summary>
    /// <remarks>
    /// The window initializes its command handling and observes dialog completion
    /// through the current DataContext when it implements
    /// <see cref="IDialogCompletionSource"/>.
    /// </remarks>
    public WpfDialogWindow()
    {
        ExecuteDialogCommand = new AsyncRelayCommand<DialogCommandInfo>(ExecuteCommandAsync);

        DataContextChanged += (_, _) =>
        {
            EffectiveCommands = ResolveCommands();

            if (DataContext is IDialogCompletionSource completion)
            {
                ObserveCompletionAsync(completion);
            }
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

    /// <summary>
    /// Identifies the <see cref="EffectiveCommands"/> dependency property.
    /// </summary>
    private static readonly DependencyPropertyKey EffectiveCommandsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(EffectiveCommands),
            typeof(IReadOnlyList<DialogCommandInfo>),
            typeof(WpfDialogWindow),
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

    /// <summary>
    /// Observes the dialog completion task and closes the host window once it completes.
    /// </summary>
    /// <param name="completion">
    /// The dialog completion source.
    /// </param>
    private async void ObserveCompletionAsync(IDialogCompletionSource completion)
    {
        try
        {
            await completion.CompletionTask;
        }
        finally
        {
            if (IsVisible)
            {
                Close();
            }
        }
    }

    /// <summary>
    /// Resolves the commands that should be displayed by the dialog host.
    /// </summary>
    /// <returns>
    /// The effective dialog commands.
    /// </returns>
    private IReadOnlyList<DialogCommandInfo> ResolveCommands()
    {
        if (DataContext is IDialogExchange exchange &&
            exchange.DialogExchange.Commands.Count > 0)
        {
            return exchange.DialogExchange.Commands;
        }

        return DefaultCommands;
    }

    /// <summary>
    /// Executes the specified dialog command.
    /// </summary>
    /// <param name="commandInfo">
    /// The dialog command metadata describing the selected user action.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous command execution.
    /// </returns>
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

    /// <inheritdoc />
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDialogCompletionSource completion &&
            DataContext is IDialogController dialogController &&
            !completion.CompletionTask.IsCompleted)
        {
            dialogController.CloseDialog(ADaxer.MvvmNav.Abstractions.Dialogs.DialogResult.None);
        }

        base.OnClosed(e);
    }

}
