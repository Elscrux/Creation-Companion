using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Mutagen.Bethesda.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReactiveUI;
namespace CreationEditor.Services.Json;

public sealed class NewtonsoftJsonSuspensionDriver(
    IFileSystem fileSystem,
    IContractResolver contractResolver,
    string stateFilePath)
    : ISuspensionDriver {

    private readonly JsonSerializerSettings _settings = new() {
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = contractResolver,
        Converters = {
            JsonConvertersMixIn.FormKey,
            JsonConvertersMixIn.ModKey,
        },
    };

    public IObservable<Unit> InvalidateState() {
        if (fileSystem.File.Exists(stateFilePath)) fileSystem.File.Delete(stateFilePath);

        return Observable.Return(Unit.Default);
    }

    public IObservable<object> LoadState() {
        if (!fileSystem.File.Exists(stateFilePath)) return Observable.Throw<object>(new FileNotFoundException(stateFilePath));

        var lines = fileSystem.File.ReadAllText(stateFilePath);
        var state = JsonConvert.DeserializeObject<object>(lines, _settings)
         ?? throw new InvalidDataException($"{stateFilePath} is not in valid format");
        return Observable.Return(state);

    }

    public IObservable<Unit> SaveState(object state) {
        var lines = JsonConvert.SerializeObject(state, _settings);
        fileSystem.File.WriteAllText(stateFilePath, lines);
        return Observable.Return(Unit.Default);
    }
}
