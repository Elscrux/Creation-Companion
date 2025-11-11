using System.Collections;
using DynamicData;
using Noggog;
namespace CreationEditor;

public static class ListExtension {
    extension(IList list) {
        public void SwapItems(int sourceIndex, int targetIndex) {
            var item1 = list[sourceIndex];
            var item2 = list[targetIndex];
            list[targetIndex] = item1;
            list[sourceIndex] = item2;
        }
        public void MoveItem(int sourceIndex, int targetIndex) {
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
    }

    extension<T>(IReadOnlyList<T?> list) {
        public T? FirstOrDefault() {
            return list.Count > 0 ? list[0] : default;
        }
    }

    extension<T>(IList<T> list)
        where T : notnull {
        public void ReplaceWith(IEnumerable<T> newItems) {
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
        public void RemoveRange(int index, int count) {
            ArgumentNullException.ThrowIfNull(list);

            switch (list) {
                case List<T> list1:
                    list1.RemoveRange(index, count);
                    break;
                case Noggog.IExtendedList<T> list1:
                    list1.RemoveRange(index, count);
                    break;
                default:
                    for (var i = 0; i < count; i++) list.RemoveAt(index);
                    break;
            }
        }
        public void Apply(Change<T> item, IEqualityComparer<T>? equalityComparer = null) {
            list.Clone(item.AsEnumerable(), equalityComparer);
        }
        public void MoveItem(int sourceIndex, int targetIndex) {
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

        public void SwapItems(int sourceIndex, int targetIndex) {
            var item1 = list[sourceIndex];
            var item2 = list[targetIndex];
            list[targetIndex] = item1;
            list[sourceIndex] = item2;
        }

        public T FindOrAdd(Func<T, bool> predicate, Func<T> add) {
            var item = list.FirstOrDefault(predicate);
            if (item is not null) return item;

            item = add();
            list.Add(item);

            return item;
        }

        public void AddSorted(T item, IComparer<T> comparer) {
            var i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0) i++;

            list.Insert(i, item);
        }
    }

    extension<T>(IList<T> list) where T : IComparable {
        public void AddSorted(T item) {
            var i = 0;
            while (i < list.Count && list[i].CompareTo(item) < 0) i++;

            list.Insert(i, item);
        }
    }
}
