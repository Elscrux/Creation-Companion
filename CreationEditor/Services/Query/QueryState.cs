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

public sealed class JsonQueryState(
    IContractResolver contractResolver,
    ILogger logger,
    IFileSystem fileSystem,
    IStatePathProvider statePathProvider)
    : IQueryState {

    private readonly JsonSerializerSettings _serializerSettings = new() {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = contractResolver
    };

    private static string GetFileName(IQueryRunner queryRunner) => "Query-" + queryRunner.Id + ".json";
    private IFileInfo GetFilePath(IQueryRunner queryRunner) => fileSystem.FileInfo.New(statePathProvider.GetFullPath(GetFileName(queryRunner)));

    public IEnumerable<IQueryRunner> LoadAllQueries() {
        return fileSystem.Directory
            .EnumerateFiles(statePathProvider.GetDirectoryPath(), "*.json")
            .Select(filePath => fileSystem.File.ReadAllText(filePath))
            .Select(json => JsonConvert.DeserializeObject<IQueryRunner>(json, _serializerSettings))
            .NotNull();
    }

    public bool Save(IQueryRunner queryRunner) {
        var filePath = GetFilePath(queryRunner);
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        logger.Here().Information("Exporting Query {Id} to {Path}", queryRunner.Id, filePath);
        var content = JsonConvert.SerializeObject(queryRunner.CreateMemento(), _serializerSettings);
        fileSystem.File.WriteAllText(filePath.FullName, content);

        return true;
    }
}
