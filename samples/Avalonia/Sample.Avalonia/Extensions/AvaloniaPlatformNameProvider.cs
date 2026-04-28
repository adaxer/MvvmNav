using System.Runtime.InteropServices;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

internal class AvaloniaPlatformNameProvider : IPlatformNameProvider
{
    public string Name => $"Avalonia ({RuntimeInformation.OSDescription})";
}
