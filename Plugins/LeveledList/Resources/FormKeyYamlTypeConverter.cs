using Mutagen.Bethesda.Plugins;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
namespace LeveledList.Resources;

public class FormKeyYamlTypeConverter : IYamlTypeConverter {
    public bool Accepts(Type type) => type == typeof(FormKey);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) {
        var value = parser.Consume<YamlDotNet.Core.Events.Scalar>().Value;
        return FormKey.Factory(value);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer) {
        var formKey = (FormKey) value!;
        emitter.Emit(new YamlDotNet.Core.Events.Scalar(formKey.ToString()));
    }
}
