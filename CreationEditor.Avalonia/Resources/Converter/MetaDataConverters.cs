using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class MetaDataConverters {
    public new static IValueConverter ToString { get; } = new FuncValueConverter<(string Name, object Value)[], string>(ConvertMetaData);
    public static IValueConverter ToName { get; } = new FuncValueConverter<(string Name, object Value), string>(tuple => tuple.Name ?? string.Empty);
    public static IValueConverter ToValue { get; } =
        new FuncValueConverter<(string Name, object Value), string>(tuple => ObjectConverters.GetStringValue(tuple.Value) ?? string.Empty);

    private static string ConvertMetaData((string Name, object Value)[]? metaData) {
        return metaData is not null
            ? string.Join("\n", metaData.Select(x => x.Name + ": " + ObjectConverters.GetStringValue(x.Value)))
            : string.Empty;
    }
}
