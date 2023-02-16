namespace CreationEditor.Extension; 

public static class TypeExtension {
    public static bool ContainsInterface(this IEnumerable<Type> types, Type type) {
        var typesList = types.ToList();
        return typesList.Contains(type)
         || typesList.Any(t => t.GetInterfaces().Contains(type));
    }
}
