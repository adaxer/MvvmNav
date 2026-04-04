namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Provides a mechanism to resolve views for view models.
/// </summary>
/// <remarks>
/// Implementations may support different visual roles for the same
/// view model, for example:
/// <list type="bullet">
/// <item><description>a normal content view</description></item>
/// <item><description>a dialog host view</description></item>
/// </list>
/// </remarks>
public interface IViewLocator
{
    /// <summary>
    /// Resolves and creates the normal view for the specified view model.
    /// </summary>
    /// <param name="viewModel">
    /// The view model instance for which a normal view should be resolved.
    /// </param>
    /// <returns>
    /// The created view instance.
    /// </returns>
    object ResolveView(object viewModel);

    /// <summary>
    /// Resolves and creates the dialog host view for the specified view model.
    /// </summary>
    /// <param name="viewModel">
    /// The view model instance for which a dialog host view should be resolved.
    /// </param>
    /// <returns>
    /// The created dialog host view instance.
    /// </returns>
    object ResolveDialog(object viewModel);
}
