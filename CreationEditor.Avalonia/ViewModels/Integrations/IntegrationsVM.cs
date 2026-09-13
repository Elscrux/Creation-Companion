using CreationEditor.Services.Integrations;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// View model for the Integrations window. Left side lists integration capabilities (e.g.
/// <c>IBSAArchiveCreator</c>); right side shows the configured integrations for the selected
/// capability and lets the user add unconfigured implementations.
/// </summary>
public sealed partial class IntegrationsVM : ViewModel {
    private readonly IIntegrationProvider _provider;
    private readonly IIntegrationRegistry _registry;

    public IReadOnlyList<IntegrationTypeItemVM> IntegrationTypes { get; }

    public ObservableCollectionExtended<IntegrationItemVM> AddedIntegrations { get; } = [];
    public ObservableCollectionExtended<AvailableIntegrationVM> AvailableImplementations { get; } = [];

    [Reactive] public partial IntegrationTypeItemVM? SelectedIntegrationType { get; set; }
    [Reactive] public partial RegisteredIntegration? SelectedIntegration { get; set; }

    public IntegrationsVM(
        IIntegrationProvider provider,
        IIntegrationRegistry registry) {
        _provider = provider;
        _registry = registry;

        IntegrationTypes = provider.IntegrationTypes
            .Select(type => new IntegrationTypeItemVM(type, GetCategoryName(type), provider, registry))
            .ToList();
        SelectedIntegrationType = IntegrationTypes.FirstOrDefault();

        this.WhenAnyValue(x => x.SelectedIntegrationType)
            .Subscribe(_ => Refresh())
            .DisposeWith(this);

        registry.Changed
            .Subscribe(_ => Refresh())
            .DisposeWith(this);

        Refresh();
    }

    private string GetCategoryName(Type integrationType) {
        // Try to get the CategoryName from a known implementation.
        var implementation = _provider.GetImplementations(integrationType).FirstOrDefault();
        return implementation?.CategoryName ?? integrationType.Name;
    }

    private void Refresh() {
        AddedIntegrations.Clear();
        AvailableImplementations.Clear();

        if (SelectedIntegrationType is null) return;

        var implementations = _provider.GetImplementations(SelectedIntegrationType.IntegrationType);
        var configuredIds = _registry.Integrations
            .Where(i => implementations.Any(impl => impl.Id == i.ImplementationId))
            .Select(i => i.ImplementationId)
            .ToHashSet();

        foreach (var integration in _registry.Integrations.Where(i => configuredIds.Contains(i.ImplementationId))) {
            var implementation = implementations.FirstOrDefault(impl => impl.Id == integration.ImplementationId);
            if (implementation is not null) {
                AddedIntegrations.Add(new IntegrationItemVM(integration, implementation));
            }
        }

        foreach (var implementation in implementations.Where(impl => !configuredIds.Contains(impl.Id))) {
            AvailableImplementations.Add(new AvailableIntegrationVM(implementation, _registry));
        }
    }

    [ReactiveCommand]
    private void RemoveIntegration(IntegrationItemVM item) {
        _registry.Remove(item.Integration);
        if (SelectedIntegration == item.Integration) SelectedIntegration = null;
    }

    [ReactiveCommand]
    private void Save() {
        _registry.Persist();
    }
}
