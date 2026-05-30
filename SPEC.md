# OXSUIT 1.0 — Specification

**Open eXtensible Standard for User Interface Themes**  
Version 1.0 · May 2026

---

## Overview

An OXSUIT file is a UTF-8 encoded XML document with the `.oxsuit` extension.  
It defines the complete visual identity of a GUI application through semantic color keys and geometry tokens.

---

## Root element

```xml
<oxsuit version="1.0"
        name="Theme Name"
        author="Author Name"
        created="YYYY-MM-DD"
        description="Optional description">
```

| Attribute | Required | Description |
|-----------|----------|-------------|
| `version` | ✅ | OXSUIT spec version. Must be `"1.0"`. |
| `name` | ✅ | Human-readable theme name. |
| `author` | ✗ | Theme author name. |
| `created` | ✗ | Creation date in `YYYY-MM-DD` format. |
| `description` | ✗ | Short description of the theme. |

---

## Color format

All color values use **web-standard hex notation**:

- `#RRGGBB` — fully opaque color
- `#RRGGBBAA` — color with alpha, **alpha is the last two digits** (CSS standard)

```xml
<color key="ContentBg"     value="#0D1117"/>       <!-- fully opaque -->
<color key="ContentBorder" value="#18C0B415"/>     <!-- 8% opacity -->
```

Platform loaders are responsible for converting to their native color model  
(e.g. WPF uses `#AARRGGBB` — the loader reorders the alpha byte).

---

## `<colors>` — Core color surfaces

Twenty-seven required semantic color keys covering every standard UI surface.  
Keys use **PascalCase** without any platform-specific suffix.  
Loaders append their own suffix as needed (e.g. WPF appends `Brush`).

### Content surface
The main reading and working area of the application.

| Key | Purpose |
|-----|---------|
| `ContentBg` | Background of the content area |
| `ContentBorder` | Border / divider color in the content area |
| `ContentText` | Normal body text |
| `ContentHigh` | Highlighted symbols, icons, and accents |
| `ContentDim` | Subdued / secondary text |

### Sidebar surface
Navigation area, sidebar, or any secondary panel.

| Key | Purpose |
|-----|---------|
| `SidebarBg` | Background of the sidebar |
| `SidebarBorder` | Border / divider color in the sidebar |
| `SidebarText` | Normal text in the sidebar |
| `SidebarHigh` | Highlighted symbols and active items |
| `SidebarDim` | Subdued / inactive items |

### Control surface
Buttons, cards, panels, and interactive containers.

| Key | Purpose |
|-----|---------|
| `ControlBg` | Default background of controls and cards |
| `ControlBorder` | Border / outline of controls |
| `ControlText` | Normal text on controls |
| `ControlHigh` | Highlighted symbols on controls |
| `ControlDim` | Subdued text on controls |
| `ControlHover` | Background on hover / focus state |

### Input surface
Text fields, search boxes, and editable areas.

| Key | Purpose |
|-----|---------|
| `InputBg` | Background of input fields |
| `InputBorder` | Border of input fields |
| `InputText` | Text typed by the user |
| `InputHigh` | Highlighted symbols inside inputs |
| `InputDim` | Placeholder / hint text |

### Accent colors
The primary brand and action colors.

| Key | Purpose |
|-----|---------|
| `AccentBg` | Primary accent / action button background |
| `AccentText` | Text on accent-colored buttons |
| `AccentHighlight` | Lighter accent variant for hover / glow |
| `PrimaryAccent` | Primary brand accent (palette slot 1) |
| `SecondaryAccent` | Secondary accent (palette slot 2) |
| `TertiaryAccent` | Tertiary accent (palette slot 3) |

---

## `<tokens>` — Visual geometry

Three optional tokens that define the *feel* of the theme — its geometry and depth.  
Font choices, sizes, spacing, and layout are deliberately excluded; those are application concerns.

```xml
<tokens>
  <token key="BorderWidth"  value="1" unit="px" min="0" max="10"/>
  <token key="CornerRadius" value="8" unit="px" min="0" max="20"/>
  <token key="ShadowDepth"  value="1"           min="0" max="3"/>
</tokens>
```

| Key | Unit | Range | Description |
|-----|------|-------|-------------|
| `BorderWidth` | px | 0–10 | Stroke width of borders and outlines. `0` = flat/borderless. |
| `CornerRadius` | px | 0–20 | Rounding of corners. `0` = sharp, `20` = strongly rounded. |
| `ShadowDepth` | — | 0–3 | Elevation feel. `0` = flat, `1` = subtle, `2` = raised, `3` = floating. |

The `min` and `max` attributes are hints for editor tools (e.g. slider range in Theminator).  
Loaders may clamp values to these ranges.

---

## Optional surface slots

Three optional named surface groups for apps that need extra color surfaces beyond the core five.  
Applications map these to their own UI concepts — a chat app might use them for message bubbles,  
a code editor for panel regions, a dashboard for widget backgrounds.

