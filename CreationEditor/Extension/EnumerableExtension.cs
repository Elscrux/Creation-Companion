namespace CreationEditor;

public static class EnumerableExtension {
    public static bool CountIsExactly<T>(this IEnumerable<T> enumerable, int count) {
        var counter = 0;
        foreach (var _ in enumerable) {
            counter++;
            if (counter > count) return false;
        }

        return counter == count;
    }
}
