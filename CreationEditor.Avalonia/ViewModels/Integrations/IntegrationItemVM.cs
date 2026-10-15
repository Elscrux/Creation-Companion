using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Sources;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps a registered integration with its descriptor and registration, exposing display info
/// and update actions (check for update / update) for the Integrations window cards.
/// </summary>
public sealed partial class IntegrationItemVM : ViewModel {
    private readonly IntegrationRegistration? _registration;
    private readonly IIntegrationSourceManager? _sourceManager;
    private readonly IIntegrationRegistry _registry;

    public IntegrationDescriptor Descriptor { get; }

    /// <summary>Name of the source this integration was installed/updated from, or null if installed manually.</summary>
    public string? SourceName => _registration?.SourceName;

    /// <summary>The installed version, or null if unknown.</summary>
    public string? InstalledVersion => _registration?.InstalledVersion;

    /// <summary>True if this integration has at least one declared source it can be updated from.</summary>
    public bool HasSource => Descriptor.Sources.Count > 0;

    /// <summary>True while an update check or update is in progress.</summary>
    [Reactive] public partial bool IsBusy { get; set; }

    /// <summary>Status message shown to the user (e.g. "Up to date", "Update available", errors).</summary>
    [Reactive] public partial string? StatusMessage { get; set; }

    /// <summary>The latest version reported by the integration's source, or null if unknown.</summary>
    [Reactive] public partial string? LatestVersion { get; set; }

    /// <summary>True when the source reports a version different from the installed one.</summary>
    [Reactive] public partial bool UpdateAvailable { get; set; }

    public IntegrationItemVM(
        IntegrationDescriptor descriptor,
        IntegrationRegistration? registration,
        IIntegrationRegistry registry,
        IIntegrationSourceManager? sourceManager = null) {
        Descriptor = descriptor;
        _registration = registration;
        _registry = registry;
        _sourceManager = sourceManager;

        // Check the source right away so the update button only shows when an update is available
        if (HasSource && _sourceManager is not null) {
            _ = CheckForUpdateAsync();
        }
    }

    /// <summary>Checks the integration's sources for a newer version.</summary>
    [ReactiveCommand(CanExecute = nameof(CanCheckForUpdate))]
    private async Task CheckForUpdateAsync() {
        if (_sourceManager is null || _registration is null) return;

        IsBusy = true;
        StatusMessage = null;
        try {
            var result = await _sourceManager.CheckForUpdateAsync(Descriptor, CancellationToken.None);
            if (result is null) {
                StatusMessage = "No sources configured.";
            } else if (result.LatestVersion is null) {
                LatestVersion = null;
                StatusMessage = result.Message ?? "Could not determine latest version.";
            } else {
                LatestVersion = result.LatestVersion;
                StatusMessage = _registration.InstalledVersion == result.LatestVersion
                    ? $"Up to date ({result.LatestVersion})."
                    : $"Update available: {result.LatestVersion}.";
            }
            UpdateAvailable = HasSource && LatestVersion is not null && _registration.InstalledVersion != LatestVersion;
        } catch (Exception e) {
            StatusMessage = $"Update check failed: {e.Message}";
        } finally {
            IsBusy = false;
        }
    }

    /// <summary>Downloads and installs (or updates) the integration from its source.</summary>
    [ReactiveCommand(CanExecute = nameof(CanCheckForUpdate))]
    private async Task UpdateAsync() {
        if (_sourceManager is null) return;

        IsBusy = true;
        StatusMessage = null;
        try {
            var result = await _sourceManager.InstallOrUpdateAsync(Descriptor, CancellationToken.None);
            StatusMessage = result switch {
                DownloadResult.Succeeded => "Installed/updated successfully.",
                DownloadResult.Failed failed => $"Update failed: {failed.Message}",
                DownloadResult.ManualDownloadRequired manual => $"Manual download required: {manual.BrowserUrl}",
                _ => "Unknown result.",
            };

            // Record the installed version so the update button disappears until the next release
            if (result is DownloadResult.Succeeded && LatestVersion is not null && _registration is not null) {
                _registration.InstalledVersion = LatestVersion;
                _registry.Register(_registration);
            }
        } catch (Exception e) {
            StatusMessage = $"Update failed: {e.Message}";
        } finally {
            IsBusy = false;
        }
    }

    private bool CanCheckForUpdate() => HasSource && !IsBusy && _sourceManager is not null;
}
