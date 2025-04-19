using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform;
using Avalonia.Utilities;
namespace CreationEditor.Avalonia.Services.Avalonia.Font;

// This is a copy of EmbeddedFontCollection https://github.com/AvaloniaUI/Avalonia/blob/b16975cb0402cd10358f17135d25c3dd3aac7ac6/src/Avalonia.Base/Media/Fonts/EmbeddedFontCollection.cs#L10
// The only changes were made in the Initialize method to load ttf files from a directory instead of an avalonia resource and in the AddGlyphTypeface method to simplify the logic
public class FileSystemFontCollection(Uri collectionKey, Uri sourceDirectory) : FontCollectionBase {
    private readonly List<FontFamily> _fontFamilies = new(1);

    public override Uri Key => collectionKey;

    public override int Count => _fontFamilies.Count;
    public override FontFamily this[int index] => _fontFamilies[index];

    public override void Initialize(IFontManagerImpl fontManager) {
        foreach (var loadFontAsset in Directory.EnumerateFiles(sourceDirectory.AbsolutePath, "*.ttf")) {
            var stream = File.OpenRead(loadFontAsset);
            var methodInfo = fontManager.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.Name.Contains("TryCreateGlyphTypeface")).Skip(1).First();
            object?[] parameters = [stream, FontSimulations.None, null];
            methodInfo.Invoke(fontManager, parameters);
            if (parameters[2] is IGlyphTypeface glyphTypeface) AddGlyphTypeface(glyphTypeface);
        }
    }

    private static Typeface GetImplicitTypeface(Typeface typeface, out string normalizedFamilyName) {
        normalizedFamilyName = typeface.FontFamily.FamilyNames.PrimaryFamilyName;
        if (!normalizedFamilyName.Contains(' '))
            return typeface;

        var style1 = typeface.Style;
        var weight1 = typeface.Weight;
        var stretch1 = typeface.Stretch;
        if (TryGetStyle(ref normalizedFamilyName, out var style2))
            style1 = style2;
        if (TryGetWeight(ref normalizedFamilyName, out var weight2))
            weight1 = weight2;
        if (TryGetStretch(ref normalizedFamilyName, out var stretch2))
            stretch1 = stretch2;
        return new Typeface(typeface.FontFamily, style1, weight1, stretch1);
    }

    private static bool TryGetWeight(ref string familyName, out FontWeight weight) {
        weight = FontWeight.Normal;
        var stringTokenizer = new StringTokenizer(familyName, ' ');
        stringTokenizer.ReadString();
        while (stringTokenizer.TryReadString(out var result)) {
            if (!new StringTokenizer(result).TryReadInt32(out var _) && Enum.TryParse(result, true, out weight)) {
                familyName = familyName.Replace(" " + result, "").TrimEnd();
                return true;
            }
        }
        return false;
    }

    private static bool TryGetStyle(ref string familyName, out FontStyle style) {
        style = FontStyle.Normal;
        var stringTokenizer = new StringTokenizer(familyName, ' ');
        stringTokenizer.ReadString();
        while (stringTokenizer.TryReadString(out var result)) {
            if (!new StringTokenizer(result).TryReadInt32(out var _) && Enum.TryParse(result, true, out style)) {
                familyName = familyName.Replace(" " + result, "").TrimEnd();
                return true;
            }
        }
        return false;
    }

    private static bool TryGetStretch(ref string familyName, out FontStretch stretch) {
        stretch = FontStretch.Normal;
        var stringTokenizer = new StringTokenizer(familyName, ' ');
        stringTokenizer.ReadString();
        while (stringTokenizer.TryReadString(out var result)) {
            if (!new StringTokenizer(result).TryReadInt32(out var _) && Enum.TryParse(result, true, out stretch)) {
                familyName = familyName.Replace(" " + result, "").TrimEnd();
                return true;
            }
        }
        return false;
    }

    private static bool TryFindStretchFallback(
        ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces,
        FontCollectionKey key,
        [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface) {
        glyphTypeface = null;
        var stretch = (int) key.Stretch;
        if (stretch < 5) {
            for (var index = 0; stretch + index < 9; ++index) {
                if (glyphTypefaces.TryGetValue(key with {
                        Stretch = (FontStretch) (stretch + index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
        } else {
            for (var index = 0; stretch - index > 1; ++index) {
                if (glyphTypefaces.TryGetValue(key with {
                        Stretch = (FontStretch) (stretch - index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
        }
        return false;
    }

    private static bool TryFindWeightFallback(
        ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces,
        FontCollectionKey key,
        [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface) {
        glyphTypeface = null;
        var weight = (int) key.Weight;
        if (weight is >= 400 and <= 500) {
            for (var index = 0; weight + index <= 500; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight + index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
            for (var index = 0; weight - index >= 100; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight - index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
            for (var index = 0; weight + index <= 900; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight + index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
        }
        if (weight < 400) {
            for (var index = 0; weight - index >= 100; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight - index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
            for (var index = 0; weight + index <= 900; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight + index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
        }
        if (weight > 500) {
            for (var index = 0; weight + index <= 900; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight + index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
            for (var index = 0; weight - index >= 100; index += 50) {
                if (glyphTypefaces.TryGetValue(key with {
                        Weight = (FontWeight) (weight - index)
                    },
                    out glyphTypeface) && glyphTypeface != null)
                    return true;
            }
        }
        return false;
    }

    private static bool TryGetNearestMatch(
        ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?> glyphTypefaces,
        FontCollectionKey key,
        [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface) {
        if (glyphTypefaces.TryGetValue(key, out glyphTypeface) && glyphTypeface != null)
            return true;

        if (key.Style != FontStyle.Normal)
            key = key with { Style = FontStyle.Normal };
        if (key.Stretch != FontStretch.Normal) {
            if (TryFindStretchFallback(glyphTypefaces, key, out glyphTypeface))
                return true;

            if (key.Weight != FontWeight.Normal) {
                if (TryFindStretchFallback(glyphTypefaces,
                    key with {
                        Weight = FontWeight.Normal
                    },
                    out glyphTypeface))
                    return true;
            }
            key = key with { Stretch = FontStretch.Normal };
        }
        if (TryFindWeightFallback(glyphTypefaces, key, out glyphTypeface) || TryFindStretchFallback(glyphTypefaces, key, out glyphTypeface))
            return true;

        foreach (var typeface in glyphTypefaces.Values) {
            if (typeface != null) {
                glyphTypeface = typeface;
                return true;
            }
        }
        return false;
    }

    public override bool TryGetGlyphTypeface(
        string familyName,
        FontStyle style,
        FontWeight weight,
        FontStretch stretch,
        [NotNullWhen(true)] out IGlyphTypeface? glyphTypeface) {
        var implicitTypeface = GetImplicitTypeface(new Typeface(familyName, style, weight, stretch), out familyName);
        style = implicitTypeface.Style;
        weight = implicitTypeface.Weight;
        stretch = implicitTypeface.Stretch;
        var key = new FontCollectionKey(style, weight, stretch);
        if (_glyphTypefaceCache.TryGetValue(familyName, out var glyphTypefaces)) {
            if (glyphTypefaces.TryGetValue(key, out glyphTypeface) && glyphTypeface != null)
                return true;

            if (TryGetNearestMatch(glyphTypefaces, key, out glyphTypeface)) {
                glyphTypefaces.TryAdd(key, glyphTypeface);
                return true;
            }
        }
        for (var index = 0; index < Count; ++index) {
            var fontFamily = _fontFamilies[index];
            if (fontFamily.Name.ToLower(CultureInfo.InvariantCulture).StartsWith(familyName.ToLower(CultureInfo.InvariantCulture))
             && _glyphTypefaceCache.TryGetValue(fontFamily.Name, out glyphTypefaces) && TryGetNearestMatch(glyphTypefaces, key, out glyphTypeface))
                return true;
        }
        glyphTypeface = null;
        return false;
    }

    public override IEnumerator<FontFamily> GetEnumerator() {
        return _fontFamilies.GetEnumerator();
    }

    private void AddGlyphTypeface(IGlyphTypeface glyphTypeface) {
        AddGlyphTypefaceByFamilyName(glyphTypeface.FamilyName, glyphTypeface);

        void AddGlyphTypefaceByFamilyName(string familyName, IGlyphTypeface typeface) {
            _glyphTypefaceCache.GetOrAdd(familyName,
                _ => {
                    _fontFamilies.Add(new FontFamily(collectionKey, familyName));
                    return new ConcurrentDictionary<FontCollectionKey, IGlyphTypeface?>();
                }).TryAdd(new FontCollectionKey(typeface.Style, typeface.Weight, typeface.Stretch), typeface);
        }
    }
}
