using System.Diagnostics;
using System.IO.Abstractions;
namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Handles the manual browser-download flow for sources that cannot download directly (e.g.
/// Nexus Mods non-premium). Opens the browser to the given URL, then polls the download folder
/// for the expected file until it appears or a timeout elapses.
/// </summary>
public sealed class ManualDownloadHandler(IFileSystem fileSystem) {
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Opens <paramref name="browserUrl"/> in the default browser and polls the download folder
    /// for <paramref name="expectedFileName"/>. Returns the full path to the downloaded file when
    /// found, or null if the timeout elapsed.
    /// </summary>
    public async Task<string?> WaitForManualDownloadAsync(
        string browserUrl,
        string expectedFileName,
        string? downloadFolder = null,
        CancellationToken token = default) {
        OpenBrowser(browserUrl);

        var folder = downloadFolder ?? GetDefaultDownloadFolder();
        if (folder is null) {
            return null;
        }

        var deadline = DateTime.UtcNow + DefaultTimeout;
        while (DateTime.UtcNow < deadline) {
            token.ThrowIfCancellationRequested();

            var candidate = fileSystem.Path.Combine(folder, expectedFileName);
            if (fileSystem.File.Exists(candidate)) {
                return candidate;
            }

            // Also match partial downloads (e.g. ".crdownload" / ".part") so we wait for completion.
            var partial = fileSystem.Directory
                .GetFiles(folder, $"{expectedFileName}.*")
                .FirstOrDefault();
            if (partial is null) {
                await Task.Delay(PollInterval, token);
                continue;
            }

            // A partial file exists; wait for it to be replaced by the final file.
            await Task.Delay(PollInterval, token);
        }

        return null;
    }

    private void OpenBrowser(string url) {
        Process.Start(new ProcessStartInfo {
            FileName = url,
            UseShellExecute = true,
            Verb = "open",
        });
    }

    private string? GetDefaultDownloadFolder() {
        var downloads = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        return downloads is null ? null : fileSystem.Path.Combine(downloads, "Downloads");
    }
}
