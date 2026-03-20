using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Core.ViewModels;

/// <summary>
/// Provides a common base implementation for view models with
/// basic state and notification support.
/// </summary>
/// <remarks>
/// Inherits from <see cref="ObservableObject"/> to provide property change
/// notifications via the MVVM Toolkit.
///
/// This base class defines commonly used properties such as <see cref="Title"/>
/// and <see cref="IsBusy"/>, which can be used by derived view models for
/// UI representation and state handling.
/// </remarks>
public abstract partial class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <remarks>
    /// The default <see cref="Title"/> is set to the runtime type name
    /// of the derived view model.
    /// </remarks>
    protected ViewModelBase()
    {
        _title = GetType().Name;
    }

    /// <summary>
    /// Gets or sets the title of the view model.
    /// </summary>
    /// <remarks>
    /// This property is typically used for display purposes (e.g. window title,
    /// header text). The default value is the name of the derived view model type.
    /// </remarks>
    [ObservableProperty]
    private string? _title;

    /// <summary>
    /// Gets or sets a value indicating whether the view model is currently busy.
    /// </summary>
    /// <remarks>
    /// This property can be used to indicate ongoing operations such as loading
    /// data or processing user actions, and is typically bound to UI elements
    /// like progress indicators or disabled states.
    /// </remarks>
    [ObservableProperty]
    private bool _isBusy = false;
}
