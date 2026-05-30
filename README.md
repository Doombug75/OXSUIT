# OXSUIT

**Open eXtensible Standard for User Interface Themes**

A lightweight, platform-agnostic XML theme format for GUI applications.  
Define colors and visual geometry once — load everywhere.

---

## What is OXSUIT?

OXSUIT is an open standard for defining the visual appearance of a user interface.  
A single `.oxsuit` file describes:

- **27 semantic color keys** covering every standard UI surface
- **15 optional slot colors** for apps that need extra named surfaces (chat bubbles, panels, categories…)
- **3 visual geometry tokens** — border width, corner radius, shadow depth

No fonts. No sizes. No layout rules. Just **color + shape**.

---

## File format

```xml
<?xml version="1.0" encoding="utf-8"?>
<oxsuit version="1.0" name="My Theme" author="Author" created="2026-01-01">

  <colors>
    <color key="ContentBg" value="#0D1117"/>
    <!-- ... -->
  </colors>

  <tokens>
    <token key="CornerRadius" value="8" unit="px" min="0" max="20"/>
    <!-- ... -->
  </tokens>

  <!-- Optional: app-specific extra colors -->
  <extensions app="MyApp">
    <color key="PrimaryBg" value="#111828"/>
    <!-- ... -->
  </extensions>

</oxsuit>
```

See [`SPEC.md`](SPEC.md) for the full specification.  
See [`examples/`](examples/) for ready-to-use theme files.  
See [`loaders/`](loaders/) for platform loader implementations.

---

## Color format

All colors use **web-standard hex notation**:

| Format | Example | Meaning |
|--------|---------|---------|
| `#RRGGBB` | `#0D1117` | Fully opaque |
| `#RRGGBBAA` | `#18C0B415` | With alpha (AA last — CSS standard) |

Platform loaders convert to their native color model as needed.

---

## Platform loaders

| Platform | Location | Status |
|----------|----------|--------|
| WPF (.NET) | [`loaders/wpf/`](loaders/wpf/) | ✅ v1.0 |

---

## License

MIT — use freely in any project, open or commercial.
