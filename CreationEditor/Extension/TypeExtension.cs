using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    public static bool AnyInheritsFrom(this IEnumerable<Type> types, Type inheritedType) {
        return types.Any(type => type.InheritsFrom(inheritedType));
    }

    public static bool AnyInheritsFrom(this IEnumerable<Type> types, params Type[] inheritedTypes) {
        return types.Any(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
    }
}
