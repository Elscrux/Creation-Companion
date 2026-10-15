using System.Reactive;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Integrations.Textures;
namespace CreationEditor.Avalonia.Services.Asset;

/// <summary>
/// Provides texture information via the configured <see cref="ITextureInfoProvider" /> integration.
/// </summary>
public interface ITextureInfoService {
    /// <summary>The currently configured texture info provider, or null if none is available.</summary>
    ITextureInfoProvider? Provider { get; }

    /// <summary>Emits whenever the configured provider changes.</summary>
    IObservable<Unit> ProviderChanged { get; }

    /// <summary>Returns the texture info for a file.</summary>
    TextureInfo? GetCached(DataSourceFileLink fileLink);

    /// <summary>Returns the texture info for a file, using the cache when possible.</summary>
    Task<TextureInfo?> GetTextureInfo(DataSourceFileLink fileLink, CancellationToken token = default);

    /// <summary>Fetches texture info for all files in the data source, caching the results.</summary>
    Task Prefetch(IDataSource dataSource, CancellationToken token = default);
}
