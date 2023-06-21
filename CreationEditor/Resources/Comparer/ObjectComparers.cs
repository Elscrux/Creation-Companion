namespace CreationEditor.Resources.Comparer;

public static class ObjectComparers {
    public static int? CheckNull(object? a1, object? a2) {
        if (a1 is null) {
            if (a2 is null) return 0;

            return -1;
        }

        if (a2 is null) return 1;

        return null;
    }
}
