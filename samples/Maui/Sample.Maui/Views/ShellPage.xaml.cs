using System.Globalization;
using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Maui.Devices;

namespace Sample.Maui.Views;

public partial class ShellPage : ContentPage, IShellView
{
    private readonly INavigationService _navigationService;

    private bool _isExpanded = true;
    private double _widthThreshold = 900;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
                return;

            _isExpanded = value;
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

    public ShellPage(IShellViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        DataContext  = viewModel;
        _navigationService = navigationService;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    public object? DataContext
    {
        get => BindingContext;
        set => BindingContext = value;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _navigationService.NavigationStateChanged += OnNavigationStateChanged;
        ApplyResponsiveExpansion();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _navigationService.NavigationStateChanged -= OnNavigationStateChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        ApplyResponsiveExpansion();
    }

    private void OnNavigationStateChanged(object? sender, EventArgs e)
    {
        ApplyResponsiveExpansion();
    }

    private void OnHamburgerClicked(object? sender, EventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    private void ApplyResponsiveExpansion()
    {
        if (Width <= 0 || Height <= 0)
            return;

        WidthThreshold = DetermineWidthThreshold();

        IsExpanded = Width >= WidthThreshold;
    }

    private double DetermineWidthThreshold()
    {
        bool isDesktop =
            DeviceInfo.Current.Platform == DevicePlatform.WinUI ||
            DeviceInfo.Current.Idiom == DeviceIdiom.Desktop;

        bool isTabletLandscape =
            DeviceInfo.Current.Idiom == DeviceIdiom.Tablet &&
            Width > Height;

        bool isPhone =
            DeviceInfo.Current.Idiom == DeviceIdiom.Phone;

        if (isDesktop)
            return 1000;

        if (isTabletLandscape)
            return 800;

        if (isPhone)
            return 700;

        return 800;
    }
}

public sealed class NavTitleConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string title = values.Length > 0
            ? values[0]?.ToString() ?? string.Empty
            : string.Empty;

        bool isExpanded = values.Length > 1 &&
                          values[1] is bool b &&
                          b;

        if (string.IsNullOrEmpty(title))
            return string.Empty;

        return isExpanded
            ? title
            : title.Substring(0, 2);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
