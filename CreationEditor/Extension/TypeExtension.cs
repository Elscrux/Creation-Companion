using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    public static bool AnyInheritsFrom(this IEnumerable<Type> types, Type type) {
        return types.Any(t => t.InheritsFrom(type));
    }
}
