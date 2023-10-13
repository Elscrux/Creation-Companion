using System.IO.Abstractions;
using CreationEditor.Services.State;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Query;

public interface IQueryState {
    IEnumerable<IQueryRunner> LoadAllQueries();

    bool Save(IQueryRunner queryRunner);
}

public sealed class JsonQueryState : IQueryState {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IStatePathProvider _statePathProvider;
    private readonly JsonSerializerSettings _serializerSettings;

    public JsonQueryState(
        IContractResolver contractResolver,
        ILogger logger,
        IFileSystem fileSystem,
        IStatePathProvider statePathProvider) {
        _logger = logger;
        _fileSystem = fileSystem;
        _statePathProvider = statePathProvider;

        _serializerSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = contractResolver
        };
    }

    private string GetFileName(IQueryRunner queryRunner) => "Query-" + queryRunner.Id + ".json";
    private IFileInfo GetFilePath(IQueryRunner queryRunner) => _fileSystem.FileInfo.New(_statePathProvider.GetFullPath(GetFileName(queryRunner)));

    public IEnumerable<IQueryRunner> LoadAllQueries() {
        return _fileSystem.Directory
            .EnumerateFiles(_statePathProvider.GetDirectoryPath(), "*.json")
            .Select(filePath => _fileSystem.File.ReadAllText(filePath))
            .Select(json => JsonConvert.DeserializeObject<IQueryRunner>(json, _serializerSettings))
            .NotNull();
    }

    public bool Save(IQueryRunner queryRunner) {
        var filePath = GetFilePath(queryRunner);
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        _logger.Here().Information("Exporting Query {Id} to {Path}", queryRunner.Id, filePath);
        var content = JsonConvert.SerializeObject(queryRunner.CreateMemento(), _serializerSettings);
        _fileSystem.File.WriteAllText(filePath.FullName, content);

        return true;
    }
}
