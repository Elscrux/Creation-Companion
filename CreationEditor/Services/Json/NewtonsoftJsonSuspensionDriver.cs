using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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

    public IObservable<object?> LoadState() => LoadState(JsonTypeInfo.CreateJsonTypeInfo<object>(new JsonSerializerOptions()));
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) {
        if (!fileSystem.File.Exists(stateFilePath)) return Observable.Throw<T>(new FileNotFoundException(stateFilePath));

        var lines = fileSystem.File.ReadAllText(stateFilePath);
        var state = JsonConvert.DeserializeObject<T>(lines, jsonSerializerSettingsProvider.SerializerSettings)
         ?? throw new InvalidDataException($"{stateFilePath} is not in valid format");
        return Observable.Return<T>(state);

    }

    public IObservable<Unit> SaveState<T>(T state) => SaveState(state, JsonTypeInfo.CreateJsonTypeInfo<T>(new JsonSerializerOptions()));
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) {
        var lines = JsonConvert.SerializeObject(state, jsonSerializerSettingsProvider.SerializerSettings);
        fileSystem.File.WriteAllText(stateFilePath, lines);
        return Observable.Return(Unit.Default);
    }
}
