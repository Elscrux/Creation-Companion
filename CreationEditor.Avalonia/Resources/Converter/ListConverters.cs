using System.Collections;
namespace CreationEditor.Avalonia.Converter;

public static class ListConverters {
    public static readonly ExtendedFuncValueConverter<IList, bool, object> Contains
        = new((list, e) => list is not null && list.Contains(e));
}
