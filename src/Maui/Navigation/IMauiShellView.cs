using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Maui.Hosting;

public interface IMauiShellView : IShellView
{
    object? BindingContext { get; set; }
}
