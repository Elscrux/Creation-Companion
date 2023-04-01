using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    public static bool InheritsFromAny(this Type type, params Type[] inheritedTypes) {
        return inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType));
    }

    public static bool AnyInheritsFrom(this IEnumerable<Type> types, Type inheritedType) {
        return types.Any(type => type.InheritsFrom(inheritedType));
    }

    public static bool AnyInheritsFrom(this IEnumerable<Type> types, params Type[] inheritedTypes) {
        return types.Any(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
    }

    public static bool AllInheritFrom(this IEnumerable<Type> types, Type inheritedType) {
        return types.All(type => type.InheritsFrom(inheritedType));
    }

    public static bool AllInheritFromAny(this IEnumerable<Type> types, params Type[] inheritedTypes) {
        return types.All(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
    }
}
