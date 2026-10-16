using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using CreationEditor.Services.UserInput;
using ktsu.CredentialCache;
using ktsu.Semantics.Strings;
using Serilog;
using CredentialCache = ktsu.CredentialCache.CredentialCache;

namespace CreationEditor.Services.Integrations.Sources.NexusMods;

/// <summary>
/// An <see cref="IIntegrationSource"/> that queries the Nexus Mods API v3 for an integration's
/// latest version and downloads its files. This is a general service: the specific mod to query
/// is supplied per-operation via the strongly-typed <see cref="NexusModsIntegrationSourceDescriptor"/>.
/// Requires an API key stored in the OS keychain (via <c>ktsu.CredentialCache</c>, service
/// <c>CreationCompanion</c>); when none is found, the user is prompted to enter one via
/// <see cref="IUserInputService"/>, and the entered key is stored in the keychain. If the API
/// refuses the download, the source falls back to a manual browser download.
/// </summary>
public sealed class NexusModsSource(
    HttpClient httpClient,
    CredentialCache credentialCache,
    IUserInputService userInputService,
    ILogger logger) : IIntegrationSource {
    private const string ApiBase = "https://api.nexusmods.com/v3";
    private const string ApplicationName = "CreationCompanion";

    /// <summary>Stable persona GUID identifying this app's Nexus Mods API key in the keyring.</summary>
    private static readonly PersonaGUID ApiKeyPersona = SemanticString<PersonaGUID>.Create("5cb8e0a7-91d1-4c34-9c30-8f4b3c2e6a11");

    /// <summary>Version categories that contain downloadable versions of a mod file.</summary>
    private static readonly string[] ActiveCategories = ["main", "update", "optional", "miscellaneous"];

    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true,
    };

    public string Name => "Nexus Mods";

    public Task<LatestVersionResult> GetLatestVersionAsync(IIntegrationSourceDescriptor sourceDescriptor, CancellationToken token)
        => GetLatestVersionAsync((NexusModsIntegrationSourceDescriptor) sourceDescriptor, token);

    public async Task<LatestVersionResult> GetLatestVersionAsync(NexusModsIntegrationSourceDescriptor sourceDescriptor, CancellationToken token) {
        var apiKey = await GetApiKeyAsync(token);
        if (apiKey is null) {
            return new LatestVersionResult(null, "No Nexus Mods API key configured. Add one to the OS keychain.");
        }

        var versions = await GetActiveVersionsAsync(sourceDescriptor, apiKey, token);
        var latest = SelectLatestVersion(versions);
        if (latest?.Version is null) {
            return new LatestVersionResult(null, "Could not determine the latest version for this mod.");
        }

        return new LatestVersionResult(latest.Version, null);
    }

    public Task<DownloadResult> DownloadAsync(
        IIntegrationSourceDescriptor sourceDescriptor,
        string version,
        string destinationPath,
        CancellationToken token)
        => DownloadAsync((NexusModsIntegrationSourceDescriptor) sourceDescriptor, version, destinationPath, token);

    public async Task<DownloadResult> DownloadAsync(
        NexusModsIntegrationSourceDescriptor sourceDescriptor,
        string version,
        string destinationPath,
        CancellationToken token) {
        var apiKey = await GetApiKeyAsync(token);
        if (apiKey is null) {
            return new DownloadResult.Failed("No Nexus Mods API key configured. Add one to the OS keychain.");
        }

        var versions = await GetActiveVersionsAsync(sourceDescriptor, apiKey, token);
        if (versions is null || versions.Count == 0) {
            return new DownloadResult.Failed("Could not find any downloadable files for this mod.");
        }

        // Select the requested version, falling back to the latest version when no version was given.
        var fileVersion = string.IsNullOrWhiteSpace(version)
            ? null
            : versions.Find(v => string.Equals(v.Version, version, StringComparison.OrdinalIgnoreCase));
        fileVersion ??= SelectLatestVersion(versions);

        if (fileVersion is null || string.IsNullOrEmpty(fileVersion.Id) || string.IsNullOrEmpty(fileVersion.Name)) {
            return new DownloadResult.Failed($"No downloadable file found for version '{version}'.");
        }

        var (downloadUrl, forbidden) = await GetRepackedDownloadLinkAsync(fileVersion.Id, apiKey, token);
        if (forbidden) {
            // The API refused the download (e.g. the account has no access); let the user download the file manually through the browser instead.
            return new DownloadResult.ManualDownloadRequired(
                $"https://www.nexusmods.com/{sourceDescriptor.GameId}/mods/{sourceDescriptor.ModId}?tab=files",
                fileVersion.Name);
        }

        if (string.IsNullOrEmpty(downloadUrl)) {
            return new DownloadResult.Failed($"Nexus Mods did not return a download link for '{fileVersion.Name}'.");
        }

        try {
            using var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            var destinationFile = Path.Combine(destinationPath, Path.GetFileName(GetDestinationFileName(fileVersion.Name, response, downloadUrl)));

            await using var sourceStream = await response.Content.ReadAsStreamAsync(token);
            await using var targetStream = File.Create(destinationFile);
            await sourceStream.CopyToAsync(targetStream, token);

            logger.Here().Information("Downloaded {File} (version {Version}) from Nexus Mods",
                Path.GetFileName(destinationFile),
                fileVersion.Version);
            return new DownloadResult.Succeeded(destinationFile);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to download {File} from Nexus Mods", fileVersion.Name);
            return new DownloadResult.Failed(e.Message);
        }
    }

    private async Task<string?> GetApiKeyAsync(CancellationToken token) {
        var storedKey = TryGetStoredApiKey();
        if (storedKey is not null) {
            return storedKey;
        }

        // No key in the credential store: ask the user to enter one.
        var enteredKey = await userInputService.RequestTextAsync(
            "Nexus Mods API Key",
            "No Nexus Mods API key was found on this machine.\n"
          + "Enter your personal API key (from https://www.nexusmods.com/settings/api-keys):",
            token: token);
        if (string.IsNullOrWhiteSpace(enteredKey)) {
            return null;
        }

        // Store the entered key so the user is only asked once.
        try {
            credentialCache.AddOrReplace(ApiKeyPersona,
                new CredentialWithToken {
                    Token = SemanticString<CredentialToken>.Create(enteredKey),
                });
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to store the Nexus Mods API key in the credential cache");
        }

        return enteredKey;
    }

    private string? TryGetStoredApiKey() {
        try {
            if (credentialCache.TryGet(ApiKeyPersona, out var credential)
             && credential is CredentialWithToken tokenCredential) {
                var apiKey = tokenCredential.Token.WeakString;
                if (!string.IsNullOrWhiteSpace(apiKey)) {
                    return apiKey;
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to read the Nexus Mods API key from the credential cache");
        }

        return null;
    }

    private static ModFileVersion? SelectLatestVersion(List<ModFileVersion>? versions) {
        return versions?
            .OrderByDescending(v => v.UploadedAt)
            .ThenByDescending(v => v.IsPrimary)
            .FirstOrDefault();
    }


    /// <summary>
    /// Resolves the active versions of the mod: gets the mod's global id, finds its most recently
    /// updated active mod file, and returns that file's versions in active categories.
    /// </summary>
    private async Task<List<ModFileVersion>?> GetActiveVersionsAsync(
        NexusModsIntegrationSourceDescriptor sourceDescriptor,
        string apiKey,
        CancellationToken token) {
        var gameId = sourceDescriptor.GameId;
        var modId = sourceDescriptor.ModId;

        // The files/versions endpoints use the mod's global (not game-scoped) id, so resolve it first.
        var mod = await GetJsonAsync<DataResponse<Mod>>(Url(gameId, modId), apiKey, token);
        if (string.IsNullOrEmpty(mod?.Data.Id)) {
            return null;
        }

        var files = await GetJsonAsync<DataResponse<ModFiles>>($"{ApiBase}/mods/{mod.Data.Id}/files", apiKey, token);
        var modFile = files?.Data.ModFileList
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.LastFileUploadedAt)
            .FirstOrDefault();
        if (string.IsNullOrEmpty(modFile?.Id)) {
            return null;
        }

        var versions = await GetJsonAsync<DataResponse<ModFileVersions>>($"{ApiBase}/mod-files/{modFile.Id}/versions", apiKey, token);
        return versions?.Data.Versions
            .Where(v => ActiveCategories.Contains(v.Category, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Requests a repacked download link for a mod file version via
    /// <c>POST /mod-file-versions/{id}/download-repacked</c>. Returns the download URL, or null
    /// when the request failed; <paramref name="forbidden"/> indicates a 403 (access denied).
    /// </summary>
    private async Task<(string? DownloadUrl, bool Forbidden)> GetRepackedDownloadLinkAsync(string versionId, string apiKey, CancellationToken token) {
        var url = $"{ApiBase}/mod-file-versions/{versionId}/download-repacked";
        try {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("apikey", apiKey);
            request.Headers.Add("Application-Name", ApplicationName);

            using var response = await httpClient.SendAsync(request, token);
            if (response.StatusCode == HttpStatusCode.Forbidden) {
                logger.Here().Warning("Nexus Mods refused the download of mod file version {VersionId} (403)", versionId);
                return (null, true);
            }

            var body = await ReadJsonAsync<DownloadRepacked>(response, url, token);
            return (body?.DownloadUrl, false);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to request a download link from Nexus Mods at {Url}", url);
            return (null, false);
        }
    }

    /// <summary>
    /// Determines the destination file name. Uses the mod file version's name when it carries an
    /// extension; otherwise derives the extension from the CDN's Content-Disposition header or the
    /// name parameter of the download URL (e.g. 'BSArch' -> 'BSArch.zip').
    /// </summary>
    private static string GetDestinationFileName(string versionName, HttpResponseMessage response, string downloadUrl) {
        if (Path.HasExtension(versionName)) {
            return versionName;
        }

        var disposition = response.Content.Headers.ContentDisposition?.FileNameStar
         ?? response.Content.Headers.ContentDisposition?.FileName;
        if (!string.IsNullOrWhiteSpace(disposition)) {
            var extension = Path.GetExtension(disposition.Trim('"'));
            if (!string.IsNullOrEmpty(extension)) {
                return versionName + extension;
            }
        }

        if (Uri.TryCreate(downloadUrl, UriKind.Absolute, out var uri)) {
            var nameParameter = uri.Query.TrimStart('?')
                .Split('&')
                .Select(p => p.Split('=', 2))
                .FirstOrDefault(p => p.Length == 2 && p[0] == "name")?[1];
            if (!string.IsNullOrWhiteSpace(nameParameter)) {
                var extension = Path.GetExtension(Uri.UnescapeDataString(nameParameter));
                if (!string.IsNullOrEmpty(extension)) {
                    return versionName + extension;
                }
            }
        }

        return versionName;
    }

    private static string Url(string gameId, long modId) => $"{ApiBase}/games/{gameId}/mods/{modId}";

    private async Task<T?> GetJsonAsync<T>(string url, string apiKey, CancellationToken token) {
        try {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("apikey", apiKey);

            using var response = await httpClient.SendAsync(request, token);
            return await ReadJsonAsync<T>(response, url, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to query Nexus Mods API at {Url}", url);
            return default;
        }
    }

    private async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response, string url, CancellationToken token) {
        if (!response.IsSuccessStatusCode) {
            logger.Here().Warning("Nexus Mods API returned {Status} for {Url}", (int) response.StatusCode, url);
            return default;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, token);
    }

    private sealed class DataResponse<T> {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    }

    private sealed class Mod {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("game_scoped_id")]
        public string GameScopedId { get; set; } = "";

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    private sealed class ModFiles {
        [JsonPropertyName("mod_files")]
        public List<ModFile> ModFileList { get; set; } = [];
    }

    private sealed class ModFile {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("last_file_uploaded_at")]
        public DateTime? LastFileUploadedAt { get; set; }
    }

    private sealed class ModFileVersions {
        [JsonPropertyName("versions")]
        public List<ModFileVersion> Versions { get; set; } = [];
    }

    private sealed class ModFileVersion {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("uploaded_at")]
        public DateTime UploadedAt { get; set; }

        [JsonPropertyName("is_primary")]
        public bool IsPrimary { get; set; }
    }

    private sealed class DownloadRepacked {
        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; } = "";

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
