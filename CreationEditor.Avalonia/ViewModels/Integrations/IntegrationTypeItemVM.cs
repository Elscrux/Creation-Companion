using CreationEditor.Services.Integrations;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps an integration capability type with its display category name and the number of
/// configured integrations for that capability.
/// </summary>
public sealed partial class IntegrationTypeItemVM : ViewModel {
    private readonly IIntegrationProvider _provider;
    private readonly IIntegrationRegistry _registry;

    public Type IntegrationType { get; }
    public string CategoryName { get; }

    /// <summary>Number of configured integrations for this capability.</summary>
    [Reactive] public partial int ConfiguredCount { get; set; }

    public IntegrationTypeItemVM(
        Type integrationType,
        string categoryName,
        IIntegrationProvider provider,
        IIntegrationRegistry registry) {
        IntegrationType = integrationType;
        CategoryName = categoryName;
        _provider = provider;
        _registry = registry;

        RefreshCount();

        registry.Changed
            .Subscribe(_ => RefreshCount())
            .DisposeWith(this);
    }

    private void RefreshCount() {
        var implementations = _provider.GetImplementations(IntegrationType)
            .Select(impl => impl.Id)
            .ToHashSet();

        ConfiguredCount = _registry.Integrations.Count(i => implementations.Contains(i.ImplementationId));
    }
}
