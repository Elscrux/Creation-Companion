namespace CreationEditor.Avalonia.Comparer;

public sealed class FuncSelectorComparer<TSelector, TCompare> : FuncComparer<TCompare>, IComparer<TSelector> {
    private readonly Func<TSelector, TCompare?> _selector;
    
    public FuncSelectorComparer(Func<TSelector, TCompare?> selector, Func<TCompare, TCompare, int> compare) : base(compare) {
        _selector = selector;
    }

    public override int Compare(object? x, object? y) {
        if (x is TSelector t1 && y is TSelector t2) {
            return Compare(t1, t2);
        }
        
        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not {typeof(TSelector).Name}");
    }
    
    public int Compare(TSelector? x, TSelector? y) {
        if (x == null) {
            if (y == null) return 0;

            return -1;
        }
        if (y == null) return 1;
        
        return base.Compare(_selector(x), _selector(y));
    }
}
