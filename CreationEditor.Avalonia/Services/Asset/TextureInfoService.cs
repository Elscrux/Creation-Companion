using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.ViewModels.Setting.AssetBrowser;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Textures;
using CreationEditor.Services.Settings;
using Noggog;
using ReactiveUI;
using Serilog;
namespace CreationEditor.Avalonia.Services.Asset;

public sealed class TextureInfoService : ITextureInfoService, IDisposableDropoff {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IIntegrationRegistry _integrationRegistry;
    private readonly AssetBrowserSettingVM? _settingVM;
    private readonly DisposableBucket _disposables = new();

    private readonly ConcurrentDictionary<DataSourceFileLink, (DateTimeOffset WriteTime, TextureInfo TextureInfo)> _cache = new();
    private readonly Subject<Unit> _providerChanged = new();

    public ITextureInfoProvider? Provider { get; private set; }
    public IObservable<Unit> ProviderChanged => _providerChanged;

    public TextureInfoService(
        ILogger logger,
        IFileSystem fileSystem,
        ISettingProvider settingProvider,
        IIntegrationRegistry integrationRegistry) {
        _logger = logger;
        _fileSystem = fileSystem;
        _integrationRegistry = integrationRegistry;
        _settingVM = settingProvider.Settings.OfType<AssetBrowserSettingVM>().FirstOrDefault();

        _settingVM?.WhenAnyValue(x => x.Setting.TextureInfoIntegrationId)
            .Subscribe(_ => UpdateProvider())
            .DisposeWith(_disposables);

        _integrationRegistry.IntegrationsChanged
            .Subscribe(_ => UpdateProvider())
            .DisposeWith(_disposables);

        UpdateProvider();
    }

    private void UpdateProvider() {
        var integrationId = _settingVM?.Setting.TextureInfoIntegrationId;
        Provider = (integrationId is not null
                ? _integrationRegistry.TryGetIntegration<ITextureInfoProvider>(integrationId.Value)
                : null)
         ?? _integrationRegistry.GetIntegrationsForCapability(typeof(ITextureInfoProvider))
                .OfType<ITextureInfoProvider>()
                .FirstOrDefault();

        // Cached results may belong to a different provider or a changed tool setup
        _cache.Clear();

        _providerChanged.OnNext(Unit.Default);
    }

    public TextureInfo? GetCached(DataSourceFileLink fileLink) {
        var writeTime = GetWriteTime(fileLink);
        return _cache.TryGetValue(fileLink, out var cached) && cached.WriteTime == writeTime
            ? cached.TextureInfo
            : null;
    }

    public async Task<TextureInfo?> GetTextureInfo(DataSourceFileLink fileLink, CancellationToken token = default) {
        if (Provider is null) return null;

        var writeTime = GetWriteTime(fileLink);
        if (_cache.TryGetValue(fileLink, out var cached) && cached.WriteTime == writeTime) return cached.TextureInfo;

        var result = await Provider.GetTextureInfo(fileLink.FullPath, token);
        if (result.Infos != null && result.Infos.TryGetValue(fileLink.FullPath, out var textureInfo)) {
            _cache[fileLink] = (writeTime, textureInfo);
            return textureInfo;
        }

        return null;
    }

    public async Task Prefetch(IDataSource dataSource, CancellationToken token = default) {
        if (Provider is null) return;

        var result = await Provider.GetTextureInfo(dataSource.FileSystem.Path.Combine(dataSource.Path, "*.dds"), token);
        if (result.Infos is null) return;

        foreach (var (path, textureInfo) in result.Infos) {
            try {
                var dataRelativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, path);
                var fileLink = new DataSourceFileLink(dataSource, dataRelativePath);
                var writeTime = GetWriteTime(fileLink);
                _cache[fileLink] = (writeTime, textureInfo);
            } catch (Exception e) {
                _logger.Here().Warning(e, "Failed to cache texture info for {Path}", path);
            }
        }
    }

    private DateTimeOffset GetWriteTime(DataSourceFileLink fileLink) {
        try {
            var file = _fileSystem.FileInfo.New(fileLink.FullPath);
            return file.Exists ? file.LastWriteTimeUtc : DateTimeOffset.MinValue;
        } catch {
            return DateTimeOffset.MinValue;
        }
    }

    public void Add(IDisposable disposable) => _disposables.Add(disposable);
    public void Dispose() => _disposables.Dispose();
}
