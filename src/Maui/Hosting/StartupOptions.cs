namespace ADaxer.MvvmNav.Maui.Hosting;

public sealed class StartupOptions
{
    public Type? ShellViewType { get; set; }
    public Type? ShellViewModelType { get; set; }
    public Type? StartupNavigationType { get; set; }
}
