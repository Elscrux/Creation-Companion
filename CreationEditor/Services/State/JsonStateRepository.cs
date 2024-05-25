using System.IO.Abstractions;
using Mutagen.Bethesda.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Noggog;
using Serilog;
namespace CreationEditor.Services.State;

public sealed class JsonStateRepository<T>(
    IContractResolver contractResolver,
    ILogger logger,
    IFileSystem fileSystem,
    string stateId)
    : IStateRepository<T>
    where T : class {

    private const int GuidLength = 36;
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
        var directoryPath = fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            RootPath,
            stateId);

        fileSystem.Directory.CreateDirectory(directoryPath);

        return directoryPath;
    }

    private static string GetFileName(string name, Guid id) => name + "-" + id + JsonExtension;
    private static StateIdentifier GetStateIdentifier(string fileName) {
        var guidStart = fileName.Length - JsonExtension.Length - GuidLength;
        var id = Guid.Parse(fileName.AsSpan(guidStart, GuidLength));
        var name = fileName[..(guidStart - 1)];
        return new StateIdentifier(id, name);
    }

    private IFileInfo GetFileInfo(string name, Guid id) {
        var fileName = GetFileName(name, id);

        var filePath = fileSystem.Path.Combine(
            GetDirectoryPath(),
            fileName.Replace(" ", "-"));

        return fileSystem.FileInfo.New(filePath);
    }

    public int Count() {
        return fileSystem.Directory
            .EnumerateFiles(GetDirectoryPath(), Wildcard + JsonExtension)
            .Count();
    }

    public IEnumerable<StateIdentifier> LoadAllStateIdentifiers() {
        return fileSystem.Directory
            .EnumerateFiles(GetDirectoryPath(), Wildcard + JsonExtension)
            .Select(filePath => GetStateIdentifier(fileSystem.Path.GetFileName(filePath)))
            .NotNull();
    }

    public IEnumerable<T> LoadAll() {
        return fileSystem.Directory
            .EnumerateFiles(GetDirectoryPath(), Wildcard + JsonExtension)
            .Select(filePath => fileSystem.File.ReadAllText(filePath))
            .Select(json => JsonConvert.DeserializeObject<T>(json, _serializerSettings))
            .NotNull();
    }

    public T? Load(Guid id) {
        var filePath = fileSystem.Directory
            .EnumerateFiles(GetDirectoryPath(), Wildcard + id + JsonExtension)
            .FirstOrDefault();

        if (filePath is null) return null;

        var json = fileSystem.File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json, _serializerSettings);
    }

    public bool Save(T state, Guid id, string name = "") {
        var filePath = GetFileInfo(name, id);
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        logger.Here().Verbose("Exporting {State} {Name} with Id {Id} to {Path}", stateId, name, id, filePath);
        var content = JsonConvert.SerializeObject(state, _serializerSettings);
        fileSystem.File.WriteAllText(filePath.FullName, content);

        return true;
    }

    public void Delete(Guid id, string name) {
        var filePath = GetFileInfo(name, id);
        if (filePath.Exists) filePath.Delete();
    }
}
