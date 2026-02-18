using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Serialization.Json;
using Newtonsoft.Json;
using ReactiveUI;
namespace CreationEditor.Services.Json;

public sealed class NewtonsoftJsonSuspensionDriver(
    IJsonSerializerSettingsProvider jsonSerializerSettingsProvider,
    IFileSystem fileSystem,
    string stateFilePath)
    : ISuspensionDriver {

    public IObservable<Unit> InvalidateState() {
        if (fileSystem.File.Exists(stateFilePath)) fileSystem.File.Delete(stateFilePath);

        return Observable.Return(Unit.Default);
    }

    public IObservable<object> LoadState() {
        if (!fileSystem.File.Exists(stateFilePath)) return Observable.Throw<object>(new FileNotFoundException(stateFilePath));

        var lines = fileSystem.File.ReadAllText(stateFilePath);
        var state = JsonConvert.DeserializeObject<object>(lines, jsonSerializerSettingsProvider.SerializerSettings)
         ?? throw new InvalidDataException($"{stateFilePath} is not in valid format");
        return Observable.Return(state);

    }

    public IObservable<Unit> SaveState(object state) {
        var lines = JsonConvert.SerializeObject(state, jsonSerializerSettingsProvider.SerializerSettings);
        fileSystem.File.WriteAllText(stateFilePath, lines);
        return Observable.Return(Unit.Default);
    }
}
