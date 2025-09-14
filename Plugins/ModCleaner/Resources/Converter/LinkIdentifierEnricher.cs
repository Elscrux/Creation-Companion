using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CreationEditor;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins.Cache;
namespace ModCleaner.Resources.Converter;

public sealed class LinkIdentifierEnricher : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<ILinkCache> LinkCacheProperty =
        AvaloniaProperty.Register<LinkIdentifierEnricher, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            FormLinkIdentifier formLink => LinkCache.TryResolveIdentifier(formLink.FormLink, out var editorId)
                ? editorId is null
                    ? formLink
                    : editorId + " " + formLink.FormLink
                : formLink.ToString(),
            AssetLinkIdentifier asset => asset.ToString(),
            _ => (object?) null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new InvalidOperationException();
}