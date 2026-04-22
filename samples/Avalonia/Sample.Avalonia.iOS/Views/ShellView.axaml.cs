using System.ComponentModel;
using System.Runtime.CompilerServices;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ADaxer.MvvmNav.Avalonia.Extensions;

namespace ADaxer.MvvmNav.Sample.Avalonia.iOS.Views;

public partial class ShellView : UserControl, IShellView
, INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;
    private bool _isPaneOpen = false;
    private double _widthThreshold = 900;

    public static readonly FuncValueConverter<string, string> FirstTwoCharsConverter =
        new FuncValueConverter<string, string>(value =>
            value?.Length >= 2 ? value.Substring(0, 2) : value ?? "");

    public static readonly FuncValueConverter<string, string> TitleWithoutEmojiConverter =
        new FuncValueConverter<string, string>(value =>
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Find the first space and return everything after it
            int spaceIndex = value.IndexOf(' ');
            return spaceIndex >= 0 && spaceIndex < value.Length - 1
                ? value.Substring(spaceIndex + 1)
                : value;
        });

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set
        {
            if (_isPaneOpen == value)
                return;

            _isPaneOpen = value;
            OnPropertyChanged();
        }
    }

    public double WidthThreshold
    {
        get => _widthThreshold;
        private set
        {
            if (Math.Abs(_widthThreshold - value) < 0.1)
                return;

            _widthThreshold = value;
            OnPropertyChanged();
        }
    }

    public ShellView(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeComponent();

        AttachedToVisualTree += OnLoaded;
        DetachedFromVisualTree += OnUnloaded;
        this.GetObservable(BoundsProperty).Subscribe(_ => ApplyResponsiveExpansion());
    }

    private void OnLoaded(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _navigationService.NavigationStateChanged += OnNavigationStateChanged;
        ApplyResponsiveExpansion();
    }

    private void OnUnloaded(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _navigationService.NavigationStateChanged -= OnNavigationStateChanged;
    }

    private void OnNavigationStateChanged(object? sender, EventArgs e)
    {
        ApplyResponsiveExpansion();
    }

    private void ApplyResponsiveExpansion()
    {
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        WidthThreshold = DetermineWidthThreshold();

        IsPaneOpen = bounds.Width >= WidthThreshold;
    }

    private double DetermineWidthThreshold()
    {
        var bounds = Bounds;
        bool isLandscape = bounds.Width > bounds.Height;

        // Using same thresholds as MAUI ShellPage
        if (isLandscape)
            return 800;

        return 700;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
