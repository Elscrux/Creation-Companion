using System.Drawing;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Where;

public sealed class ConditionState : ReactiveObject {
    private readonly ICompareFunction? _selectedCompareFunction;
    private readonly Type? _underlyingType;

    [Reactive] public object? CompareValue { get; set; }
    public IObservableCollection<IQueryConditionEntry> SubConditions { get; } = new ObservableCollectionExtended<IQueryConditionEntry>();

    public ConditionState(ICompareFunction? selectedCompareFunction, Type? underlyingType) {
        _selectedCompareFunction = selectedCompareFunction;
        _underlyingType = underlyingType;
    }

    public IEnumerable<FieldType> GetFields() {
        if (_underlyingType is null || _selectedCompareFunction is null) return Array.Empty<FieldType>();

        return _selectedCompareFunction.GetFields(_underlyingType);
    }

    public static string FieldCategoryToName(FieldCategory category) {
        return category switch {
            FieldCategory.Value => nameof(CompareValue),
            FieldCategory.Collection => nameof(SubConditions),
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    public static bool IsPrimitiveType(Type type) {
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
