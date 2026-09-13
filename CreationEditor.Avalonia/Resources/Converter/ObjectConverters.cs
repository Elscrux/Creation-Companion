using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class ObjectConverters {
    public new static readonly ExtendedFuncValueConverter<object, bool, object> Equals
        = new((obj, parameter) => Equals(obj, parameter));

    public static readonly FuncMultiValueConverter<object, bool> EqualsTwoBindings
        = new(objs => {
            if (objs is not [var a, var b]) return false;
            if (a is null) return b is null;

            return a.Equals(b);
        });

    /// <summary>True when the bound value is null.</summary>
    public static readonly FuncValueConverter<object?, bool> IsNull
        = new(value => value is null);

    /// <summary>True when the bound value is not null.</summary>
    public static readonly FuncValueConverter<object?, bool> IsNotNull
        = new(value => value is not null);
}
