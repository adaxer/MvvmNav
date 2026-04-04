using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Maui.Views;

/// <summary>
/// Default MAUI host view for dialogs shown by MvvmNav.
/// </summary>
/// <remarks>
/// This view acts as a dialog shell inside an overlay hosted by the shell view.
/// The actual dialog content is resolved through the inner <c>ContentControl</c>
/// using the current dialog view model as binding context.
/// 
/// The command bar is driven by <see cref="IDialogExchange"/>. If the bound
/// dialog view model does not provide dialog exchange metadata, a default
/// single OK command is used.
/// </remarks>
public partial class MauiDialog : ContentView
{
    private static readonly IReadOnlyList<DialogCommandInfo> DefaultCommands =
    [
        new DialogCommandInfo("OK", DialogResult.True) { IsPrimary = true }
    ];

    /// <summary>
    /// Identifies the <see cref="EffectiveCommands"/> bindable property.
    /// </summary>
    private static readonly BindablePropertyKey EffectiveCommandsPropertyKey =
        BindableProperty.CreateReadOnly(
            nameof(EffectiveCommands),
            typeof(IReadOnlyList<DialogCommandInfo>),
            typeof(MauiDialog),
            DefaultCommands);

    /// <summary>
    /// Identifies the <see cref="EffectiveCommands"/> bindable property.
    /// </summary>
    public static readonly BindableProperty EffectiveCommandsProperty =
        EffectiveCommandsPropertyKey.BindableProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MauiDialog"/> class.
    /// </summary>
    public MauiDialog()
    {
        InitializeComponent();
        ExecuteDialogCommand = new Command<DialogCommandInfo>(async command => await ExecuteCommandAsync(command));
    }

    /// <summary>
    /// Gets the commands currently shown by the dialog host.
    /// </summary>
    /// <remarks>
    /// This property contains either the commands provided by the bound
    /// dialog view model through <see cref="IDialogExchange"/> or a default
    /// fallback command if no custom commands are defined.
    /// </remarks>
    public IReadOnlyList<DialogCommandInfo> EffectiveCommands
    {
        get => (IReadOnlyList<DialogCommandInfo>)GetValue(EffectiveCommandsProperty);
        private set => SetValue(EffectiveCommandsPropertyKey, value);
    }

    /// <summary>
    /// Gets the command used by the command bar buttons.
    /// </summary>
    /// <remarks>
    /// The clicked <see cref="DialogCommandInfo"/> is passed as the command
    /// parameter and processed by the dialog host.
    /// </remarks>
    public ICommand ExecuteDialogCommand { get; }

    /// <inheritdoc />
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        EffectiveCommands = ResolveCommands();
    }

    /// <summary>
    /// Resolves the commands that should be displayed by the dialog host.
    /// </summary>
    /// <returns>
    /// The effective dialog commands.
    /// </returns>
    /// <remarks>
    /// If the bound dialog view model implements <see cref="IDialogExchange"/>
    /// and exposes one or more commands, those commands are used.
    /// Otherwise a default single OK command is returned.
    /// </remarks>
    private IReadOnlyList<DialogCommandInfo> ResolveCommands()
    {
        if (BindingContext is IDialogExchange exchange &&
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
    /// <remarks>
    /// If the bound dialog view model provides a dialog exchange callback,
    /// that callback is invoked first to decide whether the dialog should
    /// actually close.
    /// 
    /// If the callback returns <see langword="true"/>, or if no callback is
    /// provided, the dialog is closed using the command's
    /// <see cref="DialogCommandInfo.Result"/>.
    /// 
    /// If the callback returns <see langword="false"/>, the dialog remains open.
    /// </remarks>
    private async Task ExecuteCommandAsync(DialogCommandInfo? commandInfo)
    {
        if (commandInfo is null ||
            BindingContext is not IDialogController dialogController)
        {
            return;
        }

        if (BindingContext is IDialogExchange exchange &&
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
