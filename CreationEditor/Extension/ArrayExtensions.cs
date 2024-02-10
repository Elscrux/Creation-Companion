namespace CreationEditor;

public static class ArrayExtensions {
    #region Clear
    public static void Clear<T>(this T[] t) {
        Array.Clear(t, 0, t.Length);
    }

    public static void Clear<T>(this T[] t, int index, int length) {
        Array.Clear(t, index, length);
    }
    #endregion

    #region Copy
    public static void Copy<T>(this T[] t, Array array, int length) {
        Array.Copy(t, array, length);
    }

    public static void Copy<T>(this T[] t, int index, Array array, int index2, int length) {
        Array.Copy(t, index, array, index2, length);
    }
    #endregion

    #region ConvertAll
    public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] t, Converter<TInput, TOutput> converter) {
        return Array.ConvertAll(t, converter);
    }
    #endregion

    #region Exists
    public static bool Exists<T>(this T[] t, Predicate<T> predicate) {
        return Array.Exists(t, predicate);
    }
    #endregion

    #region Find
    public static T? Find<T>(this T[] t, Predicate<T> predicate) {
        return Array.Find(t, predicate);
    }
    #endregion

    #region FindAll
    public static T[] FindAll<T>(this T[] t, Predicate<T> predicate) {
        return Array.FindAll(t, predicate);
    }
    #endregion

    #region FindIndex
    public static int FindIndex<T>(this T[] t, Predicate<T> predicate) {
        return Array.FindIndex(t, predicate);
    }

    public static int FindIndex<T>(this T[] t, int startIndex, Predicate<T> predicate) {
        return Array.FindIndex(t, startIndex, predicate);
    }

    public static int FindIndex<T>(this T[] t, int startIndex, int count, Predicate<T> predicate) {
        return Array.FindIndex(t, startIndex, count, predicate);
    }
    #endregion

    #region FindLast
    public static T? FindLast<T>(this T[] t, Predicate<T> predicate) {
        return Array.FindLast(t, predicate);
    }
    #endregion

    #region FindLastIndex
    public static int FindLastIndex<T>(this T[] t, Predicate<T> predicate) {
        return Array.FindLastIndex(t, predicate);
    }

    public static int FindLastIndex<T>(this T[] t, int startIndex, Predicate<T> predicate) {
        return Array.FindLastIndex(t, startIndex, predicate);
    }

    public static int FindLastIndex<T>(this T[] t, int startIndex, int count, Predicate<T> predicate) {
        return Array.FindLastIndex(t, startIndex, count, predicate);
    }
    #endregion

    #region ForEach
    public static void ForEach<T>(this T[] t, Action<T> action) {
        Array.ForEach(t, action);
    }
    #endregion

    #region IndexOf
    public static int IndexOf<T>(this T[] t, T value) {
        return Array.IndexOf(t, value);
    }

    public static int IndexOf<T>(this T[] t, T value, int startIndex) {
        return Array.IndexOf(t, value, startIndex);
    }

    public static int IndexOf<T>(this T[] t, T value, int startIndex, int count) {
        return Array.IndexOf(t, value, startIndex, count);
    }
    #endregion

    #region LastIndexOf
    public static int LastIndexOf<T>(this T[] t, T value) {
        return Array.LastIndexOf(t, value);
    }

    public static int LastIndexOf<T>(this T[] t, T value, int startIndex) {
        return Array.LastIndexOf(t, value, startIndex);
    }

    public static int LastIndexOf<T>(this T[] t, T value, int startIndex, int count) {
        return Array.LastIndexOf(t, value, startIndex, count);
    }
    #endregion

    #region Reverse
    public static void Reverse<T>(this T[] t) {
        Array.Reverse(t);
    }

    public static void Reverse<T>(this T[] t, int index, int length) {
        Array.Reverse(t, index, length);
    }
    #endregion

    #region Sort
    public static void Sort<T>(this T[] t) {
        Array.Sort(t);
    }

    public static void Sort<T>(this T[] t, Comparison<T> comparison) {
        Array.Sort(t, comparison);
    }

    public static void Sort<T>(this T[] t, IComparer<T>? comparer) {
        Array.Sort(t, comparer);
    }

    public static void Sort<T>(this T[] t, int index, int length) {
        Array.Sort(t, index, length);
    }

    public static void Sort<T>(this T[] t, int index, int length, IComparer<T>? comparer) {
        Array.Sort(t, index, length, comparer);
    }
    #endregion
    
    #region TrueForAll
    public static bool TrueForAll<T>(this T[] t, Predicate<T> predicate) {
        return Array.TrueForAll(t, predicate);
    }
    #endregion
}
