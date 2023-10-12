using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReactiveUI;
namespace CreationEditor.Services.Json;

public sealed class NewtonsoftJsonSuspensionDriver : ISuspensionDriver {
    private readonly IFileSystem _fileSystem;
    private readonly string _stateFilePath;

    private readonly JsonSerializerSettings _settings;

    public NewtonsoftJsonSuspensionDriver(
        IFileSystem fileSystem,
        IContractResolver contractResolver,
        string stateFilePath) {
        _fileSystem = fileSystem;
        _stateFilePath = stateFilePath;
        _settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = contractResolver
        };
    }

    public IObservable<Unit> InvalidateState() {
        if (_fileSystem.File.Exists(_stateFilePath)) _fileSystem.File.Delete(_stateFilePath);

        return Observable.Return(Unit.Default);
    }

    public IObservable<object> LoadState() {
        if (!_fileSystem.File.Exists(_stateFilePath)) return Observable.Throw<object>(new FileNotFoundException(_stateFilePath));

        var lines = _fileSystem.File.ReadAllText(_stateFilePath);
        var state = JsonConvert.DeserializeObject<object>(lines, _settings) ?? throw new InvalidDataException($"{_stateFilePath} is not in valid format");
        return Observable.Return(state);

    }

    public IObservable<Unit> SaveState(object state) {
        var lines = JsonConvert.SerializeObject(state, _settings);
        _fileSystem.File.WriteAllText(_stateFilePath, lines);
        return Observable.Return(Unit.Default);
    }
}
