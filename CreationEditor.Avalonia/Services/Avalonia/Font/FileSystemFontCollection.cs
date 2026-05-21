using System.Reflection;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform;
namespace CreationEditor.Avalonia.Services.Avalonia.Font;

// This is a copy of EmbeddedFontCollection https://github.com/AvaloniaUI/Avalonia/blob/b16975cb0402cd10358f17135d25c3dd3aac7ac6/src/Avalonia.Base/Media/Fonts/EmbeddedFontCollection.cs#L10
// The only changes were made in the Initialize method to load ttf files from a directory instead of an avalonia resource and in the AddGlyphTypeface method to simplify the logic
public class FileSystemFontCollection : FontCollectionBase {
    public override Uri Key { get; }

    public FileSystemFontCollection(Uri key, Uri source) {
        Key = key;

        if (!FontManager.Current.TryGetProperty<IFontManagerImpl>("PlatformImpl", out var platformImpl)) return;

        foreach (var loadFontAsset in Directory.EnumerateFiles(source.AbsolutePath, "*.ttf")) {
            using var stream = File.Open(loadFontAsset, FileMode.Open, FileAccess.Read, FileShare.Read);
            var methodInfo = typeof(IFontManagerImpl)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.Name.Contains("TryCreateGlyphTypeface"))
                .Skip(1)
                .First();

            object?[] parameters = [stream, FontSimulations.None, null];
            methodInfo.Invoke(platformImpl, parameters);
            if (parameters[2] is IPlatformTypeface glyphTypeface) {
                TryAddGlyphTypeface(new GlyphTypeface(glyphTypeface));
            }
        }
    }
}
