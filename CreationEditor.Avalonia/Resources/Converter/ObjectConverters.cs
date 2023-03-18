namespace CreationEditor.Avalonia.Converter;

public static class ObjectConverters {
    public static readonly ExtendedFuncValueConverter<object, bool, object> Equals
        = new(((x, y) => Equals(x, y)));
}
