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
public sealed class RecordReferenceQuery : IRecordReferenceQuery, IDisposableDropoff {
    private readonly Version _version = new(1, 0);

    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;
    private readonly INotificationService _notificationService;
    private readonly IModInfoProvider<IModGetter> _modInfoProvider;
    private readonly ICacheLocationProvider _cacheLocationProvider;
    private readonly ILogger _logger;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;

    private string ModFilePath(ModKey mod) => _fileSystem.Path.Combine(_dataDirectoryProvider.Path, mod.FileName);

    public IReadOnlyDictionary<ModKey, ModReferenceCache> ModCaches => _modCaches;
    private readonly Dictionary<ModKey, ModReferenceCache> _modCaches = new();

    public RecordReferenceQuery(
        Func<string[], ICacheLocationProvider> cacheLocationProviderFactory,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem,
        IMutagenTypeProvider mutagenTypeProvider,
        INotificationService notificationService,
        IModInfoProvider<IModGetter> modInfoProvider,
        ILogger logger) {
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
        _mutagenTypeProvider = mutagenTypeProvider;
        _notificationService = notificationService;
        _modInfoProvider = modInfoProvider;
        _logger = logger;
        _cacheLocationProvider = cacheLocationProviderFactory(new[] { "References", "Record" });
    }

    public void Dispose() => _disposables.Dispose();

    public void Add(IDisposable disposable) => _disposables.Add(disposable);

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mod">mod to load references for</param>
    public void LoadModReferences(IModGetter mod) {
        if (_modCaches.ContainsKey(mod.ModKey)) return;

        _logger.Here().Debug("Starting to load Record References of {ModKey}", mod.ModKey);
        _modCaches.Add(mod.ModKey, CacheValid(mod.ModKey) ? LoadReferenceCache(mod.ModKey) : BuildReferenceCache(mod));
        _logger.Here().Debug("Finished loading Record References of {ModKey}", mod.ModKey);
    }

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mods">enumerable of mods to load references for</param>
    public void LoadModReferences(IReadOnlyList<IModGetter> mods) {
        if (mods.All(mod => _modCaches.ContainsKey(mod.ModKey))) return;

        var notify = new LinearNotifier(_notificationService, mods.Count);
        foreach (var mod in mods) {
            notify.Next($"Loading Record References in {mod.ModKey}");
            LoadModReferences(mod);
        }
        notify.Stop();
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
        if (!_fileSystem.File.Exists(cacheFile)) return false;
        if (!_fileSystem.File.Exists(modFilePath)) return false;

        // Open cache reader
        using var fileStream = _fileSystem.File.OpenRead(cacheFile);
        using var zip = new GZipInputStream(fileStream);
        using var reader = new BinaryReader(zip);

        // Read version in cache
        if (!Version.TryParse(reader.ReadString(), out var version)
         || !version.Equals(_version)) return false;

        // Read checksum in cache
        var checksum = reader.ReadBytes(_fileSystem.GetChecksumBytesLength());

        // Validate checksum
        return _fileSystem.IsFileChecksumValid(modFilePath, checksum);
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

        using (var counter = new CountingNotifier(_notificationService, "Parsing Records", (int) _modInfoProvider.GetRecordCount(mod))) {
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
        var cacheFile = _fileSystem.FileInfo.New(_cacheLocationProvider.CacheFile(mod.ModKey.FileName));
        if (!cacheFile.Exists) cacheFile.Directory?.Create();
        using var fileStream = _fileSystem.File.OpenWrite(cacheFile.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write version
        writer.Write(_version.ToString());

        // Write checksum
        var modFilePath = ModFilePath(mod.ModKey);
        if (!_fileSystem.File.Exists(modFilePath)) return new ModReferenceCache(new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>(), new HashSet<FormKey>());

        var checksum = _fileSystem.GetFileChecksum(modFilePath);
        writer.Write(checksum);

        // Write game
        writer.Write(_mutagenTypeProvider.GetGameName(mod));

        writer.Write(records.Count);
        foreach (var formKey in records) {
            writer.Write(formKey.ToString());
        }

        // Write form count
        writer.Write(modCache.Count);

        // Write references
        using (var counter = new CountingNotifier(_notificationService, "Saving Cache", modCache.Values.Sum(x => x.Count), TimeSpan.Zero)) {
            foreach (var (formKey, references) in modCache) {
                counter.NextStep();

                writer.Write(formKey.ToString());
                writer.Write(references.Count);
                foreach (var reference in references) {
                    writer.Write(reference.FormKey.ToString());
                    writer.Write(_mutagenTypeProvider.GetTypeName(reference));
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
        using var linearNotifier = new LinearNotifier(_notificationService);
        linearNotifier.Next("Decompressing Cache");
        var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();
        var records = new HashSet<FormKey>();

        // Read mod cache file
        var cacheFile = _cacheLocationProvider.CacheFile(modKey.FileName);
        var fileStream = _fileSystem.File.OpenRead(cacheFile);
        var zip = new GZipInputStream(fileStream);
        using (var reader = new BinaryReader(zip)) {
            // Skip version and checksum
            reader.ReadString();
            reader.ReadBytes(_fileSystem.GetChecksumBytesLength());

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
            using (var counter = new CountingNotifier(_notificationService, "Reading Cache", formCount)) {
                for (var i = 0; i < formCount; i++) {
                    counter.NextStep();
                    var formKey = FormKey.Factory(reader.ReadString());
                    var referenceCount = reader.ReadInt32();
                    var references = new HashSet<IFormLinkIdentifier>();
                    for (var j = 0; j < referenceCount; j++) {
                        var referenceFormKey = FormKey.Factory(reader.ReadString());
                        var typeString = reader.ReadString();
                        if (_mutagenTypeProvider.GetType(game, typeString, out var type)) {
                            references.Add(new FormLinkInformation(referenceFormKey, type));
                        } else {
                            _logger.Here().Error(
                                new ArgumentException($"Unknown object type: {typeString}"),
                                "Error while reading reference cache of '{Name}, Unknown object type: {TypeString} of {Key}",
                                modKey.Name,
                                typeString,
                                formKey);
                        }
                    }

                    modCache.Add(formKey, references);
                }
            }
        }

        return new ModReferenceCache(modCache, records);
    }
}