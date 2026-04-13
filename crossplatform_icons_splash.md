# Splashscreens und Icons in Cross-Platform-.NET-Projekten

Dieses Dokument fasst praxisnah zusammen, wie man Icons und Splashscreens in verschiedenen .NET-UI-Stacks einbindet. Der Fokus liegt auf einer sinnvollen Asset-Strategie und auf den Unterschieden zwischen WPF, Avalonia, MAUI und mobilen Host-Projekten.

## Grundidee

Nicht jede Plattform will dieselben Formate oder dieselbe Einbindung. Der wichtigste Fehler ist, ein einziges Asset überall identisch verwenden zu wollen.

In der Praxis funktioniert diese Aufteilung gut:

- Ein reduziertes, klares Logo als SVG für skalierbare Quellen
- Ein `.ico` für WPF und klassische Windows-Szenarien
- Ein PNG für Avalonia Desktop
- Plattformnative Ressourcen für mobile Hosts
- Splashscreens immer als zentriertes Motiv denken, nicht als Vollbildgrafik

## Empfehlete Asset-Strategie

Eine einfache und robuste Strategie ist:

- `logo.svg` als Master für Logos und Icons
- `logo.png` in 256x256 für Avalonia Desktop
- `logo.ico` mit mehreren Größen für WPF
- `splash.svg` oder ein splash-optimiertes PNG für MAUI
- native Icon-Sets für iOS und Android, wenn Avalonia mobile Host-Projekte verwendet werden

Das vermeidet unnötige Spezialformate im Alltag und hält die Pflege überschaubar.

## WPF

WPF verwendet klassisch `.ico`.

### Was benötigt wird

Für WPF ist ein `.ico` mit mehreren Größen ideal, zum Beispiel:

- 16x16
- 32x32
- 48x48
- 64x64
- 128x128
- 256x256

### Im Projekt setzen

Im `.csproj`:

```xml
<PropertyGroup>
  <ApplicationIcon>Assets\mvvmnav.ico</ApplicationIcon>
</PropertyGroup>
```

Am Fenster zusätzlich:

```xml
<Window Icon="/Assets/mvvmnav.ico" />
```

Wichtig ist, dass das Fenster-Icon und das Application-Icon zwei verschiedene Ebenen sind.

## Avalonia Desktop

Avalonia Desktop arbeitet plattformneutral.

### Format

PNG mit etwa 256x256.

### Resource

```xml
<ItemGroup>
  <AvaloniaResource Include="Assets\**" />
</ItemGroup>
```

### Nutzung

```xml
<Window Icon="/Assets/mvvmnav.png" />
```

oder

```xml
<Window Icon="avares://Assembly/Assets/mvvmnav.png" />
```

## MAUI

### Icon

```xml
<MauiIcon Include="Resources\AppIcon\appicon.svg" />
```

optional:

```xml
<MauiIcon Include="Resources\AppIcon\appicon.svg"
          ForegroundFile="Resources\AppIcon\appiconfg.svg"
          Color="#1E2A35" />
```

### Splash

```xml
<MauiSplashScreen Include="Resources\Splash\splash.svg"
                  Color="#1E2A35"
                  BaseSize="128,128" />
```

Splash = zentriertes Logo, nicht fullscreen.

## Avalonia Android

- native Android Ressourcen
- XML Drawables / Themes
- ggf. avalonia-anim.xml ersetzen

## Avalonia iOS

- AppIcon.appiconset mit PNGs
- Contents.json nötig

### csproj

```xml
<AppIcon Include="AppIcon" />
```

### Workflow

1. SVG → 1024 PNG
2. Größen erzeugen
3. AppIcon.appiconset befüllen

## Regeln

- SVG als Master
- PNG für Runtime
- ICO für WPF
- Splash = Logo, nicht Bild

## Fazit

Plattformen unterscheiden sich in Einbindung, nicht im Design.
