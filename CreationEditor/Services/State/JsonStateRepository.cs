using System.IO.Abstractions;
using Mutagen.Bethesda.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Noggog;
using Serilog;
namespace CreationEditor.Services.State;

public sealed class JsonStateRepository<TState, TIdentifier>(
    IContractResolver contractResolver,
    IStateIdentifier<TIdentifier> stateIdentifier,
    ILogger logger,
    IFileSystem fileSystem,
    params IEnumerable<string> stateIds)
    : IStateRepository<TState, TIdentifier>
    where TState : class
    where TIdentifier : notnull {

    private const string JsonExtension = ".json";
    private const string Wildcard = "*";
    private const string RootPath = "States";

    private readonly JsonSerializerSettings _serializerSettings = new() {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = contractResolver,
        Converters = {
            JsonConvertersMixIn.FormKey,
            JsonConvertersMixIn.ModKey,
        },
    };

    private string GetDirectoryPath(bool createDirectory = true) {
        var directoryPath = fileSystem.Path.Combine([
            AppDomain.CurrentDomain.BaseDirectory,
            RootPath,
            ..stateIds
        ]);

        if (createDirectory) fileSystem.Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }

    private IEnumerable<string> EnumerateStateFiles() => fileSystem.Directory
        .EnumerateFiles(GetDirectoryPath(), Wildcard + JsonExtension);

    private TIdentifier GetIdentifier(string fileName) {
        var idLength = fileName.Length - JsonExtension.Length;
        var idSpan = fileName.AsSpan(0, idLength);
        return stateIdentifier.Parse(idSpan);
    }

    private IFileInfo GetFileInfo(TIdentifier id) {
        var fileName = stateIdentifier.AsFileName(id);
        var filePath = fileSystem.Path.Combine(
            GetDirectoryPath(),
            fileName.Replace(' ', '-') + JsonExtension);

        return fileSystem.FileInfo.New(filePath);
    }

    public int Count() {
        return EnumerateStateFiles().Count();
    }

    public IEnumerable<string> GetNeighboringStates() {
        var directoryPath = GetDirectoryPath(false);
        var parentDirectory = fileSystem.Path.GetDirectoryName(directoryPath);
        if (!fileSystem.Directory.Exists(parentDirectory)) return [];

        return fileSystem.Directory.EnumerateDirectories(parentDirectory);

    }

    public IEnumerable<TIdentifier> LoadAllIdentifiers() {
        return EnumerateStateFiles()
            .Select(filePath => GetIdentifier(fileSystem.Path.GetFileName(filePath)));
    }

    public IEnumerable<TState> LoadAll() {
        return EnumerateStateFiles()
            .Select(filePath => fileSystem.File.ReadAllText(filePath))
            .Select(json => JsonConvert.DeserializeObject<TState>(json, _serializerSettings))
            .WhereNotNull();
    }

    public IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier() {
        return EnumerateStateFiles()
            .Select<string, KeyValuePair<TIdentifier, TState>?>(filePath => {
                var identifier = GetIdentifier(fileSystem.Path.GetFileName(filePath));
                var json = fileSystem.File.ReadAllText(filePath);
                var state = JsonConvert.DeserializeObject<TState>(json, _serializerSettings);
                if (state is null) return null;

                return new KeyValuePair<TIdentifier, TState>(identifier, state);
            })
            .WhereNotNull()
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public TState? Load(TIdentifier id) {
        var fileInfo = GetFileInfo(id);
        if (!fileInfo.Exists) return null;

        var json = fileSystem.File.ReadAllText(fileInfo.FullName);
        return JsonConvert.DeserializeObject<TState>(json, _serializerSettings);
    }

    public bool Save(TState state, TIdentifier id) {
        var filePath = GetFileInfo(id);
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        logger.Here().Verbose("Exporting {State} with Id {Id} to {Path}", stateIds.Last(), id, filePath);
        var content = JsonConvert.SerializeObject(state, _serializerSettings);
        fileSystem.File.WriteAllText(filePath.FullName, content);

        return true;
    }

    public void Delete(TIdentifier id) {
        var filePath = GetFileInfo(id);
        if (!filePath.Exists) return;

        logger.Here().Verbose("Deleting {State} with Id {Id} at {Path}", stateIds.Last(), id, filePath.FullName);
        filePath.Delete();
    }
}
