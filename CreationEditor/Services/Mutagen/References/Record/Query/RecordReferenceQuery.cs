using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Record.Query;

/// <summary>
/// RecordReferenceQuery caches mod references to achieve quick access times for references instead of iterating through contained form links all the time.
/// </summary>
public sealed class RecordReferenceQuery(
    Func<string[], ICacheLocationProvider> cacheLocationProviderFactory,
    IDataSourceService dataSourceService,
    IFileSystem fileSystem,
    IMutagenTypeProvider mutagenTypeProvider,
    INotificationService notificationService,
    IModInfoProvider<IModGetter> modInfoProvider,
    ILogger logger)
    : IRecordReferenceQuery, IDisposableDropoff {
    private static readonly string[] CacheLocation = ["References", "Record"];

    private static readonly Version Version = new(2, 0);

    private readonly DisposableBucket _disposables = new();
    private readonly ICacheLocationProvider _cacheLocationProvider = cacheLocationProviderFactory(CacheLocation);

    public IReadOnlyDictionary<ModKey, ModReferenceCache> ModCaches => _modCaches;
    private readonly Dictionary<ModKey, ModReferenceCache> _modCaches = new();

    private bool TryGetCacheFileName(IModGetter mod, out string cacheFilePath) {
        // Get mod path
        var modKey = mod.ModKey;
        var link = dataSourceService.GetFileLink(new DataRelativePath(modKey.FileName));
        if (link is null || !link.Exists()) {
            cacheFilePath = string.Empty;
            return false;
        }

        // Create a unique cache file path for game, cache version, mod filename and mod hash
        // Example: Skyrim/v2.0/NewMod.esp_4B0B3420E493A066.cache
        var gameName = mutagenTypeProvider.GetGameName(mod);
        var modHash = link.FileSystem.GetFileHash(link.FullPath);
        cacheFilePath = _cacheLocationProvider.CacheFile(gameName, $"v{Version}", $"{modKey.FileName}_{Convert.ToHexString(modHash)}");

        return true;
    }
    private string GetGameName(string cacheFilePath) =>
        cacheFilePath.Split(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar)[^3];

    public void Dispose() => _disposables.Dispose();

    public void Add(IDisposable disposable) => _disposables.Add(disposable);

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mod">mod to load references for</param>
    public void LoadModReferences(IModGetter mod) {
        if (_modCaches.ContainsKey(mod.ModKey)) return;

        logger.Here().Debug("Starting to load Record References of {ModKey}", mod.ModKey);

        ModReferenceCache modReferenceCache;
        if (TryGetCacheFileName(mod, out var cacheFilePath)) {
            if (fileSystem.File.Exists(cacheFilePath)) {
                // If cache file exists, try to load the cache
                try {
                    logger.Here().Verbose(
                        "Record Reference cache file found at {CacheFile}, trying to load the cache for {ModKey}",
                        cacheFilePath,
                        mod.ModKey);
                    modReferenceCache = LoadReferenceCache(cacheFilePath);
                } catch (Exception e) {
                    // Catch any issues while loading references
                    logger.Here().Error(e, "Loading Record References for {ModKey} failed: {Message}", mod.ModKey, e.Message);
                    logger.Here().Debug("Try to generate Record References for {ModKey} again", mod.ModKey);

                    // Delete broken cache
                    TryDeleteCache(cacheFilePath);

                    // Try again
                    modReferenceCache = BuildReferenceCache(mod);
                    WriteReferenceCache(modReferenceCache, cacheFilePath);
                }
            } else {
                // If cache file at path doesn't exist, build the cache and write it#
                logger.Here().Verbose("No Record Reference cache file found at {CacheFile}, build the cache for {ModKey}", cacheFilePath, mod.ModKey);
                modReferenceCache = BuildReferenceCache(mod);
                WriteReferenceCache(modReferenceCache, cacheFilePath);
            }
        } else {
            // If cache file path could not be generated, just build the cache
            logger.Here().Verbose("Could not get Record Reference cache file path for {ModKey}, just build the cache", mod.ModKey);
            modReferenceCache = BuildReferenceCache(mod);
        }

        _modCaches.Add(mod.ModKey, modReferenceCache);
        logger.Here().Debug("Finished loading Record References of {ModKey}", mod.ModKey);
    }

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mods">enumerable of mods to load references for</param>
    public async Task LoadModReferences(IReadOnlyList<IModGetter> mods) {
        if (mods.All(mod => _modCaches.ContainsKey(mod.ModKey))) return;

        var notify = new LinearNotifier(notificationService, mods.Count);

        await Task.WhenAll(mods.Select(m => Task.Run(() => LoadModReferences(m))));

        foreach (var mod in mods) {
            notify.Next($"Loading Record References in {mod.ModKey}");
            LoadModReferences(mod);
        }
        notify.Stop();
    }

    private void TryDeleteCache(string cacheFilePath) {
        if (!fileSystem.File.Exists(cacheFilePath)) return;

        try {
            fileSystem.File.Delete(cacheFilePath);
        } catch (Exception e) {
            logger.Here().Warning(e, "Trying to delete cache file {CacheFile} failed: {Message}", cacheFilePath, e.Message);
        }
    }

    /// <summary>
    /// Builds reference cache of a mod
    /// </summary>
    /// <param name="mod">mod to build reference cache for</param>
    /// <returns>reference cache of mod</returns>
    public ModReferenceCache BuildReferenceCache(IModGetter mod) {
        // Fill modCache
        var modCache = new ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>>();
        var records = new HashSet<FormKey>();

        using (var counter = new CountingNotifier(notificationService, "Parsing Records", (int) modInfoProvider.GetRecordCount(mod))) {
            foreach (var record in mod.EnumerateMajorRecords()) {
                records.Add(record.FormKey);
                counter.NextStep();

                foreach (var formLink in record.EnumerateFormLinks().Where(formLink => !formLink.IsNull)) {
                    var references = modCache.GetOrAdd(formLink.FormKey);
                    references.Add(FormLinkInformation.Factory(record));
                }
            }
            counter.Stop();
        }

        return new ModReferenceCache(modCache, records);
    }

    /// <summary>
    /// Writes reference cache of a mod to cache file
    /// </summary>
    /// <param name="cache">reference cache to write</param>
    /// <param name="cacheFilePath">path to the cache file</param>
    private void WriteReferenceCache(ModReferenceCache cache, string cacheFilePath) {
        // Open writer
        var cacheFile = fileSystem.FileInfo.New(cacheFilePath);
        if (!cacheFile.Exists) cacheFile.Directory?.Create();
        using var fileStream = fileSystem.File.OpenWrite(cacheFile.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write form keys
        writer.Write(cache.FormKeys.Count);
        foreach (var formKey in cache.FormKeys) {
            writer.Write(formKey.ToString());
        }

        // Write referenced form count
        writer.Write(cache.Cache.Count);

        // Write references
        using var counter = new CountingNotifier(notificationService, "Saving Cache", cache.Cache.Values.Sum(x => x.Count), TimeSpan.Zero);
        foreach (var (formKey, references) in cache.Cache) {
            counter.NextStep();

            writer.Write(formKey.ToString());
            writer.Write(references.Count);
            foreach (var reference in references) {
                writer.Write(reference.FormKey.ToString());
                writer.Write(mutagenTypeProvider.GetTypeName(reference));
            }
        }
    }

    /// <summary>
    /// Load reference cache of a mod
    /// </summary>
    /// <param name="cacheFilePath">path to the cache file</param>
    /// <returns>reference cache of the given mod key</returns>
    private ModReferenceCache LoadReferenceCache(string cacheFilePath) {
        using var linearNotifier = new LinearNotifier(notificationService);
        linearNotifier.Next("Decompressing Cache");
        var game = GetGameName(cacheFilePath);

        // Read mod cache file
        var fileStream = fileSystem.File.OpenRead(cacheFilePath);
        var zip = new GZipInputStream(fileStream);
        using (var reader = new BinaryReader(zip)) {
            // Read form keys
            var recordCount = reader.ReadInt32();
            var records = new HashSet<FormKey>(recordCount);
            for (var i = 0; i < recordCount; i++) {
                var formKey = FormKey.Factory(reader.ReadString());
                records.Add(formKey);
            }

            // Read references
            var formCount = reader.ReadInt32();
            var modCache = new ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>>();
            using (var counter = new CountingNotifier(notificationService, "Reading Cache", formCount)) {
                for (var i = 0; i < formCount; i++) {
                    counter.NextStep();
                    var formKey = FormKey.Factory(reader.ReadString());
                    var referenceCount = reader.ReadInt32();
                    var references = new HashSet<IFormLinkIdentifier>(referenceCount);
                    for (var j = 0; j < referenceCount; j++) {
                        var referenceFormKey = FormKey.Factory(reader.ReadString());
                        var typeString = reader.ReadString();
                        var type = mutagenTypeProvider.GetType(game, typeString);
                        references.Add(new FormLinkInformation(referenceFormKey, type));
                    }

                    modCache.TryAdd(formKey, references);
                }
            }

            return new ModReferenceCache(modCache, records);
        }
    }
}
