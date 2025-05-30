using System.Reflection;
using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    /// <summary>
    /// Custom get property method that supports properties that explicitly implement an interface and thus have
    /// a name like "Interface.Property" not just "Property" which causes the default GetProperty method to fail.
    /// </summary>
    /// <param name="type">The type to get the property from</param>
    /// <param name="name">Name of the property</param>
    /// <returns>Property if found, otherwise null</returns>
    public static PropertyInfo? GetPropertyCustom(this Type type, string name) {
        return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(p => {
                var lastIndexOf = p.Name.AsSpan().LastIndexOf('.');
                if (lastIndexOf == -1) return p.Name == name;

                return string.Equals(p.Name[(lastIndexOf + 1)..], name, StringComparison.Ordinal);
            });
    }

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
    /// Returns if all the types in the calling enumerable inherit from the parameter type.
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
    public static bool InheritsFromAny(this Type type, params IEnumerable<Type> parentTypes) {
        return parentTypes.Any(parentType => type.InheritsFrom(parentType));
    }

    /// <summary>
    /// Returns it the calling type inherits from all the types in the parameter enumerable.
    /// Any 1 : N
    /// </summary>
    /// <param name="type">Types that inherits</param>
    /// <param name="parentTypes">Parent types</param>
    /// <returns>true if any inherits</returns>
    public static bool InheritsFromAll(this Type type, params IEnumerable<Type> parentTypes) {
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

    public static bool InheritsFromOpenGeneric(this Type type, Type openGenericType, bool excludeSelf = false, bool couldInherit = false) {
        if (openGenericType == type) return !excludeSelf;
        if (openGenericType.IsAssignableFrom(type)) return true;
        if (openGenericType.IsGenericType && IsAssignableToGenericType(type, openGenericType)) return true;
    
        if (couldInherit && openGenericType is { IsGenericParameter: true, BaseType: not null }) {
            return type.InheritsFrom(openGenericType.BaseType, excludeSelf: excludeSelf, couldInherit: couldInherit);
        }
    
        return false;
    }
    
    public static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var genTypeDef = genericType.GetGenericTypeDefinition();
        foreach (var it in givenType.GetInterfaces()) {
            if (!it.IsGenericType) continue;
    
            var genDef = it.GetGenericTypeDefinition();
            if (genDef != genTypeDef) continue;
    
            return true;
        }
    
        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }

    public static IEnumerable<T> GetAllSubClasses<T>(this Type type, Func<Type, object?> creator) {
        return type
            .GetSubclassesOf()
            .Select(creator)
            .OfType<T>();
    }

    public static IEnumerable<T> GetAllSubClasses<T>(this Type type) {
        return type.GetAllSubClasses<T>(Activator.CreateInstance);
    }

    public static IEnumerable<T> GetAllSubClasses<T>() {
        return GetAllSubClasses<T>(typeof(T));
    }

    private static readonly string[] BlacklistedNames = [
        "BinaryWriteTranslator",
        "Registration",
        "StaticRegistration",
        "FormVersion",
    ];

    private static readonly Type[] BlacklistedTypes = [
        typeof(Type),
    ];

    /// <summary>
    /// Returns all the members of the type.
    /// </summary>
    /// <param name="type">Type to get members from</param>
    /// <returns>Members of the type</returns>
    public static IEnumerable<T> GetAllMemberInfos<T>(this Type? type) where T : MemberInfo {
        if (type is null) return [];

        if (!type.IsInterface) {
            return GetMembers(type);
        }

        // If the record type is an interface, we need to get the fields from all the interfaces it inherits from.
        var dictionary = new Dictionary<string, T>();
        var typeQueue = new Queue<Type>();
        typeQueue.Enqueue(type);

        while (typeQueue.Count != 0) {
            var t = typeQueue.Dequeue();
            foreach (var queryField in GetMembers(t)) {
                dictionary.TryAdd(queryField.Name, queryField);
            }

            foreach (var @interface in t.GetInterfaces()) {
                typeQueue.Enqueue(@interface);
            }
        }

        return dictionary.Values.OrderBy(emember => emember.Name);

        IEnumerable<T> GetMembers(Type t) => t.GetMembers()
            .OfType<T>()
            .Where(p => !BlacklistedNames.Contains(p.Name) && !BlacklistedTypes.Contains(p.DeclaringType));
    }
}
