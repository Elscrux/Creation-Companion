using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Core;
namespace CreationEditor;

public static class AutofacExtension {
    public static bool TryResolve<T>(this IComponentContext context, Parameter parameter, [NotNullWhen(true)] out T? instance)
        where T : class {
        instance = context.ResolveOptional<T>(parameter);
        return instance != null;
    }

    public static bool TryResolve<T>(this IComponentContext context, Parameter parameter1, Parameter parameter2, [NotNullWhen(true)] out T? instance)
        where T : class {
        instance = context.ResolveOptional<T>(parameter1, parameter2);
        return instance != null;
    }

    public static bool TryResolve<T>(this IComponentContext context, IEnumerable<Parameter> parameters, [NotNullWhen(true)] out T? instance)
        where T : class {
        instance = context.ResolveOptional<T>(parameters);
        return instance != null;
    }
}
