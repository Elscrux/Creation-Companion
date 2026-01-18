using System.Collections;
using System.Collections.Specialized;
namespace CreationEditor;

public static class NotifyCollectionChangedEventArgsExtension {
    extension(NotifyCollectionChangedEventArgs e) {
        public IEnumerable<NotifyCollectionChangedEventArgs> Transform<TSource, TTarget>(IList currentItems, IList? newItems, Func<TSource, TTarget?> selector) {
            yield return e.Action switch {
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
                NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    e.NewItems!,
                    e.OldItems!),
                NotifyCollectionChangedAction.Move =>
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Move,
                        e.NewItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList(),
                        e.NewStartingIndex,
                        e.OldStartingIndex),
                NotifyCollectionChangedAction.Reset =>
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        currentItems),
                _ => throw new ArgumentOutOfRangeException(nameof(e)),
            };

            yield return e.Action switch {
                NotifyCollectionChangedAction.Reset =>
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        newItems?.OfType<TSource>().Select(selector).Where(x => x is not null).ToList()),
                _ => throw new ArgumentOutOfRangeException(nameof(e)),
            };
        }
    }
}
