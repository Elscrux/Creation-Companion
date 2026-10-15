using CreationEditor.Services.Config;
using Newtonsoft.Json;
namespace CreationEditor.Services.Serialization.Json;

/// <summary>
/// Serializes <see cref="ConfiguredValues{TDefinition}" /> items as their plain ConfigValues
/// dictionary. The config definition is not persisted; it is re-created with default options
/// from the item type when reading.
/// </summary>
public sealed class ConfiguredValuesJsonConverter : JsonConverter {
    public override bool CanConvert(Type objectType) =>
        objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ConfiguredValues<>);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        var configured = (IConfiguredValues) value!;
        serializer.Serialize(writer, configured.ConfigValues, typeof(ConfigValues));
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType is JsonToken.Null) return null;

        var item = (IConfiguredValues) Activator.CreateInstance(objectType)!;
        item.ConfigValues = serializer.Deserialize<ConfigValues>(reader) ?? new ConfigValues();
        return item;
    }
}
