using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Environments.DI;
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
    IDataDirectoryProvider dataDirectoryProvider,
    IFileSystem fileSystem,
    IMutagenTypeProvider mutagenTypeProvider,
    INotificationService notificationService,
    IModInfoProvider<IModGetter> modInfoProvider,
    ILogger logger)
    : IRecordReferenceQuery, IDisposableDropoff {
    private static readonly string[] CacheLocation = ["References", "Record"];

    private readonly Version _version = new(1, 0);

    private readonly DisposableBucket _disposables = new();
    private readonly ICacheLocationProvider _cacheLocationProvider = cacheLocationProviderFactory(CacheLocation);

    private string ModFilePath(ModKey mod) => fileSystem.Path.Combine(dataDirectoryProvider.Path, mod.FileName);

    public IReadOnlyDictionary<ModKey, ModReferenceCache> ModCaches => _modCaches;
    private readonly Dictionary<ModKey, ModReferenceCache> _modCaches = new();

    public void Dispose() => _disposables.Dispose();

    public void Add(IDisposable disposable) => _disposables.Add(disposable);

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mod">mod to load references for</param>
    public void LoadModReferences(IModGetter mod) {
        if (_modCaches.ContainsKey(mod.ModKey)) return;

        logger.Here().Debug("Starting to load Record References of {ModKey}", mod.ModKey);
        try {
            _modCaches.Add(mod.ModKey, CacheValid(mod.ModKey) ? LoadReferenceCache(mod.ModKey) : BuildReferenceCache(mod));
        } catch (Exception e) {
            // Catch any issues while loading references
            logger.Here().Error("Loading record references for {ModKey} failed: {Message}", mod.ModKey, e.Message);
            logger.Here().Debug("Try to generate record references for {ModKey} again", mod.ModKey);

            // Delete broken cache
            TryDeleteCache(mod.ModKey);

            // Try again
            _modCaches.Add(mod.ModKey, BuildReferenceCache(mod));
        }
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

    private void TryDeleteCache(ModKey modKey) {
        var cacheFile = _cacheLocationProvider.CacheFile(modKey.FileName);
        if (fileSystem.File.Exists(cacheFile)) {
            try {
                fileSystem.File.Delete(cacheFile);
            } catch (Exception e) {
                logger.Here().Warning("Trying to delete cache file {CacheFile} failed: {Message}", cacheFile, e.Message);
            }
        }
    }

    /// <summary>
    /// Check if the cache of a mod is up to date
    /// </summary>
    /// <param name="modKey">mod key to check cache for</param>
    /// <returns></returns>
    private bool CacheValid(ModKey modKey) {
        var cacheFile = _cacheLocationProvider.CacheFile(modKey.FileName);
        var modFilePath = ModFilePath(modKey);

        // Check if mod and cache exist
        if (!fileSystem.File.Exists(cacheFile)) return false;
        if (!fileSystem.File.Exists(modFilePath)) return false;

        // Open cache reader
        using var fileStream = fileSystem.File.OpenRead(cacheFile);
        using var zip = new GZipInputStream(fileStream);
        using var reader = new BinaryReader(zip);

        // Read version in cache
        if (!Version.TryParse(reader.ReadString(), out var version)
         || !version.Equals(_version)) return false;

        // Read hash in cache
        var hash = reader.ReadBytes(fileSystem.GetHashBytesLength());

        // Validate hash
        return fileSystem.IsFileHashValid(modFilePath, hash);
    }

    /// <summary>
    /// Builds reference cache of one mod
    /// </summary>
    /// <param name="mod">mod to build reference cache for</param>
    /// <returns>reference cache of mod</returns>
    private ModReferenceCache BuildReferenceCache(IModGetter mod) {
        // Fill modCache
        var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();
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

        // Write modCache to file
        var cacheFile = fileSystem.FileInfo.New(_cacheLocationProvider.CacheFile(mod.ModKey.FileName));
        if (!cacheFile.Exists) cacheFile.Directory?.Create();
        using var fileStream = fileSystem.File.OpenWrite(cacheFile.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write version
        writer.Write(_version.ToString());

        // Write hash
        var modFilePath = ModFilePath(mod.ModKey);
        if (!fileSystem.File.Exists(modFilePath)) return new ModReferenceCache(new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>(), []);

        var hash = fileSystem.GetFileHash(modFilePath);
        writer.Write(hash);

        // Write game
        writer.Write(mutagenTypeProvider.GetGameName(mod));

        writer.Write(records.Count);
        foreach (var formKey in records) {
            writer.Write(formKey.ToString());
        }

        // Write form count
        writer.Write(modCache.Count);

        // Write references
        using (var counter = new CountingNotifier(notificationService, "Saving Cache", modCache.Values.Sum(x => x.Count), TimeSpan.Zero)) {
            foreach (var (formKey, references) in modCache) {
                counter.NextStep();

                writer.Write(formKey.ToString());
                writer.Write(references.Count);
                foreach (var reference in references) {
                    writer.Write(reference.FormKey.ToString());
                    writer.Write(mutagenTypeProvider.GetTypeName(reference));
                }
            }

            return new ModReferenceCache(modCache, records);
        }
    }

    /// <summary>
    /// Load reference cache of a mod
    /// </summary>
    /// <param name="modKey">mod to load cache for</param>
    /// <returns>reference cache of the given mod key</returns>
    private ModReferenceCache LoadReferenceCache(ModKey modKey) {
        using var linearNotifier = new LinearNotifier(notificationService);
        linearNotifier.Next("Decompressing Cache");
        var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();
        var records = new HashSet<FormKey>();

        // Read mod cache file
        var cacheFile = _cacheLocationProvider.CacheFile(modKey.FileName);
        var fileStream = fileSystem.File.OpenRead(cacheFile);
        var zip = new GZipInputStream(fileStream);
        using (var reader = new BinaryReader(zip)) {
            // Skip version and hash
            reader.ReadString();
            reader.ReadBytes(fileSystem.GetHashBytesLength());

            // Read game string
            var game = reader.ReadString();

            // Build ref cache
            var recordCount = reader.ReadInt32();
            for (var i = 0; i < recordCount; i++) {
                var formKey = FormKey.Factory(reader.ReadString());
                records.Add(formKey);
            }

            // Build ref cache
            var formCount = reader.ReadInt32();
            using (var counter = new CountingNotifier(notificationService, "Reading Cache", formCount)) {
                for (var i = 0; i < formCount; i++) {
                    counter.NextStep();
                    var formKey = FormKey.Factory(reader.ReadString());
                    var referenceCount = reader.ReadInt32();
                    var references = new HashSet<IFormLinkIdentifier>();
                    for (var j = 0; j < referenceCount; j++) {
                        var referenceFormKey = FormKey.Factory(reader.ReadString());
                        var typeString = reader.ReadString();
                        var type = mutagenTypeProvider.GetType(game, typeString);
                        references.Add(new FormLinkInformation(referenceFormKey, type));
                    }

                    modCache.Add(formKey, references);
                }
            }
        }

        return new ModReferenceCache(modCache, records);
    }
}
