using System.Drawing;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public interface IFieldInformation {
    string Name { get; }
    Type TypeClass { get; }

    static bool IsValueType(Type type) {
        return type == typeof(FormKey)
         || type.InheritsFrom(typeof(IFormLinkGetter))
         || type.InheritsFrom(typeof(Enum))
         || type == typeof(Color)
         || type == typeof(bool)
         || type == typeof(string)
         || type == typeof(int)
         || type == typeof(uint)
         || type == typeof(long)
         || type == typeof(ulong)
         || type == typeof(short)
         || type == typeof(ushort)
         || type == typeof(float)
         || type == typeof(double)
         || type == typeof(sbyte)
         || type == typeof(byte);
    }
}

public sealed record ValueFieldInformation(Type TypeClass, Type ActualType) : IFieldInformation {
    public string Name => nameof(ConditionState.CompareValue);
}

public sealed record CollectionFieldInformation(Type TypeClass, Type ElementType) : IFieldInformation {
    public string Name => nameof(ConditionState.SubConditions);
}
