using Mutagen.Bethesda.Plugins;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
namespace LeveledList.Resources;

public sealed class FormKeyYamlTypeConverter : IYamlTypeConverter {
    public bool Accepts(Type type) => type == typeof(FormKey);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) {
        var value = parser.Consume<Scalar>().Value;
        return FormKey.Factory(value);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer) {
        var formKey = (FormKey) value!;
        emitter.Emit(new Scalar(formKey.ToString()));
    }
}
