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

Am Fenster zusätzlich:

<Window Icon="/Assets/mvvmnav.ico" />

Wichtig ist, dass das Fenster-Icon und das Application-Icon zwei verschiedene Ebenen sind. Das ApplicationIcon betrifft eher EXE, Explorer und Taskbar. Das Window.Icon betrifft das tatsächliche Fenster oben links.

Avalonia Desktop

Avalonia Desktop arbeitet angenehm plattformneutral, aber nicht mit denselben Regeln wie WPF.

Format

Für Avalonia Desktop ist ein PNG meist die pragmatischste Wahl. Eine Größe von etwa 256x256 ist ein guter Standard.

Resource-Einbindung

Die Datei muss als AvaloniaResource eingebunden sein.

Zum Beispiel im .csproj:

<ItemGroup>
  <AvaloniaResource Include="Assets\**" />
</ItemGroup>
Verwendung im Fenster

Wenn die Datei im selben Projekt liegt:

<Window Icon="/Assets/mvvmnav.png" />

Wenn die Datei in einer anderen Assembly liegt:

<Window Icon="avares://My.Assembly/Assets/mvvmnav.png" />
Wichtige Erkenntnis

Bei Avalonia ist avares:// der robuste Weg für assemblyübergreifende Ressourcen. Wenn Asset und Window im selben Projekt liegen, reicht der kürzere Pfad oft aus.

MAUI

MAUI hat den großen Vorteil, dass Icons und Splashscreens zentral im MAUI-Projekt definiert werden.

App-Icon in MAUI
Quelle

Am besten als SVG.

Projektdatei
<ItemGroup>
  <MauiIcon Include="Resources\AppIcon\appicon.svg" />
</ItemGroup>

Für Android Adaptive Icons kann zusätzlich ein Vordergrund-Asset verwendet werden:

<ItemGroup>
  <MauiIcon Include="Resources\AppIcon\appicon.svg"
            ForegroundFile="Resources\AppIcon\appiconfg.svg"
            Color="#1E2A35" />
</ItemGroup>
Warum zwei Dateien

Android trennt bei Adaptive Icons häufig zwischen Hintergrund und Vordergrund. Das eigentliche Logo liegt dann im Vordergrund, der Hintergrund kommt über Farbe oder eine zweite Grafik.

Splashscreen in MAUI
Quelle

SVG ist am robustesten. PNG geht ebenfalls, ist aber weniger flexibel.

Projektdatei
<ItemGroup>
  <MauiSplashScreen Include="Resources\Splash\splash.svg"
                    Color="#1E2A35"
                    BaseSize="128,128" />
</ItemGroup>
Wichtige Design-Regel

Ein MAUI-Splashscreen sollte wie ein zentriertes Logo gedacht werden, nicht wie eine randfüllende Illustration. Das gilt besonders für Android und iOS. Das Motiv braucht Luft außen herum.

Ein gutes Splash-Asset hat also:

klares Zentrum
transparenten Hintergrund
etwas Padding
keine wichtigen Elemente direkt am Rand
Avalonia Mobile

Bei Avalonia für Android und iOS läuft die Einbindung nicht zentral wie bei MAUI, sondern plattformnativ in den jeweiligen Host-Projekten.

Avalonia Android
Icon

Das Launcher-Icon wird über die Android-Ressourcen eingebunden. Typischerweise liegt es im Android-Projekt unter Resources.

Splashscreen

Auch der Splashscreen wird über Android-Ressourcen und Themes gesteuert. Wenn noch das Avalonia-A auftaucht, liegt das meist an vordefinierten Android-XML-Ressourcen im Host-Projekt.

Ein häufiger Fall ist, dass eine Avalonia-spezifische XML-Datei noch referenziert wird und dadurch das Standardlogo angezeigt wird. Dann muss nicht nur ein Bild ersetzt, sondern auch die referenzierende XML oder das Theme angepasst werden.

Wichtig ist: Diese XML-Dateien sind Android-Ressourcen, nicht SVG.

Avalonia iOS
App-Icon

iOS erwartet kein SVG als direktes App-Icon, sondern ein AppIcon.appiconset mit mehreren PNG-Dateien und einer Contents.json.

Typischer Ablauf
Aus dem SVG ein 1024x1024 PNG erzeugen
Daraus die benötigten iOS-Größen generieren
Im iOS-Projekt einen Ordner Assets.xcassets/AppIcon.appiconset anlegen
Dort alle PNGs und die Contents.json hineinkopieren
Im .csproj das Set aktivieren
Im .csproj
<ItemGroup>
  <AppIcon Include="AppIcon" />
</ItemGroup>
Wichtige iOS-Regel

Für das Ausgangs-PNG ist die Pixeldichte egal. Relevant ist nur die Pixelgröße. 1024x1024 ist die entscheidende Mastergröße. Das Motiv sollte nicht komplett randfüllend sein. Ein wenig Luft außen ist sinnvoll, weil iOS das App-Icon maskiert.

Design-Regeln, die fast immer helfen
Für Icons

Icons müssen reduzierter sein als Splashscreens. Was auf einer Webseite gut aussieht, ist als Taskbar-Icon oft zu detailreich.

Ein gutes Icon hat:

klare Geometrie
wenige Formen
hohe Erkennbarkeit
keine filigranen Details
Für Splashscreens

Splashscreens sollten wie zentrierte Logos wirken.

Ein gutes Splash-Motiv hat:

Fokus in der Mitte
genügend Padding
keinen Vollbildcharakter
gute Lesbarkeit auf hellem oder dunklem Hintergrund
Typische Fehler
Ein einziges Format überall verwenden wollen

Das spart am Anfang Zeit, rächt sich aber später. WPF will etwas anderes als Avalonia oder iOS.

Zu detailreiche Logos

Was als großes Bild schön ist, wird bei 16x16 unlesbar.

Falsche Build Actions

Gerade bei Avalonia ist die korrekte Resource-Einbindung entscheidend. Sonst werden Dateien zur Laufzeit nicht gefunden.

Relative Pfade und Case-Sensitivity

Unter Linux und macOS ist Groß-/Kleinschreibung relevant. Gerade bei Projektpfaden und Ressourcen sollte man sehr konsequent sein.

Praktische Minimalstrategie für ein neues Projekt

Wenn man schnell, sauber und ohne Overengineering starten will, reicht oft diese Strategie:

logo.svg als Master
logo.ico für WPF
logo.png 256x256 für Avalonia Desktop
appicon.svg für MAUI
splash.svg für MAUI
AppIcon.appiconset für iOS-Host-Projekte
Android-Ressourcen für Avalonia Android nativ anpassen
Fazit

Cross-Platform-Assets in .NET sind kein Hexenwerk, aber man muss akzeptieren, dass die Plattformen unterschiedliche Erwartungen haben.

Die wichtigste didaktische Kernbotschaft ist:

Nicht das Bild ist das Problem, sondern die Einbindungslogik der jeweiligen Plattform.

Wer das einmal sauber trennt in:

Logoquelle
Desktop-Icon
Splash-Motiv
plattformspezifische Einbindung

kommt in späteren Projekten deutlich schneller ans Ziel.