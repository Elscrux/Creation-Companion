using System.Reflection;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Integrations;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.BSA;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Config view model for the BSA archive action. Provides a dropdown of configured BSA archive
/// creator integrations and exposes the selected integration's own config view.
/// </summary>
public sealed partial class BSAArchiveActionConfigVM : ViewModel {
    private readonly IIntegrationRegistry _registry;
    private readonly IIntegrationProvider _provider;

    public BSAArchiveActionConfig Config { get; }

    public ObservableCollectionExtended<IntegrationItemVM> AvailableIntegrations { get; } = [];

    [Reactive] public partial IntegrationItemVM? SelectedIntegration { get; set; }
    [Reactive] public partial object? IntegrationConfigVM { get; set; }

    public BSAArchiveActionConfigVM(
        BSAArchiveActionConfig config,
        IIntegrationRegistry registry,
        IIntegrationProvider provider) {
        Config = config;
        _registry = registry;
        _provider = provider;

        RefreshIntegrations();

        registry.Changed
            .Subscribe(_ => RefreshIntegrations())
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedIntegration)
            .Subscribe(integration => {
                Config.SelectedIntegrationId = integration?.Integration.ImplementationId ?? Guid.Empty;
                IntegrationConfigVM = CreateIntegrationConfigVM(integration);
            })
            .DisposeWith(this);
    }

    private void RefreshIntegrations() {
        AvailableIntegrations.Clear();

        var bsaImplementations = _provider.GetImplementations(typeof(IBSAArchiveCreator))
            .Select(impl => impl.Id)
            .ToHashSet();

        foreach (var integration in _registry.Integrations.Where(i => bsaImplementations.Contains(i.ImplementationId))) {
            var implementation = _provider.GetImplementation(integration.ImplementationId);
            if (implementation is not null) {
                AvailableIntegrations.Add(new IntegrationItemVM(integration, implementation));
            }
        }

        SelectedIntegration = AvailableIntegrations.FirstOrDefault(i => i.Integration.ImplementationId == Config.SelectedIntegrationId);
    }

    private object? CreateIntegrationConfigVM(IntegrationItemVM? item) {
        if (item is null) return null;

        var config = item.Integration.Config ?? item.Implementation.CreateDefaultConfig();
        return CreateConfigVM(item.Implementation.ConfigType, config);
    }

    /// <summary>
    /// Creates a config VM for the given config model by naming convention
    /// (<c>FooConfig</c> → <c>FooConfigVM</c>) across loaded assemblies.
    /// </summary>
    private static object? CreateConfigVM(Type configType, object config) {
        var configVMTypeName = configType.Name + "VM";
        var configVMType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => SafeGetTypes(a))
            .FirstOrDefault(t => t.Name == configVMTypeName && t.IsClass && !t.IsAbstract);

        return configVMType is null
            ? null
            : System.Activator.CreateInstance(configVMType, config);
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly) {
        try {
            return assembly.GetTypes();
        } catch (ReflectionTypeLoadException) {
            return [];
        }
    }
}
