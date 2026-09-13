using CreationEditor.Services.Integrations;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps an available (unconfigured) integration implementation with its own install-path state,
/// so each "Add" card is a self-contained unit: pick the install path and confirm in one place.
/// </summary>
public sealed partial class AvailableIntegrationVM : ViewModel {
    private readonly IIntegrationRegistry _registry;

    public IIntegration Implementation { get; }

    public string DisplayName => Implementation.DisplayName;
    public string Description => Implementation.Description;

    /// <summary>True while the install-path picker is shown for this implementation.</summary>
    [Reactive] public partial bool IsAdding { get; set; }

    /// <summary>The install path for the tool executable.</summary>
    [Reactive] public partial string InstallPath { get; set; } = string.Empty;

    /// <summary>True when the install path is set, so the integration can be added.</summary>
    [Reactive] public partial bool CanAdd { get; set; }

    public AvailableIntegrationVM(IIntegration implementation, IIntegrationRegistry registry) {
        Implementation = implementation;
        _registry = registry;

        this.WhenAnyValue(x => x.InstallPath)
            .Subscribe(path => CanAdd = !string.IsNullOrWhiteSpace(path))
            .DisposeWith(this);
    }

    /// <summary>Shows the install-path picker for this implementation.</summary>
    [ReactiveCommand]
    private void BeginAdd() {
        InstallPath = string.Empty;
        IsAdding = true;
    }

    /// <summary>Confirms the add, requiring the install path to be set.</summary>
    [ReactiveCommand(CanExecute = nameof(CanConfirmAdd))]
    private void ConfirmAdd() {
        if (string.IsNullOrWhiteSpace(InstallPath)) return;

        var integration = new RegisteredIntegration {
            ImplementationId = Implementation.Id,
            InstallPath = InstallPath,
            Config = Implementation.CreateDefaultConfig(),
        };

        _registry.Save(integration);
        IsAdding = false;
    }

    private bool CanConfirmAdd() => IsAdding && !string.IsNullOrWhiteSpace(InstallPath);

    /// <summary>Hides the install-path picker without adding.</summary>
    [ReactiveCommand]
    private void CancelAdd() {
        IsAdding = false;
    }
}
