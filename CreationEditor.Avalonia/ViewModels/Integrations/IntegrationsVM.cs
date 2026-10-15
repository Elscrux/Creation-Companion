using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels.Config;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Sources;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// View model for the Integrations window. Left side lists integration capabilities,
/// right side shows the registered integrations for the selected
/// capability and lets the user add unregistered implementations.
/// </summary>
public sealed partial class IntegrationsVM : ViewModel {
    private readonly IIntegrationProvider _provider;
    private readonly IIntegrationRegistry _registry;
    private readonly IConfigVMFactory _configVMFactory;
    private readonly IIntegrationSourceManager _sourceManager;
    private readonly List<IDisposable> _itemSubscriptions = [];

    public IReadOnlyList<IntegrationCategoryItemVM> IntegrationCategories { get; }

    public ObservableCollectionExtended<IntegrationItemVM> RegisteredIntegrations { get; } = [];
    public ObservableCollectionExtended<IntegrationRegistrationVM> AvailableImplementations { get; } = [];

    [Reactive] public partial IntegrationCategoryItemVM? SelectedIntegrationCategory { get; set; }
    [Reactive] public partial IntegrationRegistration? SelectedIntegration { get; set; }

    /// <summary>True when any registered integration of the selected capability has an update available</summary>
    [Reactive] public partial bool HasUpdates { get; set; }

    public IntegrationsVM(
        IIntegrationProvider provider,
        IIntegrationRegistry registry,
        Func<string, Type, IntegrationCategoryItemVM> integrationTypeVMFactory,
        IConfigVMFactory configVMFactory,
        IIntegrationSourceManager sourceManager) {
        _provider = provider;
        _registry = registry;
        _configVMFactory = configVMFactory;
        _sourceManager = sourceManager;

        IntegrationCategories = provider.IntegrationDescriptors
            .GroupBy(d => d.CapabilityType)
            .Select(descriptor => integrationTypeVMFactory(descriptor.First().CategoryName, descriptor.Key))
            .ToArray();
        SelectedIntegrationCategory = IntegrationCategories.FirstOrDefault();

        this.WhenAnyValue(x => x.SelectedIntegrationCategory)
            .Subscribe(_ => Refresh())
            .DisposeWith(this);

        registry.IntegrationsChanged
            .Subscribe(_ => Refresh())
            .DisposeWith(this);

        Refresh();
    }

    private void Refresh() {
        foreach (var subscription in _itemSubscriptions) {
            subscription.Dispose();
        }
        _itemSubscriptions.Clear();

        RegisteredIntegrations.Clear();
        AvailableImplementations.Clear();

        if (SelectedIntegrationCategory is null) {
            HasUpdates = false;
            return;
        }

        var capabilityDescriptors = _provider.GetDescriptors(SelectedIntegrationCategory.CapabilityType)
            .Select(descriptor => descriptor.Id)
            .ToHashSet();

        foreach (var registration in _registry.IntegrationRegistrations.Where(i => capabilityDescriptors.Contains(i.IntegrationId))) {
            var descriptor = _provider.GetDescriptor(registration.IntegrationId);
            var item = new IntegrationItemVM(descriptor, registration, _registry, _sourceManager);
            _itemSubscriptions.Add(item
                .WhenAnyValue(x => x.UpdateAvailable)
                .Subscribe(_ => UpdateHasUpdates()));
            RegisteredIntegrations.Add(item);
        }

        foreach (var descriptor in _provider.GetDescriptors(SelectedIntegrationCategory.CapabilityType)) {
            if (RegisteredIntegrations.Any(i => i.Descriptor.Id == descriptor.Id)) continue;

            AvailableImplementations.Add(new IntegrationRegistrationVM(descriptor, _registry, _configVMFactory, _sourceManager));
        }

        UpdateHasUpdates();
    }

    private void UpdateHasUpdates() => HasUpdates = RegisteredIntegrations.Any(i => i.UpdateAvailable);

    [ReactiveCommand]
    private void UnregisterIntegration(IntegrationItemVM item) {
        _registry.Unregister(item.Descriptor.Id);
        if (SelectedIntegration?.IntegrationId == item.Descriptor.Id) SelectedIntegration = null;
    }

    /// <summary>Checks for and applies updates for all registered integrations with an available update.</summary>
    [ReactiveCommand]
    private async Task UpdateAllAsync() {
        // Snapshot: a successful update re-registers the integration, which refreshes the list
        foreach (var item in RegisteredIntegrations.Where(i => i.UpdateAvailable).ToArray()) {
            await item.UpdateCommand.Execute().FirstAsync();
        }
    }
}
