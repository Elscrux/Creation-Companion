using System.Collections.Specialized;
namespace CreationEditor;

public static class NotifyCollectionChangedEventArgsExtension {
    public static NotifyCollectionChangedEventArgs Transform<TSource, TTarget>(
        this NotifyCollectionChangedEventArgs e,
        Func<TSource, TTarget?> selector) {
        return e.Action switch {
            NotifyCollectionChangedAction.Add =>
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    e.NewItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList(),
                    e.NewStartingIndex),
            NotifyCollectionChangedAction.Remove =>
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    e.OldItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList(),
                    e.OldStartingIndex),
            NotifyCollectionChangedAction.Replace =>
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    e.NewItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList(),
                    e.NewStartingIndex,
                    e.OldStartingIndex),
            NotifyCollectionChangedAction.Move =>
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Move,
                    e.NewItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList(),
                    e.NewStartingIndex,
                    e.OldStartingIndex),
            NotifyCollectionChangedAction.Reset =>
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset),
            _ => throw new ArgumentOutOfRangeException(nameof(e))
        };
    }
}
