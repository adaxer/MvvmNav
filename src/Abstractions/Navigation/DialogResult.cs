namespace ADaxer.MvvmNav.Abstractions.Navigation;

public class DialogResult
{
    public static DialogResult Cancel { get; } = new(false);

    public static DialogResult Ok { get; } = new(true);

    public DialogResult(bool isConfirmed)
    {
        IsConfirmed = isConfirmed;
    }

    public bool IsConfirmed { get; }
}

public sealed class DialogResult<TResult> : DialogResult
{
    public DialogResult(bool isConfirmed, TResult? value = default)
        : base(isConfirmed)
    {
        Value = value;
    }

    public TResult? Value { get; }
}
