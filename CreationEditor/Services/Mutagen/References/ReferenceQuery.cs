using System.IO.Abstractions;
using System.IO.Compression;
using CreationEditor.Extension;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

/// <summary>
/// ReferenceQuery caches mod references to achieve quick access times for references instead of iterating through contained form links all the time.
/// </summary>
public class ReferenceQuery : IReferenceQuery {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheSubdirectory = "References";
    private const string CacheExtension = "cache";
    private const string TempCacheExtension = "temp";

    private readonly IEnvironmentContext _environmentContext;
    private readonly IFileSystem _fileSystem;
    private readonly INotificationService _notificationService;
    private readonly IModInfoProvider<IModGetter> _modInfoProvider;
    private readonly ILogger _logger;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;

    private DirectoryPath CacheDirPath => _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory, CacheSubdirectory);
    private FilePath CacheFile(ModKey mod) => _fileSystem.Path.Combine(CacheDirPath, $"{mod.Name}.{CacheExtension}");
    private FilePath TempCacheFile(ModKey mod) => _fileSystem.Path.Combine(CacheDirPath, $"{mod.Name}.{TempCacheExtension}");
    private FilePath ModFilePath(ModKey mod) => _fileSystem.Path.Combine(_environmentContext.DataDirectoryProvider.Path, mod.FileName);

    private readonly Dictionary<ModKey, ReferenceCache> _modCaches = new();

    public ReferenceQuery(
        IEnvironmentContext environmentContext,
        IFileSystem fileSystem,
        IMutagenTypeProvider mutagenTypeProvider,
        INotificationService notificationService,
        IModInfoProvider<IModGetter> modInfoProvider,
        ILogger logger) {
        _environmentContext = environmentContext;
        _fileSystem = fileSystem;
        _mutagenTypeProvider = mutagenTypeProvider;
        _notificationService = notificationService;
        _modInfoProvider = modInfoProvider;
        _logger = logger;
    }

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mod">mod to load references for</param>
    public void LoadModReferences(IModGetter mod) {
        if (_modCaches.ContainsKey(mod.ModKey)) return;

        _logger.Here().Debug("Starting to load references of {ModKey}", mod.ModKey);
        _modCaches.Add(mod.ModKey, CacheValid(mod.ModKey) ? LoadReferenceCache(mod.ModKey) : BuildReferenceCache(mod));
        _logger.Here().Debug("Finished loading references of {ModKey}", mod.ModKey);
    }

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mods">enumerable of mods to load references for</param>
    public void LoadModReferences(IReadOnlyList<IModGetter> mods) {
        if (mods.All(mod => _modCaches.ContainsKey(mod.ModKey))) return;

        var notify = new LinearNotifier(_notificationService, mods.Count);
        foreach (var mod in mods) {
            notify.Next($"Loading References in {mod.ModKey}");
            LoadModReferences(mod);
        }
        notify.Stop();
    }

    /// <summary>
    /// Loads all references of a link cache
    /// </summary>
    /// <param name="linkCache">link cache to load references for</param>
    public void LoadModReferences(ILinkCache linkCache) {
        LoadModReferences(linkCache.PriorityOrder);
    }

    /// <summary>
    /// Loads all references of an environment
    /// </summary>
    /// <param name="environment">environment to load references for</param>
    public void LoadModReferences(IGameEnvironment environment) {
        LoadModReferences(environment.LinkCache);
    }

    /// <summary>
    /// Returns references of one form key in all loaded mod caches
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey) {
        foreach (var referenceCache in _modCaches.Values.Where(refCache => refCache.Cache.ContainsKey(formKey))) {
            foreach (var formLinkIdentifier in referenceCache.Cache[formKey]) {
                yield return formLinkIdentifier;
            }
        }
    }

    /// <summary>
    /// Returns references of one form key in a mod
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="mod">mod to get references from</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod) {
        LoadModReferences(mod);

        if (!_modCaches[mod.ModKey].Cache.TryGetValue(formKey, out var references)) yield break;

        foreach (var reference in references) {
            yield return reference;
        }
    }

    /// <summary>
    /// Returns references of one form key in a number of mods
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="mods">mods to to get references from</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods) {
        foreach (var mod in mods) {
            foreach (var reference in GetReferences(formKey, mod)) {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// Returns references of one form key in a link cache
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="linkCache">link cache to get references from</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache) {
        return GetReferences(formKey, linkCache.PriorityOrder);
    }

    /// <summary>
    /// Returns references of one form key in a game environment
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="environment">environment to get references from</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment) {
        return GetReferences(formKey, environment.LinkCache);
    }

    /// <summary>
    /// Check if the cache of a mod is up to date
    /// </summary>
    /// <param name="modKey">mod key to check cache for</param>
    /// <returns></returns>
    private bool CacheValid(ModKey modKey) {
        var cacheFile = CacheFile(modKey);
        var modFilePath = ModFilePath(modKey);

        // Check if mod and cache exist
        if (!_fileSystem.File.Exists(cacheFile)) return false;
        if (!_fileSystem.File.Exists(modFilePath)) return false;

        // Read checksum in cache
        using var fileStream = _fileSystem.File.OpenRead(cacheFile);
        using var zip = new GZipStream(fileStream, CompressionMode.Decompress);
        using var reader = new BinaryReader(zip);
        var checksum = reader.ReadBytes(_fileSystem.GetChecksumBytesLength());

        // Read checksum
        var actualChecksum = _fileSystem.GetChecksum(modFilePath);

        return actualChecksum.SequenceEqual(checksum);
    }

    /// <summary>
    /// Builds reference cache of one mod
    /// </summary>
    /// <param name="mod">mod to build reference cache for</param>
    /// <returns>reference cache of mod</returns>
    private ReferenceCache BuildReferenceCache(IModGetter mod) {
        // Fill modCache
        var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();

        var counter = new CountingNotifier(_notificationService, "Parsing Records", (int) _modInfoProvider.GetRecordCount(mod));
        foreach (var record in mod.EnumerateMajorRecords()) {
            counter.NextStep();

            foreach (var formLink in record.EnumerateFormLinks().Where(formLink => !formLink.IsNull)) {
                var references = modCache.GetOrAdd(formLink.FormKey);
                references.Add(FormLinkInformation.Factory(record));
            }
        }
        counter.Stop();

        // Write modCache to file
        var cacheFile = CacheFile(mod.ModKey);
        if (!_fileSystem.File.Exists(cacheFile)) cacheFile.Directory?.Create();
        using var fileStream = _fileSystem.File.OpenWrite(cacheFile.Path);
        using var zip = new GZipStream(fileStream, CompressionMode.Compress);
        var writer = new BinaryWriter(zip);

        // Write checksum
        var modFilePath = ModFilePath(mod.ModKey);
        if (!_fileSystem.File.Exists(modFilePath)) return new ReferenceCache(new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>());

        var checksum = _fileSystem.GetChecksum(modFilePath);
        writer.Write(checksum);

        // Write game
        var game = _mutagenTypeProvider.GetGameName(mod.EnumerateMajorRecords().First());
        writer.Write(game);

        // Write form count
        writer.Write(modCache.Count);

        // Write references
        counter = new CountingNotifier(_notificationService, "Saving Cache", modCache.Values.Sum(x => x.Count), TimeSpan.Zero);
        foreach (var (formKey, references) in modCache) {
            counter.NextStep();

            writer.Write(formKey.ToString());
            writer.Write(references.Count);
            foreach (var reference in references) {
                writer.Write(reference.FormKey.ToString());
                writer.Write(_mutagenTypeProvider.GetTypeName(reference.Type));
            }
        }
        counter.Stop();

        return new ReferenceCache(modCache);
    }

    /// <summary>
    /// Load reference cache of a mod
    /// </summary>
    /// <param name="modKey">mod to load cache for</param>
    /// <returns>reference cache of the given mod key</returns>
    private ReferenceCache LoadReferenceCache(ModKey modKey) {
        var tempFile = TempCacheFile(modKey);
        try {
            var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();

            // Decompress cache and copy to temporary uncompressed cache 
            var linearNotifier = new LinearNotifier(_notificationService);
            linearNotifier.Next("Decompressing Cache");

            using (var fileStream = _fileSystem.File.OpenRead(CacheFile(modKey))) {
                using (var zip = new GZipStream(fileStream, CompressionMode.Decompress)) {
                    if (!_fileSystem.File.Exists(tempFile)) tempFile.Directory?.Create();
                    using (var tempFileStream = _fileSystem.File.OpenWrite(tempFile.Path)) {
                        zip.CopyTo(tempFileStream);
                    }
                }
            }
            linearNotifier.Stop();

            // Read mod cache file
            using (var reader = new BinaryReader(_fileSystem.File.OpenRead(TempCacheFile(modKey)))) {
                reader.ReadBytes(_fileSystem.GetChecksumBytesLength());

                // Read game string
                var game = reader.ReadString();

                // Build ref cache
                var formCount = reader.ReadInt32();
                var counter = new CountingNotifier(_notificationService, "Reading Cache", formCount);
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
                counter.Stop();
            }

            return new ReferenceCache(modCache);
        } finally {
            if (_fileSystem.File.Exists(tempFile)) _fileSystem.File.Delete(tempFile);
        }
    }
}
