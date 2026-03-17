namespace ADaxer.MvvmNav.Abstractions.Navigation;

public class DialogResult
{
    public static DialogResult None { get; } = new(default);

    public static DialogResult False { get; } = new(false);

    public static DialogResult True { get; } = new(true);

    public DialogResult(bool? isConfirmed)
    {
        IsConfirmed = isConfirmed;
    }

    public bool? IsConfirmed { get; }
}

public sealed class DialogResult<TResult> : DialogResult
{
    public DialogResult(bool? isConfirmed, TResult? value = default)
        : base(isConfirmed)
    {
        Value = value;
    }
    public DialogResult(DialogResult fromResult, TResult? value = default)
        : base(fromResult.IsConfirmed)
    {
        Value = value;
    }

    public TResult? Value { get; }
}
