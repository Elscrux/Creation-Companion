using CreationEditor.Avalonia.ViewModels.Config;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Sources;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps an available (unregistered) integration descriptor with its own registration state,
/// so each "Add" card is a self-contained unit: edit the registration settings via the config
/// VM and confirm in one place, or install directly from one of the descriptor's sources.
/// The concrete integration is never instantiated here.
/// </summary>
public sealed partial class IntegrationRegistrationVM(
    IntegrationDescriptor descriptor,
    IIntegrationRegistry registry,
    IConfigVMFactory configVMFactory,
    IIntegrationSourceManager? sourceManager = null)
    : ViewModel {

    public IntegrationDescriptor Descriptor { get; } = descriptor;

    public string DisplayName => Descriptor.DisplayName;
    public string Description => Descriptor.Description;

    /// <summary>True if this integration has at least one declared source it can be installed from.</summary>
    public bool HasSource => Descriptor.Sources.Count > 0;

    /// <summary>Config VM editing this integration's registration settings</summary>
    public ConfigVM? SettingsVM { get; } = descriptor.RegistrationConfigDefinition is null ? null : configVMFactory.Create(descriptor.RegistrationConfigDefinition);

    /// <summary>True while the registration settings editor is shown for this descriptor</summary>
    [Reactive] public partial bool IsAdding { get; set; }

    /// <summary>True while a source install is in progress</summary>
    [Reactive] public partial bool IsBusy { get; set; }

    /// <summary>Status message for the source install (e.g. success or error details)</summary>
    [Reactive] public partial string? StatusMessage { get; set; }

    // Build the settings editor from the descriptor's default registration settings.

    /// <summary>Shows the registration settings editor for this descriptor</summary>
    [ReactiveCommand]
    private void BeginRegistration() {
        IsAdding = true;
    }

    /// <summary>Confirms the registration, applying the edited settings values</summary>
    [ReactiveCommand(CanExecute = nameof(CanConfirmRegistration))]
    private void ConfirmRegistration() {
        if (Descriptor.RegistrationConfigDefinition is null) return;

        registry.Register(new IntegrationRegistration {
            IntegrationId = Descriptor.Id,
            RegistrationConfig = SettingsVM?.GetConfigValues(),
        });

        IsAdding = false;
    }

    private bool CanConfirmRegistration() => IsAdding;

    /// <summary>Hides the registration settings editor without registering</summary>
    [ReactiveCommand]
    private void CancelRegistration() {
        IsAdding = false;
    }

    /// <summary>
    /// Downloads and installs the integration from its first available source, then registers it
    /// with the installed tool path merged into the edited registration settings.
    /// </summary>
    [ReactiveCommand(CanExecute = nameof(CanInstallFromSource))]
    private async Task InstallFromSourceAsync() {
        if (sourceManager is null) return;

        IsBusy = true;
        StatusMessage = null;
        try {
            var result = await sourceManager.InstallOrUpdateAsync(Descriptor, CancellationToken.None);
            StatusMessage = result switch {
                DownloadResult.Succeeded => "Installed successfully.",
                DownloadResult.Failed failed => $"Install failed: {failed.Message}",
                DownloadResult.ManualDownloadRequired manual => $"Manual download required: {manual.BrowserUrl}",
                _ => "Unknown install result.",
            };
        } catch (Exception e) {
            StatusMessage = $"Install failed: {e.Message}";
        } finally {
            IsBusy = false;
        }
    }

    private bool CanInstallFromSource() => HasSource && !IsBusy && sourceManager is not null;
}
