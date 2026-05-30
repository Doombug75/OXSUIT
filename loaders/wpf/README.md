# OXSUIT WPF Loader

**Platform:** WPF (.NET 6 / 7 / 8 / 10+, Windows only)  
**File:** `OxsuitLoader.cs`  
**Namespace:** `OXSUIT.Loaders.WPF`

---

## What it does

`OxsuitLoader` reads an `.oxsuit` theme file and returns a WPF
`ResourceDictionary` populated with `SolidColorBrush` entries and
geometry tokens — ready to be merged into any window or the whole
application with a single line of code.

It handles all format details automatically:

- **Color byte-order** — OXSUIT uses web-standard `#RRGGBBAA` (alpha last);
  WPF uses `#AARRGGBB` (alpha first). The loader reorders silently.
- **Resource key naming** — OXSUIT keys (`ContentBg`, `SidebarText`, …)
  get `Brush` appended to match WPF convention (`ContentBgBrush`,
  `SidebarTextBrush`, …).
- **Geometry tokens** — `CornerRadius`, `BorderWidth`, and `ShadowDepth`
  are converted to the appropriate WPF types and stored with an
  `Oxsuit` prefix (`OxsuitCornerRadius`, `OxsuitBorderThickness`, …).
- **App-specific extensions** — optional `<extensions app="MyApp">`
  blocks are loaded on request and ignored otherwise.

---

## Requirements

- .NET 6 or later (WPF / Windows)
- `System.Xml.Linq` — included in all .NET base class libraries,
  no NuGet package needed
- `PresentationFramework` — standard WPF reference

---

## Installation

Copy `OxsuitLoader.cs` into your project.  
Change the namespace declaration at the top if needed:

```csharp
namespace YourApp.Services;   // or whatever fits your project
```

No other changes are required.

---

## Quick start

### 1. Apply a theme to the whole application at startup

The most common use case: load a theme once in `App.xaml.cs` and
every window picks it up automatically through the application's
merged resource dictionaries.

```csharp
// App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    var themePath = @"C:\Themes\MyTheme.oxsuit";
    var dict = OxsuitLoader.Load(themePath);
    Application.Current.Resources.MergedDictionaries.Add(dict);
}
```

After this, any WPF control that uses a `DynamicResource` or
`StaticResource` with a key like `ContentBgBrush` will resolve
correctly across the entire application.

---

### 2. Apply a theme to a single window only

```csharp
// In your window's constructor, before InitializeComponent()
public MainWindow()
{
    var dict = OxsuitLoader.Load(@"C:\Themes\MyTheme.oxsuit");
    Resources.MergedDictionaries.Add(dict);

    InitializeComponent();
}
```

> **Important:** Add to `MergedDictionaries` **before** calling
> `InitializeComponent()`. WPF resolves `StaticResource` bindings
> during XAML parsing — if the dictionary is not present yet, those
> bindings will fall back to default (usually transparent/invisible).
> `DynamicResource` bindings are resolved at render time so order
> matters less, but adding the dictionary before init is always safest.

---

### 3. Switch themes at runtime

```csharp
private void SwitchTheme(string newThemePath)
{
    var dict = OxsuitLoader.Load(newThemePath);
    Application.Current.Resources.MergedDictionaries.Clear();
    Application.Current.Resources.MergedDictionaries.Add(dict);
}
```

> **Note:** Only controls bound with `DynamicResource` will update
> live when the theme changes. `StaticResource` bindings are resolved
> once at load time and do not react to dictionary changes.

---

## Using tokens in XAML

When a theme includes a `<tokens>` block, the loader creates
ready-to-use WPF typed resources:

| OXSUIT token | WPF resource key | Type |
|---|---|---|
| `CornerRadius` | `OxsuitCornerRadius` | `CornerRadius` |
| `BorderWidth` | `OxsuitBorderWidth` | `double` |
| `BorderWidth` | `OxsuitBorderThickness` | `Thickness` |
| `ShadowDepth` | `OxsuitShadowDepth` | `double` |

