using System.Reflection;
using Autofac;
using CreationEditor.Avalonia.ViewModels.Integrations;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Sources;
using Module = Autofac.Module;
namespace CreationEditor.Avalonia.Modules;

public sealed class IntegrationsModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<IntegrationProvider>()
            .As<IIntegrationProvider>()
            .SingleInstance();

        builder.RegisterType<IntegrationsRegistry>()
            .As<IIntegrationRegistry>()
            .SingleInstance();

        builder.RegisterType<IntegrationCategoryItemVM>()
            .AsSelf();

        builder.RegisterType<IntegrationsVM>()
            .AsSelf();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var integrationTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false }
             && typeof(IIntegration).IsAssignableFrom(type))
            .ToArray();

        // Register each integration's static descriptor so the provider can reference integrations
        // without instantiating them.
        foreach (var integrationType in integrationTypes) {
            var propertyInfo = integrationType.GetProperty(nameof(IIntegration.StaticDescriptor), BindingFlags.Static | BindingFlags.Public);
            if (propertyInfo?.GetValue(integrationType) is IntegrationDescriptor descriptor) {
                builder.RegisterInstance(descriptor)
                    .As<IntegrationDescriptor>();

                builder.RegisterType(integrationType)
                    .Keyed<IIntegration>(descriptor.Id);
            } else {
                throw new InvalidOperationException(
                    $"Integration type '{integrationType.FullName}' does not have a public static Descriptor property of type IntegrationDescriptor");
            }
        }

        builder.RegisterType<IntegrationFactory>()
            .As<IIntegrationFactory>()
            .SingleInstance();

        // Sources: discover all IIntegrationSource implementations and register them.
        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IIntegrationSource>()
            .As<IIntegrationSource>()
            .SingleInstance();

        builder.RegisterType<IntegrationSourceProvider>()
            .As<IIntegrationSourceProvider>()
            .SingleInstance();

        builder.RegisterType<ManualDownloadHandler>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<IntegrationSourceManager>()
            .As<IIntegrationSourceManager>()
            .SingleInstance();

        // Shared HttpClient for sources.
        builder.Register(_ => new HttpClient())
            .As<HttpClient>()
            .SingleInstance();
    }
}
