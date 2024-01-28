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
}