Example XAML usage:

```xml
<!-- Border that uses the theme's corner rounding -->
<Border CornerRadius="{DynamicResource OxsuitCornerRadius}"
        BorderThickness="{DynamicResource OxsuitBorderThickness}"
        BorderBrush="{DynamicResource ContentBorderBrush}"
        Background="{DynamicResource ContentBgBrush}">
    <TextBlock Foreground="{DynamicResource ContentTextBrush}"
               Text="Themed content"/>
</Border>
```

```xml
<!-- Button using accent and shape tokens -->
<Button Background="{DynamicResource AccentBgBrush}"
        Foreground="{DynamicResource AccentTextBrush}">
    <Button.Template>
        <ControlTemplate TargetType="Button">
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{DynamicResource OxsuitCornerRadius}"
                    BorderThickness="{DynamicResource OxsuitBorderThickness}"
                    BorderBrush="{DynamicResource ControlBorderBrush}"
                    Padding="12,6">
                <ContentPresenter HorizontalAlignment="Center"
                                  VerticalAlignment="Center"/>
            </Border>
        </ControlTemplate>
    </Button.Template>
    Click me
</Button>
```

---

## App-specific extension blocks

If your `.oxsuit` file contains an `<extensions app="YourApp">` block,
pass your app's name as the second argument to `Load()`:

```csharp
var dict = OxsuitLoader.Load(themePath, appExtension: "YourApp");
```

Extension colors follow the same naming rule: key + `Brush`.

```xml
<!-- in the .oxsuit file -->
<extensions app="YourApp">
  <color key="WelcomeScreenBg" value="#1A1A2E"/>
  <color key="StatusBarText"   value="#E0E0E0"/>
</extensions>
```

```csharp
// These will be available after Load():
var bg   = (SolidColorBrush)dict["WelcomeScreenBgBrush"];
var text = (SolidColorBrush)dict["StatusBarTextBrush"];
```

If no matching extension block exists in the file, `Load()` silently
continues with only the core colors and tokens.

---

## Using brushes in code-behind

The returned dictionary works like any `ResourceDictionary`:

```csharp
var dict = OxsuitLoader.Load(themePath);

// Cast and use directly
var bg   = (SolidColorBrush)dict["ContentBgBrush"];
var text = (SolidColorBrush)dict["ContentTextBrush"];

myPanel.Background = bg;
myLabel.Foreground = text;
```

Or after merging into application resources, use `FindResource`:

```csharp
var accent = (SolidColorBrush)Application.Current.FindResource("AccentBgBrush");
```

---

## Full resource key reference

All color keys are the OXSUIT key name with `Brush` appended.

### Core surfaces (always present)

| OXSUIT key | WPF resource key | Purpose |
|---|---|---|
| `ContentBg` | `ContentBgBrush` | Main content area background |
| `ContentBorder` | `ContentBorderBrush` | Dividers in the content area |
| `ContentText` | `ContentTextBrush` | Normal body text |
| `ContentHigh` | `ContentHighBrush` | Highlighted icons / accents |
| `ContentDim` | `ContentDimBrush` | Subdued / secondary text |
| `SidebarBg` | `SidebarBgBrush` | Sidebar / nav panel background |
| `SidebarBorder` | `SidebarBorderBrush` | Sidebar dividers |
| `SidebarText` | `SidebarTextBrush` | Sidebar label text |
| `SidebarHigh` | `SidebarHighBrush` | Active / highlighted sidebar items |
| `SidebarDim` | `SidebarDimBrush` | Inactive sidebar items |
| `ControlBg` | `ControlBgBrush` | Button / card background |
| `ControlBorder` | `ControlBorderBrush` | Button / card border |
| `ControlText` | `ControlTextBrush` | Text on buttons and cards |
| `ControlHigh` | `ControlHighBrush` | Icons / symbols on controls |
| `ControlDim` | `ControlDimBrush` | Subdued text on controls |
| `ControlHover` | `ControlHoverBrush` | Control background on hover |
| `InputBg` | `InputBgBrush` | Text field background |
| `InputBorder` | `InputBorderBrush` | Text field border |
| `InputText` | `InputTextBrush` | Text typed by the user |
| `InputHigh` | `InputHighBrush` | Symbols / icons inside inputs |
| `InputDim` | `InputDimBrush` | Placeholder / hint text |
| `AccentBg` | `AccentBgBrush` | Primary action button background |
| `AccentText` | `AccentTextBrush` | Text on accent buttons |
| `AccentHighlight` | `AccentHighlightBrush` | Hover / glow variant of accent |
| `PrimaryAccent` | `PrimaryAccentBrush` | Brand accent slot 1 |
| `SecondaryAccent` | `SecondaryAccentBrush` | Brand accent slot 2 |
| `TertiaryAccent` | `TertiaryAccentBrush` | Brand accent slot 3 |

