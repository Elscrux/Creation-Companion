using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class IntConverters {
    public new static readonly ExtendedFuncValueConverter<int, bool, int> Equals
        = new((x, y) => x == y);

    public static readonly ExtendedFuncValueConverter<int, bool, int> NotEquals
        = new((x, y) => x != y);

    public static readonly FuncMultiValueConverter<int, int> Min
        = new(ints => ints.Min());

    public static readonly FuncMultiValueConverter<int, int> Max
        = new(ints => ints.Max());
}
