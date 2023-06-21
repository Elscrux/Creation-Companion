using System.Collections;
namespace CreationEditor.Resources.Comparer;

public class FuncComparer<TCompare> : IComparer<TCompare>, IComparer {
    private readonly Func<TCompare, TCompare, int> _compare;

    public FuncComparer(Func<TCompare, TCompare, int> compare) {
        _compare = compare;
    }

    public virtual int Compare(object? x, object? y) {
        if (x is TCompare t1 && y is TCompare t2) {
            return Compare(t1, t2);
        }

        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not {typeof(TCompare).Name}");
    }

    public virtual int Compare(TCompare? x, TCompare? y) {
        if (x is null) {
            if (y is null) return 0;

            return -1;
        }
        if (y is null) return 1;

        return _compare(x, y);
    }
}