### Optional surface slots (present when defined in the file)

| OXSUIT key | WPF resource key |
|---|---|
| `PrimaryBg` | `PrimaryBgBrush` |
| `PrimaryBorder` | `PrimaryBorderBrush` |
| `PrimaryText` | `PrimaryTextBrush` |
| `PrimaryHigh` | `PrimaryHighBrush` |
| `PrimaryDim` | `PrimaryDimBrush` |
| `SecondaryBg` | `SecondaryBgBrush` |
| `SecondaryBorder` | `SecondaryBorderBrush` |
| `SecondaryText` | `SecondaryTextBrush` |
| `SecondaryHigh` | `SecondaryHighBrush` |
| `SecondaryDim` | `SecondaryDimBrush` |
| `TertiaryBg` | `TertiaryBgBrush` |
| `TertiaryBorder` | `TertiaryBorderBrush` |
| `TertiaryText` | `TertiaryTextBrush` |
| `TertiaryHigh` | `TertiaryHighBrush` |
| `TertiaryDim` | `TertiaryDimBrush` |

### Geometry tokens (present when defined in the file)

| OXSUIT token | WPF resource key | WPF type |
|---|---|---|
| `CornerRadius` | `OxsuitCornerRadius` | `CornerRadius` |
| `BorderWidth` | `OxsuitBorderWidth` | `double` |
| `BorderWidth` | `OxsuitBorderThickness` | `Thickness` |
| `ShadowDepth` | `OxsuitShadowDepth` | `double` |

---

## Error handling

`OxsuitLoader.Load()` throws on file-not-found, malformed XML, or a
missing root element. Wrap calls in a try/catch at the call site:

```csharp
ResourceDictionary dict;
try
{
    dict = OxsuitLoader.Load(themePath);
}
catch (Exception ex)
{
    MessageBox.Show($"Could not load theme:\n{ex.Message}",
                    "Theme Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    return; // keep whatever theme is currently active
}

Application.Current.Resources.MergedDictionaries.Clear();
Application.Current.Resources.MergedDictionaries.Add(dict);
```

---

## Legacy app naming: mapping OXSUIT keys to custom WPF keys

Some applications used their own resource key names before adopting
OXSUIT — for example, using `PrimaryBubbleBrush` instead of
`PrimaryBgBrush` for chat bubble backgrounds.

In that case, do not use the canonical loader directly on those keys.
Instead, create a thin wrapper that renames the entries after loading:

```csharp
// Example: app uses "PrimaryBubbleBrush" instead of "PrimaryBgBrush"
private static ResourceDictionary LoadWithLegacyMapping(string path)
{
    var dict = OxsuitLoader.Load(path);

    // Rename OXSUIT canonical keys → legacy app keys
    var legacyMappings = new Dictionary<string, string>
    {
        ["PrimaryBgBrush"]        = "PrimaryBubbleBrush",
        ["PrimaryBorderBrush"]    = "PrimaryBubbleBorderBrush",
        ["SecondaryBgBrush"]      = "SecondaryBubbleBrush",
        ["SecondaryBorderBrush"]  = "SecondaryBubbleBorderBrush",
        ["TertiaryBgBrush"]       = "TertiaryBubbleBrush",
        ["TertiaryBorderBrush"]   = "TertiaryBubbleBorderBrush",
    };

    foreach (var (oxsuitKey, legacyKey) in legacyMappings)
    {
        if (dict.Contains(oxsuitKey))
        {
            dict[legacyKey] = dict[oxsuitKey];
            dict.Remove(oxsuitKey);
        }
    }

    return dict;
}
```

