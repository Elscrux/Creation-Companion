using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using ModCleaner.Models;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Resources.Converter;

public sealed class GetIncomingEdgesConverter : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<Graph<ILinkIdentifier, Edge<ILinkIdentifier>>> GraphProperty =
        AvaloniaProperty.Register<LinkIdentifierEnricher, Graph<ILinkIdentifier, Edge<ILinkIdentifier>>>(nameof(Graph));

    public Graph<ILinkIdentifier, Edge<ILinkIdentifier>> Graph {
        get => GetValue(GraphProperty);
        set => SetValue(GraphProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            IMajorRecordGetter record => Graph.IncomingEdges.GetValueOrDefault(new FormLinkIdentifier(record.ToStandardizedIdentifier()))?.Select(e => e.Source).ToList(),
            FormLinkIdentifier formLinkIdentifier => Graph.IncomingEdges.GetValueOrDefault(formLinkIdentifier)?.Select(e => e.Source).ToList(),
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new InvalidOperationException();
}
