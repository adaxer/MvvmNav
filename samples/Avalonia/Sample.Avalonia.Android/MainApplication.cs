using Android.Runtime;
using Avalonia.Android;

namespace ADaxer.MvvmNav.Sample.Avalonia.Android;

[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }
}
