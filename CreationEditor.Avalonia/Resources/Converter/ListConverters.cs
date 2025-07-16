using System.Collections;
using Noggog;
namespace CreationEditor.Avalonia.Converter;

public static class ListConverters {
    public static readonly ExtendedFuncValueConverter<IList, bool, object> Contains
        = new((list, e) => list is not null && list.Contains(e));

    public static readonly ExtendedFuncValueConverter<IEnumerable, string, object> JoinToString
        = new((enumerable, e) => {
            if (enumerable is null || e is not string s) return string.Empty;

            return string.Join(s, enumerable
                .OfType<object?>()
                .WhereNotNull()
                .Select(e => e.ToString()));
        });
}
