namespace ADaxer.MvvmNav.Abstractions.Navigation;

public sealed class NavigationOptions
{
    public static NavigationOptions Default { get; } = new();

    public bool AddToBackStack { get; init; } = true;

    public bool ClearBackStack { get; init; }

    public bool IsModal { get; init; }
}
