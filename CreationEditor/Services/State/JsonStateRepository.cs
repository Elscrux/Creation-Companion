using System.IO.Abstractions;
using Mutagen.Bethesda.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Noggog;
using Serilog;
namespace CreationEditor.Services.State;

public sealed class JsonStateRepository<TStateOut, TState, TIdentifier>(
    IContractResolver contractResolver,
    IStateIdentifier<TIdentifier> stateIdentifier,
    ILogger logger,
    IFileSystem fileSystem,
    params IEnumerable<string> stateIds)
    : IStateRepository<TStateOut, TState, TIdentifier>
    where TStateOut : class, TState
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

    private string GetDirectoryPath() {
        var directoryPath = fileSystem.Path.Combine([
            AppDomain.CurrentDomain.BaseDirectory,
            RootPath,
            ..stateIds
        ]);

        fileSystem.Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }

    private IEnumerable<string> EnumerateStateFiles() => fileSystem.Directory
        .EnumerateFiles(GetDirectoryPath(), Wildcard + JsonExtension, SearchOption.AllDirectories);

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

    public IEnumerable<TIdentifier> LoadAllIdentifiers() {
        var stateDirectory = GetDirectoryPath();
        return EnumerateStateFiles()
            .Select(filePath => GetIdentifier(fileSystem.Path.GetRelativePath(stateDirectory, filePath)));
    }

    public IEnumerable<TStateOut> LoadAll() {
        return EnumerateStateFiles()
            .Select(filePath => fileSystem.File.ReadAllText(filePath))
            .Select(json => JsonConvert.DeserializeObject<TStateOut>(json, _serializerSettings))
            .WhereNotNull();
    }

    public IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier() {
        var stateDirectory = GetDirectoryPath();
        return EnumerateStateFiles()
            .Select<string, KeyValuePair<TIdentifier, TState>?>(filePath => {
                var fileName = fileSystem.Path.GetRelativePath(stateDirectory, filePath);
                var identifier = GetIdentifier(fileName);
                var json = fileSystem.File.ReadAllText(filePath);
                var state = JsonConvert.DeserializeObject<TStateOut>(json, _serializerSettings);
                if (state is null) return null;

                return new KeyValuePair<TIdentifier, TState>(identifier, state);
            })
            .WhereNotNull()
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public TStateOut? Load(TIdentifier id) {
        var fileInfo = GetFileInfo(id);
        if (!fileInfo.Exists) return null;

        var json = fileSystem.File.ReadAllText(fileInfo.FullName);
        return JsonConvert.DeserializeObject<TStateOut>(json, _serializerSettings);
    }

    public bool Save(TState state, TIdentifier id) {
        if (state is not TStateOut stateT) {
            throw new ArgumentException($"State must be of type {typeof(TStateOut).Name}");
        }

        var filePath = GetFileInfo(id);
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        logger.Here().Verbose("Exporting {State} with Id {Id} to {Path}", stateIds.Last(), id, filePath);
        var content = JsonConvert.SerializeObject(stateT, _serializerSettings);
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