> New applications should use the canonical OXSUIT key names
> (`PrimaryBgBrush`, etc.) from the start to stay compatible without
> any mapping layer.

---

## Title bar theming (Windows 11)

OXSUIT does not dictate how the OS title bar is styled — that is a
platform concern. In WPF on Windows 11, use the DWM API to match the
title bar to the loaded theme:

```csharp
using System.Runtime.InteropServices;
using System.Windows.Interop;

[DllImport("dwmapi.dll")]
private static extern int DwmSetWindowAttribute(
    IntPtr hwnd, int attr, ref int value, int size);

private void ApplyTitleBarFromTheme(ResourceDictionary dict)
{
    if (PresentationSource.FromVisual(this) is not HwndSource src) return;
    var hwnd = src.Handle;

    var bgBrush   = dict["SidebarBgBrush"]   as SolidColorBrush;
    var textBrush = dict["SidebarTextBrush"] as SolidColorBrush;
    if (bgBrush is null || textBrush is null) return;

    int ToColorRef(Color c) => c.R | (c.G << 8) | (c.B << 16);

    int dark    = bgBrush.Color.R < 128 ? 1 : 0;   // simple dark-mode heuristic
    int caption = ToColorRef(bgBrush.Color);
    int text    = ToColorRef(textBrush.Color);

    DwmSetWindowAttribute(hwnd, 20, ref dark,    sizeof(int)); // DWMWA_USE_IMMERSIVE_DARK_MODE
    DwmSetWindowAttribute(hwnd, 35, ref caption, sizeof(int)); // DWMWA_CAPTION_COLOR
    DwmSetWindowAttribute(hwnd, 36, ref text,    sizeof(int)); // DWMWA_TEXT_COLOR
}
```

Call `ApplyTitleBarFromTheme` in the window's `SourceInitialized`
event (the HWND does not exist before that point):

```csharp
SourceInitialized += (_, _) => ApplyTitleBarFromTheme(dict);
```

---

## Worked example: full themed window

```csharp
public partial class MainWindow : Window
{
    private ResourceDictionary _theme = new();

    public MainWindow()
    {
        LoadTheme(@"Themes\ClaudesChoice.oxsuit");
        InitializeComponent();
        SourceInitialized += (_, _) => ApplyTitleBarFromTheme(_theme);
    }

    private void LoadTheme(string path)
    {
        try
        {
            _theme = OxsuitLoader.Load(path, appExtension: "MyApp");
        }
        catch
        {
            _theme = new ResourceDictionary();  // empty = WPF defaults
        }
        Resources.MergedDictionaries.Clear();
        Resources.MergedDictionaries.Add(_theme);
    }

    private void ThemePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is string newPath)
            LoadTheme(newPath);
    }
}
```

```xml
<!-- MainWindow.xaml — controls bind to theme resources -->
<Grid Background="{DynamicResource ContentBgBrush}">
    <Border Background="{DynamicResource SidebarBgBrush}"
            Width="200" HorizontalAlignment="Left"/>

    <TextBlock Text="Hello OXSUIT"
               Foreground="{DynamicResource ContentTextBrush}"
               Margin="220,20,0,0"/>

    <Button Background="{DynamicResource AccentBgBrush}"
            Foreground="{DynamicResource AccentTextBrush}"
            Content="Click"
            Margin="220,60,0,0" Width="80"/>
</Grid>
```

---

## See also

- [`../../SPEC.md`](../../SPEC.md) — Full OXSUIT format specification
- [`../../examples/`](../../examples/) — Ready-to-use theme files
- [`../../README.md`](../../README.md) — Project overview
