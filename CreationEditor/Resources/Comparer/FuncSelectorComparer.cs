namespace CreationEditor.Resources.Comparer;

public sealed class FuncSelectorComparer<TSelector, TCompare>(
    Func<TSelector, TCompare?> selector,
    Func<TCompare, TCompare, int> compare)
    : FuncComparer<TCompare>(compare), IComparer<TSelector> {

    public override int Compare(object? x, object? y) {
        if (x is TSelector t1 && y is TSelector t2) {
            return Compare(t1, t2);
        }

        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not {typeof(TSelector).Name}");
    }

    public int Compare(TSelector? x, TSelector? y) {
        if (x is null) {
            if (y is null) return 0;

            return -1;
        }
        if (y is null) return 1;

        return base.Compare(selector(x), selector(y));
    }
}
