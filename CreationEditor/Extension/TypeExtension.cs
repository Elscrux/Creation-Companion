using Autofac;
using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    /// <summary>
    /// Returns if any of the types in the calling enumerable inherits from the parameter type.
    /// Any 1 : 1
    /// </summary>
    /// <param name="types">Types that inherit</param>
    /// <param name="inheritedType">Parent type</param>
    /// <returns>true if any inherits</returns>
    public static bool AnyInheritsFrom(this IEnumerable<Type> types, Type inheritedType) {
        return types.Any(type => type.InheritsFrom(inheritedType));
    }

    /// <summary>
    /// Returns if all of the types in the calling enumerable inherit from the parameter type.
    /// All 1 : 1
    /// </summary>
    /// <param name="types">Types that inherit</param>
    /// <param name="inheritedType">Parent type</param>
    /// <returns>true if all inherit</returns>
    public static bool AllInheritFrom(this IEnumerable<Type> types, Type inheritedType) {
        return types.All(type => type.InheritsFrom(inheritedType));
    }

    /// <summary>
    /// Returns it the calling type inherits from any of the types in the parameter enumerable.
    /// Any 1 : N
    /// </summary>
    /// <param name="type">Types that inherits</param>
    /// <param name="parentTypes">Parent types</param>
    /// <returns>true if any inherits</returns>
    public static bool InheritsFromAny(this Type type, params Type[] parentTypes) {
        return parentTypes.Any(parentType => type.InheritsFrom(parentType));
    }

    /// <summary>
    /// Returns it the calling type inherits from all of the types in the parameter enumerable.
    /// Any 1 : N
    /// </summary>
    /// <param name="type">Types that inherits</param>
    /// <param name="parentTypes">Parent types</param>
    /// <returns>true if any inherits</returns>
    public static bool InheritsFromAll(this Type type, params Type[] parentTypes) {
        return parentTypes.All(parentType => type.InheritsFrom(parentType));
    }

    /// <summary>
    /// Returns if all of the types in the calling enumerable inherit from any of the types in the parameter enumerable.
    /// All N : N
    /// </summary>
    /// <param name="types">Types that inherit</param>
    /// <param name="inheritedTypes">Parent types</param>
    /// <returns>true if all inherit</returns>
    public static bool AllInheritFromAny(this IEnumerable<Type> types, IEnumerable<Type> inheritedTypes) {
        return types.All(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
    }

    /// <summary>
    /// Returns if any of the types in the calling enumerable inherits from any of the types in the parameter enumerable.
    /// Any N : N
    /// </summary>
    /// <param name="types">Types that inherit</param>
    /// <param name="inheritedTypes">Parent types</param>
    /// <returns>true if any inherits</returns>
    public static bool AnyInheritsFromAny(this IEnumerable<Type> types, IEnumerable<Type> inheritedTypes) {
        return types.Any(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
    }

    public static IEnumerable<T> GetAllSubClass<T>(this Type type, Func<Type, object?> creator) {
        return type
            .GetSubclassesOf()
            .Select(creator)
            .OfType<T>();
    }

    public static IEnumerable<T> ResolveAllSubClassAndGenericByInterface<T>(this Type type, IComponentContext componentContext) {
        return type.GetAllSubClass<T>(subClassType => {
            if (subClassType.ContainsGenericParameters) {
                foreach (var @interface in subClassType.GetInterfaces()) {
                    var resolveOptional = componentContext.ResolveOptional(@interface);
                    if (resolveOptional != null) {
                        return resolveOptional;
                    }
                }
            }
            return componentContext.Resolve(subClassType);
        });
    }

    public static IEnumerable<T> GetAllSubClass<T>(this Type type) {
        return type.GetAllSubClass<T>(Activator.CreateInstance);
    }

    public static IEnumerable<T> GetAllSubClass<T>() {
        return GetAllSubClass<T>(typeof(T));
    }
}
