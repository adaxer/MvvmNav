namespace ADaxer.MvvmNav.Maui.Hosting;

public interface IMauiMvvmNavStarter
{
    Window CreateWindow(IActivationState? activationState);
    Task StartAsync(CancellationToken cancellationToken = default);
}
