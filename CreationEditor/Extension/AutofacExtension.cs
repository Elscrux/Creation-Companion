using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Core;
namespace CreationEditor;

public static class AutofacExtension {
    extension(IComponentContext context) {
        public bool TryResolve<T>(Parameter parameter, [NotNullWhen(true)] out T? instance)
            where T : class {
            instance = context.ResolveOptional<T>(parameter);
            return instance != null;
        }
        public bool TryResolve<T>(Parameter parameter1, Parameter parameter2, [NotNullWhen(true)] out T? instance)
            where T : class {
            instance = context.ResolveOptional<T>(parameter1, parameter2);
            return instance != null;
        }
        public bool TryResolve<T>(IEnumerable<Parameter> parameters, [NotNullWhen(true)] out T? instance)
            where T : class {
            instance = context.ResolveOptional<T>(parameters);
            return instance != null;
        }
    }
}
