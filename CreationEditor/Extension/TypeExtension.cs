using System.Reflection;
using Noggog;
namespace CreationEditor;

public static class TypeExtension {
    /// <param name="type">The type to get the property from</param>
    extension(Type type) {
        /// <summary>
        /// Custom get property method that supports properties that explicitly implement an interface and thus have
        /// a name like "Interface.Property" not just "Property" which causes the default GetProperty method to fail.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <returns>Property if found, otherwise null</returns>
        public PropertyInfo? GetPropertyCustom(string name) {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(p => {
                    var lastIndexOf = p.Name.AsSpan().LastIndexOf('.');
                    if (lastIndexOf == -1) return p.Name == name;

                    return string.Equals(p.Name[(lastIndexOf + 1)..], name, StringComparison.Ordinal);
                });
        }
        /// <summary>
        /// Returns it the calling type inherits from any of the types in the parameter enumerable.
        /// Any 1 : N
        /// </summary>
        /// <param name="parentTypes">Parent types</param>
        /// <returns>true if any inherits</returns>
        public bool InheritsFromAny(params IEnumerable<Type> parentTypes) {
            return parentTypes.Any(parentType => type.InheritsFrom(parentType));
        }
        /// <summary>
        /// Returns it the calling type inherits from all the types in the parameter enumerable.
        /// Any 1 : N
        /// </summary>
        /// <param name="parentTypes">Parent types</param>
        /// <returns>true if any inherits</returns>
        public bool InheritsFromAll(params IEnumerable<Type> parentTypes) {
            return parentTypes.All(parentType => type.InheritsFrom(parentType));
        }
        public bool InheritsFromOpenGeneric(Type openGenericType, bool excludeSelf = false, bool couldInherit = false) {
            if (openGenericType == type) return !excludeSelf;
            if (openGenericType.IsAssignableFrom(type)) return true;
            if (openGenericType.IsGenericType && IsAssignableToGenericType(type, openGenericType)) return true;

            if (couldInherit && openGenericType is { IsGenericParameter: true, BaseType: not null }) {
                return type.InheritsFrom(openGenericType.BaseType, excludeSelf: excludeSelf, couldInherit: couldInherit);
            }

            return false;
        }
        public bool IsAssignableToGenericType(Type genericType) {
            var genTypeDef = genericType.GetGenericTypeDefinition();
            foreach (var it in type.GetInterfaces()) {
                if (!it.IsGenericType) continue;

                var genDef = it.GetGenericTypeDefinition();
                if (genDef != genTypeDef) continue;

                return true;
            }

            var baseType = type.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
        public IEnumerable<T> GetAllSubClasses<T>(Func<Type, object?> creator) {
            return type
                .GetSubclassesOf()
                .Select(creator)
                .OfType<T>();
        }
        public IEnumerable<T> GetAllSubClasses<T>() {
            return type.GetAllSubClasses<T>(Activator.CreateInstance);
        }
    }

    /// <param name="types">Types that inherit</param>
    extension(IEnumerable<Type> types) {
        /// <summary>
        /// Returns if any of the types in the calling enumerable inherits from the parameter type.
        /// Any 1 : 1
        /// </summary>
        /// <param name="inheritedType">Parent type</param>
        /// <returns>true if any inherits</returns>
        public bool AnyInheritsFrom(Type inheritedType) {
            return types.Any(type => type.InheritsFrom(inheritedType));
        }
        /// <summary>
        /// Returns if all the types in the calling enumerable inherit from the parameter type.
        /// All 1 : 1
        /// </summary>
        /// <param name="inheritedType">Parent type</param>
        /// <returns>true if all inherit</returns>
        public bool AllInheritFrom(Type inheritedType) {
            return types.All(type => type.InheritsFrom(inheritedType));
        }
        /// <summary>
        /// Returns if all of the types in the calling enumerable inherit from any of the types in the parameter enumerable.
        /// All N : N
        /// </summary>
        /// <param name="inheritedTypes">Parent types</param>
        /// <returns>true if all inherit</returns>
        public bool AllInheritFromAny(IEnumerable<Type> inheritedTypes) {
            return types.All(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
        }
        /// <summary>
        /// Returns if any of the types in the calling enumerable inherits from any of the types in the parameter enumerable.
        /// Any N : N
        /// </summary>
        /// <param name="inheritedTypes">Parent types</param>
        /// <returns>true if any inherits</returns>
        public bool AnyInheritsFromAny(IEnumerable<Type> inheritedTypes) {
            return types.Any(type => inheritedTypes.Any(inheritedType => type.InheritsFrom(inheritedType)));
        }
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

    /// <param name="type">Type to get members from</param>
    extension(Type? type) {
        /// <summary>
        /// Returns all the members of the type.
        /// </summary>
        /// <returns>Members of the type</returns>
        public IEnumerable<T> GetAllMemberInfos<T>()
            where T : MemberInfo {
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
}