Each slot has the same five keys as a core surface: `Bg`, `Border`, `Text`, `High`, `Dim`.

```xml
<color key="PrimaryBg"       value="#111828"/>
<color key="PrimaryBorder"   value="#18C0B420"/>
<color key="PrimaryText"     value="#E6EDF3"/>
<color key="PrimaryHigh"     value="#2DD8CE"/>
<color key="PrimaryDim"      value="#6E8094"/>

<color key="SecondaryBg"     value="#0E1420"/>
<color key="SecondaryBorder" value="#18C0B420"/>
<color key="SecondaryText"   value="#A8B8CC"/>
<color key="SecondaryHigh"   value="#2DD8CE"/>
<color key="SecondaryDim"    value="#6E8094"/>

<color key="TertiaryBg"      value="#151C30"/>
<color key="TertiaryBorder"  value="#18C0B415"/>
<color key="TertiaryText"    value="#687888"/>
<color key="TertiaryHigh"    value="#2DD8CE"/>
<color key="TertiaryDim"     value="#6E8094"/>
```

These keys live inside the main `<colors>` block and are simply omitted if not needed.

---

## `<extensions>` — App-specific colors

For colors that only make sense in a specific application.  
Loaders silently ignore extension blocks they do not recognise.

```xml
<extensions app="MyApp">
  <color key="MySpecialColor" value="#FF6B35"/>
</extensions>
```

---

## Full example skeleton

```xml
<?xml version="1.0" encoding="utf-8"?>
<oxsuit version="1.0" name="My Theme" author="Author" created="2026-01-01">

  <colors>
    <!-- Content surface -->
    <color key="ContentBg"       value="#0D1117"/>
    <color key="ContentBorder"   value="#18C0B415"/>
    <color key="ContentText"     value="#E6EDF3"/>
    <color key="ContentHigh"     value="#2DD8CE"/>
    <color key="ContentDim"      value="#6E8094"/>

    <!-- Sidebar surface -->
    <color key="SidebarBg"       value="#161B22"/>
    <color key="SidebarBorder"   value="#18C0B425"/>
    <color key="SidebarText"     value="#CDD6E0"/>
    <color key="SidebarHigh"     value="#E8A840"/>
    <color key="SidebarDim"      value="#4D6278"/>

    <!-- Control surface -->
    <color key="ControlBg"       value="#1C2333"/>
    <color key="ControlBorder"   value="#18C0B435"/>
    <color key="ControlText"     value="#8899B8"/>
    <color key="ControlHigh"     value="#B060E8"/>
    <color key="ControlDim"      value="#4A5870"/>
    <color key="ControlHover"    value="#243044"/>

    <!-- Input surface -->
    <color key="InputBg"         value="#0A0F18"/>
    <color key="InputBorder"     value="#18C0B450"/>
    <color key="InputText"       value="#D8E4F0"/>
    <color key="InputHigh"       value="#40C8E0"/>
    <color key="InputDim"        value="#506080"/>

    <!-- Accent -->
    <color key="AccentBg"        value="#18C0B4"/>
    <color key="AccentText"      value="#060C10"/>
    <color key="AccentHighlight" value="#30E0D4"/>
    <color key="PrimaryAccent"   value="#18C0B4"/>
    <color key="SecondaryAccent" value="#E8A840"/>
    <color key="TertiaryAccent"  value="#B060E8"/>

    <!-- Optional surface slots -->
    <color key="PrimaryBg"       value="#111828"/>
    <color key="PrimaryBorder"   value="#18C0B420"/>
    <color key="PrimaryText"     value="#E6EDF3"/>
    <color key="PrimaryHigh"     value="#2DD8CE"/>
    <color key="PrimaryDim"      value="#6E8094"/>

    <color key="SecondaryBg"     value="#0E1420"/>
    <color key="SecondaryBorder" value="#18C0B420"/>
    <color key="SecondaryText"   value="#A8B8CC"/>
    <color key="SecondaryHigh"   value="#2DD8CE"/>
    <color key="SecondaryDim"    value="#6E8094"/>

    <color key="TertiaryBg"      value="#151C30"/>
    <color key="TertiaryBorder"  value="#18C0B415"/>
    <color key="TertiaryText"    value="#687888"/>
    <color key="TertiaryHigh"    value="#2DD8CE"/>
    <color key="TertiaryDim"     value="#6E8094"/>
  </colors>

  <tokens>
    <token key="BorderWidth"  value="1" unit="px" min="0" max="10"/>
    <token key="CornerRadius" value="8" unit="px" min="0" max="20"/>
    <token key="ShadowDepth"  value="1"           min="0" max="3"/>
  </tokens>

</oxsuit>
```

---

## Summary

| Section | Keys | Required |
|---------|------|----------|
| Core colors (5 surfaces) | 27 | ✅ |
| Optional surface slots | 15 | ✗ |
| Tokens | 3 | ✗ |
| Extensions | unlimited | ✗ |
