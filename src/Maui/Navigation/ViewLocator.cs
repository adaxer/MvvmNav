using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Maui.Navigation;

/// <summary>
/// Default MAUI implementation of <see cref="IViewLocator"/>.
/// </summary>
/// <remarks>
/// This locator supports two mapping categories:
/// <list type="bullet">
/// <item><description>normal content views</description></item>
/// <item><description>dialog host views</description></item>
/// </list>
/// 
/// For dialog host resolution, base type mappings are supported so that
/// a generic dialog shell can be registered for a common base type such
/// as <c>DialogViewModelBase</c>.
/// </remarks>
public sealed class ViewLocator : IViewLocator
{
    private readonly Dictionary<Type, Type> _views = new();
    private readonly Dictionary<Type, Type> _dialogs = new();

    /// <summary>
    /// Gets the current global locator instance.
    /// </summary>
    /// <remarks>
    /// This static reference is intended for controls created from XAML
    /// that are not themselves constructed through dependency injection.
    /// </remarks>
    public static ViewLocator Current { get; internal set; } = new();

    /// <summary>
    /// Registers a normal content view mapping.
    /// </summary>
    /// <param name="viewModelType">
    /// The view model type.
    /// </param>
    /// <param name="viewType">
    /// The corresponding MAUI view type.
    /// </param>
    public void RegisterView(Type viewModelType, Type viewType)
    {
        Register(viewModelType, viewType, _views, "view");
    }

    /// <summary>
    /// Registers a dialog host view mapping.
    /// </summary>
    /// <param name="viewModelType">
    /// The dialog view model type or base type.
    /// </param>
    /// <param name="viewType">
    /// The corresponding MAUI dialog host view type.
    /// </param>
    public void RegisterDialog(Type viewModelType, Type viewType)
    {
        Register(viewModelType, viewType, _dialogs, "dialog");
    }

    /// <inheritdoc />
    public object ResolveView(object viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewType = ResolveMappedType(viewModel.GetType(), _views, allowBaseTypeMatch: false, "view");
        return CreateView(viewType, viewModel);
    }

    /// <inheritdoc />
    public object ResolveDialog(object viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewType = ResolveMappedType(viewModel.GetType(), _dialogs, allowBaseTypeMatch: true, "dialog");
        return CreateView(viewType, viewModel);
    }

    private static void Register(
        Type viewModelType,
        Type viewType,
        IDictionary<Type, Type> map,
        string mappingKind)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);
        ArgumentNullException.ThrowIfNull(viewType);

        if (!typeof(View).IsAssignableFrom(viewType))
        {
            throw new InvalidOperationException(
                $"The registered {mappingKind} type '{viewType.FullName}' must derive from {nameof(View)}.");
        }

        map[viewModelType] = viewType;
    }

    private static Type ResolveMappedType(
        Type viewModelType,
        IReadOnlyDictionary<Type, Type> map,
        bool allowBaseTypeMatch,
        string mappingKind)
    {
        if (map.TryGetValue(viewModelType, out var exactMatch))
        {
            return exactMatch;
        }

        if (!allowBaseTypeMatch)
        {
            throw new InvalidOperationException(
                $"No {mappingKind} registered for view model type '{viewModelType.FullName}'.");
        }

        var current = viewModelType.BaseType;
        while (current is not null)
        {
            if (map.TryGetValue(current, out var baseMatch))
            {
                return baseMatch;
            }

            current = current.BaseType;
        }

        throw new InvalidOperationException(
            $"No {mappingKind} registered for view model type '{viewModelType.FullName}'.");
    }

    private static View CreateView(Type viewType, object viewModel)
    {
        var view = Activator.CreateInstance(viewType) as View;
        if (view is null)
        {
            throw new InvalidOperationException(
                $"Could not create view of type '{viewType.FullName}'.");
        }

        view.BindingContext = viewModel;
        return view;
    }
}
