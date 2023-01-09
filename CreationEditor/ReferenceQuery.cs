using System.Globalization;
using System.IO.Abstractions;
using System.IO.Compression;
using CreationEditor.Notification;
using Elscrux.Logging;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using MutagenLibrary.Core.Plugins;
using Serilog;
namespace CreationEditor;

public interface IReferenceQuery {
    public void LoadModReferences(IModGetter mod);
    public void LoadModReferences(IReadOnlyList<IModGetter> mods);
    public void LoadModReferences(ILinkCache linkCache);
    public void LoadModReferences(IGameEnvironment environment);

    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey);
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod);
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods);
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache);
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment);
}

/// <summary>
/// ReferenceQuery caches mod references to achieve quick access times for references instead of iterating through contained form links all the time.
/// </summary>
public sealed class ReferenceQuery : IReferenceQuery {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheSubdirectory = "References";
    private const string CacheExtension = "cache";
    private const string TempCacheExtension = "temp";
    private const string BaseNamespace = "Mutagen.Bethesda.";

    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly IFileSystem _fileSystem;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    private string CacheDirPath => _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory, CacheSubdirectory);
    private string CacheFile(ModKey mod) => _fileSystem.Path.Combine(CacheDirPath, $"{mod.Name}.{CacheExtension}");
    private string TempCacheFile(ModKey mod) => _fileSystem.Path.Combine(CacheDirPath, $"{mod.Name}.{TempCacheExtension}");
    private string ModFilePath(ModKey mod) => _fileSystem.Path.Combine(_simpleEnvironmentContext.DataDirectoryProvider.Path, mod.FileName);

    private readonly Dictionary<ModKey, ReferenceCache> _modCaches = new();

    public ReferenceQuery(
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IFileSystem fileSystem,
        INotificationService notificationService,
        ILogger logger) {
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _fileSystem = fileSystem;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Loads all references of a mod
    /// </summary>
    /// <param name="mod">mod to load references for</param>
    public void LoadModReferences(IModGetter mod) {
        if (_modCaches.ContainsKey(mod.ModKey)) return;
        
        _logger.Here().Debug("Starting to load references of {ModKey}", mod.ModKey);
        _modCaches.Add(mod.ModKey, CacheValid(mod) ? LoadReferenceCache(mod.ModKey) : BuildReferenceCache(mod));
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
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey) {
        var references = new HashSet<IFormLinkIdentifier>();
        foreach (var referenceCache in _modCaches.Values.Where(refCache => refCache.Cache.ContainsKey(formKey))) {
            foreach (var formLinkIdentifier in referenceCache.Cache[formKey]) {
                references.Add(formLinkIdentifier);
            }
        }

        return references;
    }

    /// <summary>
    /// Returns references of one form key in a mod
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="mod">mod to get references from</param>
    /// <returns>form links of references</returns>
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod) {
        LoadModReferences(mod);

        return _modCaches[mod.ModKey].Cache.TryGetValue(formKey, out var references) ? references : new HashSet<IFormLinkIdentifier>();
    }

    /// <summary>
    /// Returns references of one form key in a number of mods
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="mods">mods to to get references from</param>
    /// <returns>form links of references</returns>
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods) {
        var modList = mods.ToList();

        LoadModReferences(modList);

        var list = new HashSet<IFormLinkIdentifier>();
        foreach (var mod in modList.Where(mod => _modCaches[mod.ModKey].Cache.ContainsKey(formKey))) {
            foreach (var formLinkIdentifier in _modCaches[mod.ModKey].Cache[formKey]) {
                list.Add(formLinkIdentifier);
            }
        }

        return list;
    }

    /// <summary>
    /// Returns references of one form key in a link cache
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="linkCache">link cache to get references from</param>
    /// <returns>form links of references</returns>
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache) {
        return GetReferences(formKey, linkCache.PriorityOrder);
    }
    
    /// <summary>
    /// Returns references of one form key in a game environment
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="environment">environment to get references from</param>
    /// <returns>form links of references</returns>
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment) {
        return GetReferences(formKey, environment.LinkCache);
    }
    
    /// <summary>
    /// Regenerate cache of mod if necessary
    /// </summary>
    /// <param name="mod">mod to try to regenerate cache</param>
    /// <returns></returns>
    private bool CacheValid(IModKeyed mod) {
        if (!_fileSystem.File.Exists(CacheFile(mod.ModKey))) return false;

        var modFilePath = ModFilePath(mod.ModKey);
        if (!_fileSystem.File.Exists(modFilePath)) return false;

        using var fileStream = _fileSystem.File.OpenRead(CacheFile(mod.ModKey));
        using var zip = new GZipStream(fileStream, CompressionMode.Decompress);
        using var reader = new BinaryReader(zip);
        var dateTimeStr = reader.ReadString();
        if (DateTime.TryParse(dateTimeStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime)) {
                        
            var lastWriteTime = _fileSystem.File.GetLastWriteTime(modFilePath);
            return dateTime.CompareTo(lastWriteTime) >= 0;
        } else {
            _logger.Here().Warning("Couldn't parse {DateTimeStr} as DateTime in reference cache of {ModKey} -> build references instead", dateTimeStr, mod.ModKey);
            return false;
        }
    }

    /// <summary>
    /// Builds reference cache of one mod
    /// </summary>
    /// <param name="mod">mod to build reference cache for</param>
    /// <returns>reference cache of mod</returns>
    private ReferenceCache BuildReferenceCache(IModGetter mod) {
        //Fill modCache
        var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();

        var counter = new CountingNotifier(_notificationService, "Parsing Records", (int) mod.GetRecordCount());
        foreach (var recordGetter in mod.EnumerateMajorRecords()) {
            counter.NextStep();

            foreach (var formLinkGetter in recordGetter.EnumerateFormLinks()) {
                if (modCache.ContainsKey(formLinkGetter.FormKey)) {
                    modCache.TryGetValue(formLinkGetter.FormKey, out var references);
                    references?.Add(recordGetter.ToStandardizedIdentifier());
                } else {
                    recordGetter.ToLink();
                    modCache.Add(formLinkGetter.FormKey, new HashSet<IFormLinkIdentifier> { recordGetter.ToStandardizedIdentifier() });
                }
            }
        }
        counter.Stop();

        //Write modCache to file
        var fileInfo = _fileSystem.FileInfo.FromFileName(CacheFile(mod.ModKey));
        if (!fileInfo.Exists) fileInfo.Directory?.Create();
        using var fileStream = _fileSystem.File.OpenWrite(fileInfo.FullName);
        using var zip = new GZipStream(fileStream, CompressionMode.Compress);
        var writer = new BinaryWriter(zip);

        writer.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        writer.Write(modCache.Count);

        counter = new CountingNotifier(_notificationService, "Saving Cache", modCache.Count);
        foreach (var (key, value) in modCache) {
            counter.NextStep();

            writer.Write(key.ToString());
            writer.Write(value.Count);
            foreach (var formLinkGetter in value) {
                writer.Write(formLinkGetter.FormKey.ToString());
                writer.Write(formLinkGetter.Type.FullName![BaseNamespace.Length..]);
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
        try {
            var modCache = new Dictionary<FormKey, HashSet<IFormLinkIdentifier>>();

            //Decompress cache and copy to temporary uncompressed cache 
            var linearNotifier = new LinearNotifier(_notificationService);
            linearNotifier.Next("Decompressing Cache");
            
            using (var fileStream = _fileSystem.File.OpenRead(CacheFile(modKey))) {
                using (var zip = new GZipStream(fileStream, CompressionMode.Decompress)) {
                    var fileInfo = new FileInfo(TempCacheFile(modKey));
                    if (!fileInfo.Exists) fileInfo.Directory?.Create();
                    using (var tempFileStream = _fileSystem.File.OpenWrite(fileInfo.FullName)) {
                        zip.CopyTo(tempFileStream);
                    }
                }
            }
            linearNotifier.Stop();

            //Read mod cache file
            using (var reader = new BinaryReader(_fileSystem.File.OpenRead(TempCacheFile(modKey)))) {
                reader.ReadString(); //Skip date

                //Build ref cache
                var formCount = reader.ReadInt32();
                var counter = new CountingNotifier(_notificationService, "Reading Cache", formCount);
                for (var i = 0; i < formCount; i++) {
                    counter.NextStep();

                    var key = FormKey.Factory(reader.ReadString());
                    var referenceCount = reader.ReadInt32();
                    var value = new HashSet<IFormLinkIdentifier>();
                    for (var j = 0; j < referenceCount; j++) {
                        var formKey = FormKey.Factory(reader.ReadString());
                        var typeString = reader.ReadString();
                        var registration = LoquiRegistration.GetRegisterByFullName(BaseNamespace + typeString);
                        if (registration == null) {
                            _logger.Here().Error(
                                new ArgumentException($"Unknown object type: {typeString}"),
                                "Error while reading reference cache of '{Name}, Unknown object type: {TypeString} of {Key}",
                                modKey.Name,
                                typeString,
                                key);
                        } else {
                            value.Add(new FormLinkInformation(formKey, registration.GetterType));
                        }
                    }

                    modCache.Add(key, value);
                }
                counter.Stop();
            }

            return new ReferenceCache(modCache);
        } finally {
            var tempCacheFile = TempCacheFile(modKey);
            if (_fileSystem.File.Exists(tempCacheFile)) _fileSystem.File.Delete(tempCacheFile);
        }
    }
}
