using CreationEditor.Resources.Comparer;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Comparer;

public static class NamedComparers {
    public static readonly FuncComparer<INamedRequiredGetter> NamedRequiredComparer
        = new((x, y) => {
            var xName = x.Name;
            var yName = y.Name;

            var xIsNullOrEmpty = string.IsNullOrEmpty(xName);
            var yIsNullOrEmpty = string.IsNullOrEmpty(yName);
            if (xIsNullOrEmpty) {
                if (yIsNullOrEmpty) return 0;

                return 1;
            }
            if (yIsNullOrEmpty) return -1;

            return StringComparer.OrdinalIgnoreCase.Compare(xName, yName);
        });
}
