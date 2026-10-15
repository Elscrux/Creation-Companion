using CreationEditor.Services.Integrations;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps an integration capability type with its display category name and the number of
/// registered integrations for that capability.
/// </summary>
public sealed partial class IntegrationCategoryItemVM : ViewModel {
    private readonly IIntegrationProvider _provider;
    private readonly IIntegrationRegistry _registry;

    public string CategoryName { get; }
    public Type CapabilityType { get; }

    [Reactive] public partial int RegisteredCount { get; set; }

    /// <summary>True when at least one integration is registered for this capability</summary>
    public bool HasRegistrations => RegisteredCount > 0;

    public IntegrationCategoryItemVM(
        string categoryName,
        Type capabilityType,
        IIntegrationProvider provider,
        IIntegrationRegistry registry) {
        CategoryName = categoryName;
        CapabilityType = capabilityType;
        _provider = provider;
        _registry = registry;

        RefreshCount();

        registry.IntegrationsChanged
            .Subscribe(_ => RefreshCount())
            .DisposeWith(this);
    }

    private void RefreshCount() {
        RegisteredCount = _registry.IntegrationRegistrations
            .Count(i => _provider.GetDescriptor(i.IntegrationId).CategoryName == CategoryName);
        this.RaisePropertyChanged(nameof(HasRegistrations));
    }
}
