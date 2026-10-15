using System.Reflection;
using Autofac;
using CreationEditor.Avalonia.ViewModels.Config;
using CreationEditor.Services.Config;
using Module = Autofac.Module;
namespace CreationEditor.Avalonia.Modules;

public sealed class ConfigModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IConfigOption>()
            .Keyed<IConfigOption>(x => x.FullName ?? throw new InvalidOperationException($"Config option type '{x.FullName}' does not have a full name"));

        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IConfigOptionVM>()
            .Keyed<IConfigOptionVM>(x => {
                var name = x.Name;
                var arityIndex = name.IndexOf('`');
                if (arityIndex > 0) name = name[..arityIndex];

                var optionTypeName = name.EndsWith("VM", StringComparison.Ordinal) ? name[..^2] : name;
                var optionType = assemblies
                        .SelectMany(SafeGetTypes)
                        .FirstOrDefault(t => typeof(IConfigOption).IsAssignableFrom(t) && StripArity(t.Name) == optionTypeName)
                 ?? throw new InvalidOperationException($"Could not find config option type for VM '{x.FullName}'");

                // Generic option types are keyed by their open definition so every closure resolves
                var keyType = optionType.IsGenericType ? optionType.GetGenericTypeDefinition() : optionType;
                return keyType.FullName ?? throw new InvalidOperationException($"Config option type '{optionType.FullName}' does not have a full name");
            });

        builder.RegisterType<ConfigVM>()
            .AsSelf();

        builder.RegisterType<ConfigOptionAutoDetector>()
            .As<IConfigOptionAutoDetector>()
            .SingleInstance();

        builder.RegisterType<ConfigVMFactory>()
            .As<IConfigVMFactory>();
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly) {
        try {
            return assembly.GetTypes();
        } catch (ReflectionTypeLoadException) {
            return [];
        }
    }

    private static string StripArity(string typeName) {
        var arityIndex = typeName.IndexOf('`');
        return arityIndex > 0 ? typeName[..arityIndex] : typeName;
    }
}
