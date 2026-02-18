using Mutagen.Bethesda.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace CreationEditor.Services.Serialization.Json;

public interface IJsonSerializerSettingsProvider {
    public JsonSerializerSettings SerializerSettings { get; }
}

public class JsonSerializerSettingsProvider(IContractResolver contractResolver) : IJsonSerializerSettingsProvider {
    public JsonSerializerSettings SerializerSettings { get; } = new() {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = contractResolver,
        Converters = {
            JsonConvertersMixIn.FormKey,
            JsonConvertersMixIn.ModKey,
        },
    };
}
