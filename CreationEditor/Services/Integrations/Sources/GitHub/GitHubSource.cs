using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Serilog;
namespace CreationEditor.Services.Integrations.Sources.GitHub;

/// <summary>
/// An <see cref="IIntegrationSource"/> that queries the GitHub Releases API for an integration's
/// latest version and downloads its release assets. This is a general service: the specific
/// repository to query is supplied per-operation via the strongly-typed <see cref="GitHubIntegrationSourceDescriptor"/>.
/// </summary>
public sealed class GitHubSource(
    HttpClient httpClient,
    ILogger logger) : IIntegrationSource {
    private const string ApiBase = "https://api.github.com";

    public string Name => "GitHub";

    public Task<LatestVersionResult> GetLatestVersionAsync(IIntegrationSourceDescriptor sourceDescriptor, CancellationToken token)
        => GetLatestVersionAsync((GitHubIntegrationSourceDescriptor) sourceDescriptor, token);

    public async Task<LatestVersionResult> GetLatestVersionAsync(GitHubIntegrationSourceDescriptor integrationSourceDescriptor, CancellationToken token) {
        var release = await GetLatestReleaseAsync(integrationSourceDescriptor.Owner, integrationSourceDescriptor.Repo, token);
        if (release is null) {
            return new LatestVersionResult(null, "No release found.");
        }

        return new LatestVersionResult(release.TagName, null);
    }

    public Task<DownloadResult> DownloadAsync(IIntegrationSourceDescriptor sourceDescriptor, string version, string destinationPath, CancellationToken token)
        => DownloadAsync((GitHubIntegrationSourceDescriptor) sourceDescriptor, version, destinationPath, token);

    public async Task<DownloadResult> DownloadAsync(
        GitHubIntegrationSourceDescriptor integrationSourceDescriptor,
        string version,
        string destinationPath,
        CancellationToken token) {
        var release = await GetLatestReleaseAsync(integrationSourceDescriptor.Owner, integrationSourceDescriptor.Repo, token);
        if (release is null) {
            return new DownloadResult.Failed("No release found.");
        }

        var asset = SelectAsset(release, integrationSourceDescriptor);
        if (asset is null || string.IsNullOrEmpty(asset.Name) || string.IsNullOrEmpty(asset.BrowserDownloadUrl)) {
            return new DownloadResult.Failed($"Release '{release.TagName}' has no downloadable assets.");
        }

        var destinationFile = Path.Combine(destinationPath, asset.Name);
        try {
            using var response = await httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            await using var sourceStream = await response.Content.ReadAsStreamAsync(token);
            await using var targetStream = File.Create(destinationFile);
            await sourceStream.CopyToAsync(targetStream, token);

            logger.Here().Information("Downloaded {Asset} from GitHub release {Tag}", asset.Name, release.TagName);
            return new DownloadResult.Succeeded(destinationFile);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to download {Asset} from GitHub", asset.Name);
            return new DownloadResult.Failed(e.Message);
        }
    }

    private static GitHubAsset? SelectAsset(GitHubRelease release, GitHubIntegrationSourceDescriptor descriptor) {
        var pattern = descriptor.AssetNamePattern;
        if (string.IsNullOrWhiteSpace(pattern)) return release.Assets.FirstOrDefault();

        return release.Assets.Find(asset => asset.Name is not null && MatchesPattern(asset.Name, pattern));
    }

    /// <summary>Case-insensitive wildcard match supporting * and ?.</summary>
    private static bool MatchesPattern(string name, string pattern) {
        var regex = new Regex(
            "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
            RegexOptions.IgnoreCase);
        return regex.IsMatch(name);
    }

    private async Task<GitHubRelease?> GetLatestReleaseAsync(string owner, string repo, CancellationToken token) {
        var url = $"{ApiBase}/repos/{owner}/{repo}/releases/latest";
        try {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("CreationCompanion", "1.0"));

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            if (!response.IsSuccessStatusCode) {
                logger.Here().Warning("GitHub API returned {Status} for {Url}", response.StatusCode, url);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(token);
            return await JsonSerializer.DeserializeAsync<GitHubRelease>(stream,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                },
                token);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to query GitHub API at {Url}", url);
            return null;
        }
    }

    private sealed class GitHubRelease {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("assets")]
        public List<GitHubAsset> Assets { get; set; } = [];
    }

    private sealed class GitHubAsset {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? BrowserDownloadUrl { get; set; }
    }
}
