namespace CreationEditor.Avalonia.Converter;

public static class ObjectConverters {
    public new static readonly ExtendedFuncValueConverter<object, bool, object> Equals
        = new(((obj, parameter) => Equals(obj, parameter)));
}
