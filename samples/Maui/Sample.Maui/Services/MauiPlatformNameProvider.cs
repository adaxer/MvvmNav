using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Maui;

internal class MauiPlatformNameProvider: IPlatformNameProvider
{
    public string Name => $"Maui {DeviceInfo.Platform}";
}
