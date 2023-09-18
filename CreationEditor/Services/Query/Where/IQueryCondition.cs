using System.Drawing;
using System.Reactive;
using CreationEditor.Core;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query.Where;

public interface IQueryConditionMemento {
    string SelectedFunctionOperator { get; init; }
}

public sealed record FieldType(Type TypeClass, Type ActualType, string CompareValue);

public interface IQueryCondition : IReactiveObject, IMementoProvider<IQueryConditionMemento> {
    int Priority { get; }

    Type UnderlyingType { get; set; }
    IList<ICompareFunction> Functions { get; }
    ICompareFunction SelectedFunction { get; set; }

    IObservable<string> Summary { get; }
    IObservable<Unit> ConditionChanged { get; }

    bool Accepts(Type type);
    bool Evaluate(object? fieldValue);
    List<FieldType> GetFields();

    public static bool IsPrimitiveType(Type type) {
        return type == typeof(FormKey)
         || type.InheritsFrom(typeof(IFormLinkGetter))
         || type.InheritsFrom(typeof(Enum))
         || type == typeof(Color)
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
