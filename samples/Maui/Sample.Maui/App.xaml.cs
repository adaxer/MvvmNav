using ADaxer.MvvmNav.Maui.Hosting;

namespace ADaxer.MvvmNav.Sample.Maui;

public partial class App : Application
{
    private readonly IMvvmNavStarter _starter;

    public App(IMvvmNavStarter starter)
    {
        InitializeComponent();
        _starter = starter;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return _starter.CreateWindow(activationState);
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _starter.StartAsync();
    }
}
