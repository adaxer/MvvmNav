# Avalonia iOS File Handling & Stolperfallen

## Kombinierte FileService-Strategie

Die beiden Varianten lassen sich sauber zu einer robusten Lösung
zusammenführen:

-   Primär: Zugriff über Avalonia Assets (avares)
-   Fallback: klassisches File-System (für Debug / lokale Tests)

``` csharp
public async Task<string> GetFileAsync(string folderName, string fileName, CancellationToken cancellationToken = default)
{
    var path = $"{folderName}/{fileName}";

    try
    {
        var uri = new Uri($"avares://ADaxer.MvvmNav.Sample.Avalonia.iOS/{path}");
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Asset load failed, fallback to file system: {Path}", path);

        try
        {
            var fsPath = Path.Combine(AppContext.BaseDirectory, path);
            return await File.ReadAllTextAsync(fsPath, cancellationToken);
        }
        catch (Exception ex2)
        {
            _logger.LogError(ex2, "File system load failed: {Path}", path);
            return "# File not found";
        }
    }
}
```

## Wichtige Erkenntnisse zu Assets

-   iOS kennt **kein relatives File-System wie Desktop**
-   Assets müssen:
    -   im `.csproj` enthalten sein
    -   als `AvaloniaResource` markiert sein
-   Zugriff erfolgt ausschließlich über `avares://`

## Typische Stolperfallen (Mac / iOS / Avalonia / MAUI)

### 1. Try/Catch greift nicht bei async

-   Fehler entsteht **erst beim await**
-   Ohne await → kein Catch

### 2. Assets nicht im Bundle

-   Dateien im Ordner ≠ Dateien im App Bundle
-   Lösung: `<AvaloniaResource Include="..." />`

### 3. Falsches Assembly im URI

-   `avares://AssemblyName/...` muss exakt stimmen

### 4. AssetLoader.GetAssets Verwirrung

-   Erwartet `baseUri`
-   Besser: `AssetLoader.Open(uri)`

### 5. iOS Crash ohne brauchbare Exception

-   Mono AOT → SIGABRT
-   Ursache oft fehlende Ressource

### 6. Debug vs. Runtime Verhalten

-   Desktop funktioniert, iOS nicht
-   Grund: anderes Ressourcenmodell

### 7. „Datei ist doch da!"

-   Im Repo ja
-   Im Bundle nein

## 🔐 iOS Signing & Deployment

### 8. Fehlende Codesigning-Identität

-   Kein Apple Development Zertifikat im Keychain
-   oder ohne privaten Schlüssel
-   Lösung: Xcode → Accounts → Manage Certificates

### 9. Keine Bereitstellungsprofile gefunden

-   Kein Profil für den exakten Bundle Identifier
-   Lösung: Xcode Run auf echtem Gerät → Profil wird erzeugt

### 10. Bundle Identifier ist entscheidend

-   Product Name egal
-   Bundle Identifier = Identität
-   Muss exakt übereinstimmen

### 11. Xcode vor Rider benutzen

-   Bei Problemen immer zuerst in Xcode testen

### 12. RuntimeIdentifier entscheidet

`ios-arm64` → echtes Gerät\
`iossimulator-*` → Simulator
