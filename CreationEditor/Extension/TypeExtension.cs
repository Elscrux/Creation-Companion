namespace CreationEditor.Extension;

public static class TypeExtension {
    public static bool ContainsInterface(this Type originalType, Type type) {
        return originalType == type
         || originalType.GetInterfaces().Contains(type);
    }

    public static bool ContainsInterface(this IEnumerable<Type> types, Type type) {
        return types.Any(t => t.ContainsInterface(type));
    }
}
