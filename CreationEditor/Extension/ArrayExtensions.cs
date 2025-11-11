namespace CreationEditor;

public static class ArrayExtensions {
    extension<T>(T[] t) {
        public void Clear() => Array.Clear(t, 0, t.Length);
        public void Clear(int index, int length) => Array.Clear(t, index, length);
        public void Copy(Array array, int length) => Array.Copy(t, array, length);
        public void Copy(int index, Array array, int index2, int length) => Array.Copy(t, index, array, index2, length);
        public TOutput[] ConvertAll<TOutput>(Converter<T, TOutput> converter) => Array.ConvertAll(t, converter);
        public bool Exists(Predicate<T> predicate) => Array.Exists(t, predicate);
        public T? Find(Predicate<T> predicate) => Array.Find(t, predicate);
        public T[] FindAll(Predicate<T> predicate) => Array.FindAll(t, predicate);
        public int FindIndex(Predicate<T> predicate) => Array.FindIndex(t, predicate);
        public int FindIndex(int startIndex, Predicate<T> predicate) => Array.FindIndex(t, startIndex, predicate);
        public int FindIndex(int startIndex, int count, Predicate<T> predicate) => Array.FindIndex(t, startIndex, count, predicate);
        public T? FindLast(Predicate<T> predicate) => Array.FindLast(t, predicate);
        public int FindLastIndex(Predicate<T> predicate) => Array.FindLastIndex(t, predicate);
        public int FindLastIndex(int startIndex, Predicate<T> predicate) => Array.FindLastIndex(t, startIndex, predicate);
        public int FindLastIndex(int startIndex, int count, Predicate<T> predicate) => Array.FindLastIndex(t, startIndex, count, predicate);
        public void ForEach(Action<T> action) => Array.ForEach(t, action);
        public int IndexOf(T value) => Array.IndexOf(t, value);
        public int IndexOf(T value, int startIndex) => Array.IndexOf(t, value, startIndex);
        public int IndexOf(T value, int startIndex, int count) => Array.IndexOf(t, value, startIndex, count);
        public int LastIndexOf(T value) => Array.LastIndexOf(t, value);
        public int LastIndexOf(T value, int startIndex) => Array.LastIndexOf(t, value, startIndex);
        public int LastIndexOf(T value, int startIndex, int count) => Array.LastIndexOf(t, value, startIndex, count);
        public void Reverse() => Array.Reverse((Array) t);
        public void Reverse(int index, int length) => Array.Reverse((Array) t, index, length);
        public void Sort() => Array.Sort((Array) t);
        public void Sort(Comparison<T> comparison) => Array.Sort(t, comparison);
        public void Sort(IComparer<T>? comparer) => Array.Sort(t, comparer);
        public void Sort(int index, int length) => Array.Sort((Array) t, index, length);
        public void Sort(int index, int length, IComparer<T>? comparer) => Array.Sort(t, index, length, comparer);
        public bool TrueForAll(Predicate<T> predicate) => Array.TrueForAll(t, predicate);
    }
}
