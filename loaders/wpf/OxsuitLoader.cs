// OXSUIT 1.0 — WPF Loader
// Loads an .oxsuit file into a WPF ResourceDictionary.
//
// Color conversion:
//   OXSUIT uses web-standard #RRGGBB / #RRGGBBAA (alpha last).
//   WPF uses #AARRGGBB (alpha first).
//   This loader handles the conversion transparently.
//
// Resource key mapping:
//   OXSUIT key        → WPF resource key
//   "ContentBg"       → "ContentBgBrush"        (SolidColorBrush)
//   "CornerRadius"    → "OxsuitCornerRadius"     (CornerRadius)
//   "BorderWidth"     → "OxsuitBorderWidth"      (double)
//                     → "OxsuitBorderThickness"  (Thickness)
//   "ShadowDepth"     → "OxsuitShadowDepth"      (double)

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace OXSUIT.Loaders.WPF;

public static class OxsuitLoader
{
    /// <summary>
    /// Loads an .oxsuit file and returns a WPF ResourceDictionary
    /// ready to be merged into Application.Current.Resources or a window's resources.
    /// </summary>
    /// <param name="path">Full path to the .oxsuit file.</param>
    /// <param name="appExtension">
    ///   Optional app name to also load a matching &lt;extensions app="…"&gt; block.
    ///   Pass e.g. "ClaudetRelay" to include app-specific colors.
    /// </param>
    public static ResourceDictionary Load(string path, string? appExtension = null)
    {
        var xml  = XDocument.Load(path);
        var root = xml.Root ?? throw new InvalidOperationException("Invalid OXSUIT file — missing root element.");
        var rd   = new ResourceDictionary();

        // ── Core colors ──────────────────────────────────────────────────────────
        var colorsEl = root.Element("colors");
        if (colorsEl != null)
            foreach (var el in colorsEl.Elements("color"))
                AddColor(rd, el);

        // ── Tokens ───────────────────────────────────────────────────────────────
        var tokensEl = root.Element("tokens");
        if (tokensEl != null)
            foreach (var el in tokensEl.Elements("token"))
                AddToken(rd, el);

        // ── App-specific extensions ───────────────────────────────────────────────
        if (appExtension != null)
        {
            var ext = root.Elements("extensions")
                          .FirstOrDefault(e => string.Equals(
                              e.Attribute("app")?.Value, appExtension,
                              StringComparison.OrdinalIgnoreCase));
            if (ext != null)
                foreach (var el in ext.Elements("color"))
                    AddColor(rd, el);
        }

        return rd;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────────

    private static void AddColor(ResourceDictionary rd, XElement el)
    {
        var key   = el.Attribute("key")?.Value;
        var value = el.Attribute("value")?.Value;
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;

        rd[key + "Brush"] = new SolidColorBrush(ParseWebColor(value));
    }

    private static void AddToken(ResourceDictionary rd, XElement el)
    {
        var key   = el.Attribute("key")?.Value;
        var value = el.Attribute("value")?.Value;
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;

        switch (key)
        {
            case "CornerRadius" when double.TryParse(value, out var r):
                rd["OxsuitCornerRadius"] = new CornerRadius(r);
                break;

            case "BorderWidth" when double.TryParse(value, out var bw):
                rd["OxsuitBorderWidth"]     = bw;
                rd["OxsuitBorderThickness"] = new Thickness(bw);
                break;

            case "ShadowDepth" when double.TryParse(value, out var sd):
                rd["OxsuitShadowDepth"] = sd;
                break;
        }
    }

    /// <summary>
    /// Parses a web-standard hex color string and returns a WPF Color.
    ///
    /// Supported formats:
    ///   #RGB       → expands each channel, fully opaque
    ///   #RRGGBB    → fully opaque
    ///   #RRGGBBAA  → alpha is the LAST two digits (CSS / OXSUIT standard)
    ///
    /// WPF Color.FromArgb expects alpha FIRST — this method reorders accordingly.
    /// </summary>
    private static Color ParseWebColor(string hex)
    {
        hex = hex.TrimStart('#');
        return hex.Length switch
        {
            3 => Color.FromRgb(
                    Convert.ToByte(new string(hex[0], 2), 16),
                    Convert.ToByte(new string(hex[1], 2), 16),
                    Convert.ToByte(new string(hex[2], 2), 16)),

            6 => Color.FromRgb(
                    Convert.ToByte(hex[0..2], 16),
                    Convert.ToByte(hex[2..4], 16),
                    Convert.ToByte(hex[4..6], 16)),

            8 => Color.FromArgb(              // OXSUIT: RRGGBBAA → WPF: AARRGGBB
                    Convert.ToByte(hex[6..8], 16),   // alpha is LAST in web format
                    Convert.ToByte(hex[0..2], 16),
                    Convert.ToByte(hex[2..4], 16),
                    Convert.ToByte(hex[4..6], 16)),

            _ => Colors.Magenta                // fallback — signals a bad value
        };
    }
}
