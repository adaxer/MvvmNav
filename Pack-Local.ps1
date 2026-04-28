$configuration = "Release"
$output = "d:\dev\.nuget"
$abstractions = "src/Abstractions/MvvmNav.Abstractions.csproj"
$core = "src/Core/MvvmNav.Core.csproj"
$wpf = "src/Wpf/MvvmNav.Wpf.csproj"
$maui = "src/Maui/MvvmNav.Maui.csproj"
$avalonia = "src/Avalonia/MvvmNav.Avalonia.csproj"

Write-Host "Build..." -ForegroundColor Cyan
#dotnet build $abstractions -c $configuration
#dotnet build $core -c $configuration
#dotnet build $wpf -c $configuration
#dotnet build $maui -c $configuration
dotnet build $avalonia -c $configuration

Write-Host "Pack ..." -ForegroundColor Cyan
#dotnet pack $abstractions -c $configuration -o $output --no-build
#dotnet pack $core -c $configuration -o $output --no-build
#dotnet pack $wpf -c $configuration -o $output --no-build
#dotnet pack $maui -c $configuration -o $output --no-build
dotnet pack $avalonia -c $configuration -o $output --no-build

Write-Host "Done." -ForegroundColor Green