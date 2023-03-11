namespace CreationEditor;

public static class TypeExtension {
    public static bool IsOrContainsInterface(this Type originalType, Type type) {
        return originalType == type
         || originalType.GetInterfaces().Contains(type);
    }

    public static bool IsOrContainsInterface(this IEnumerable<Type> types, Type type) {
        return types.Any(t => t.IsOrContainsInterface(type));
    }
}
