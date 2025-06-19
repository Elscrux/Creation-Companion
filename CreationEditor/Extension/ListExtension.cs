using System.Collections;
using DynamicData;
using Noggog;
namespace CreationEditor;

public static class ListExtension {
    public static T? FirstOrDefault<T>(this IReadOnlyList<T?> list) {
        return list.Count > 0 ? list[0] : default;
    }

    public static T FindOrAdd<T>(this IList<T> list, Func<T, bool> predicate, Func<T> add) {
        var item = list.FirstOrDefault(predicate);
        if (item is not null) return item;

        item = add();
        list.Add(item);

        return item;
    }

    public static void ReplaceWith<T>(this IList<T> list, IEnumerable<T> newItems)
        where T : notnull {
        var counter = 0;
        using var enumerator = newItems.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (counter < list.Count) {
                list[counter] = enumerator.Current;
            } else {
                list.Add(enumerator.Current);
            }
            ++counter;
        }

        var remainingCount = list.Count - counter;
        if (remainingCount > 0) {
            list.RemoveRange(counter, remainingCount);
        }
    }

    public static void RemoveRange<T>(this IList<T> source, int index, int count)
        where T : notnull {
        ArgumentNullException.ThrowIfNull(source);

        switch (source) {
            case List<T> list:
                list.RemoveRange(index, count);
                break;
            case Noggog.IExtendedList<T> list:
                list.RemoveRange(index, count);
                break;
            default:
                for (var i = 0; i < count; i++) source.RemoveAt(index);
                break;
        }
    }

    public static void AddSorted<T>(this IList<T> list, T item)
        where T : IComparable {
        var i = 0;
        while (i < list.Count && list[i].CompareTo(item) < 0) i++;

        list.Insert(i, item);
    }

    public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer) {
        var i = 0;
        while (i < list.Count && comparer.Compare(list[i], item) < 0) i++;

        list.Insert(i, item);
    }

    public static void Apply<T>(this IList<T> source, Change<T> item, IEqualityComparer<T>? equalityComparer = null)
        where T : notnull {
        source.Clone(item.AsEnumerable(), equalityComparer);
    }

    public static void MoveItem(this IList list, int sourceIndex, int targetIndex) {
        if (sourceIndex < targetIndex)
        {
            var item = list[sourceIndex];
            list.RemoveAt(sourceIndex);
            list.Insert(targetIndex, item);
        }
        else
        {
            var removeIndex = sourceIndex + 1;
            if (list.Count + 1 > removeIndex)
            {
                var item = list[sourceIndex];
                list.RemoveAt(removeIndex - 1);
                list.Insert(targetIndex, item);
            }
        }
    }

    public static void MoveItem<T>(this IList<T> list, int sourceIndex, int targetIndex) {
        if (sourceIndex < targetIndex)
        {
            var item = list[sourceIndex];
            list.RemoveAt(sourceIndex);
            list.Insert(targetIndex, item);
        }
        else
        {
            var removeIndex = sourceIndex + 1;
            if (list.Count + 1 > removeIndex)
            {
                var item = list[sourceIndex];
                list.RemoveAt(removeIndex - 1);
                list.Insert(targetIndex, item);
            }
        }
    }

    public static void SwapItems(this IList list, int sourceIndex, int targetIndex) {
        var item1 = list[sourceIndex];
        var item2 = list[targetIndex];
        list[targetIndex] = item1;
        list[sourceIndex] = item2;
    }

    public static void SwapItems<T>(this IList<T> list, int sourceIndex, int targetIndex) {
        var item1 = list[sourceIndex];
        var item2 = list[targetIndex];
        list[targetIndex] = item1;
        list[sourceIndex] = item2;
    }
}
