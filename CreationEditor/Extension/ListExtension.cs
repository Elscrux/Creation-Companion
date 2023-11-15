namespace CreationEditor;

public static class ListExtension {
    public static T? FirstOrDefault<T>(this IReadOnlyList<T?> list) {
        return list.Count > 0 ? list[0] : default;
    }
}
