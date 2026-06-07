using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using DynamicData.Binding;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins.Records;
namespace ModCleaner.Resources.Converter;

public sealed class IsExcludedConverter : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<IObservableCollection<ILinkIdentifier>> ExcludedLinksProperty =
        AvaloniaProperty.Register<LinkIdentifierEnricher, IObservableCollection<ILinkIdentifier>>(nameof(ExcludedLinks));

    public IObservableCollection<ILinkIdentifier> ExcludedLinks {
        get => GetValue(ExcludedLinksProperty);
        set => SetValue(ExcludedLinksProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            IMajorRecordGetter record => ExcludedLinks.Contains(new FormLinkIdentifier(record)),
            FormLinkIdentifier formLinkIdentifier => ExcludedLinks.Contains(formLinkIdentifier),
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new InvalidOperationException();
}
