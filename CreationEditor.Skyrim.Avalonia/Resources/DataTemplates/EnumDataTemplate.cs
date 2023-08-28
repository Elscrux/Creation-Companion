using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Definitions;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public sealed class EnumDataTemplate : ICustomConditionValueDataTemplate {
    public bool Match(Condition.Function function) => SkyrimDefinitions.ConditionValueEnums.Any(condition => condition.Match(function));

    public IObservable<Control?> Build(ILinkCache linkCache, EditableCondition editableCondition, IConditionDataGetter data, IObservable<Unit>? conditionDataChanged) {
        var conditionValueEnum = SkyrimDefinitions.ConditionValueEnums.FirstOrDefault(condition => condition.Match(editableCondition.Function));
        if (conditionValueEnum is null) return Observable.Empty<Control?>();

        var enumType = conditionValueEnum.Enums.First().GetType();
        var comboBox = new ComboBox {
            DataContext = editableCondition,
            ItemsSource = conditionValueEnum.Enums,
            [!SelectingItemsControl.SelectedItemProperty] = new Binding(nameof(EditableCondition.FloatValue)) {
                Converter = new ExtendedFuncValueConverter<float, Enum?, object?>(
                    (value, _) => (Enum?) Enum.ToObject(enumType, Convert.ToUInt32(value)),
                    (e, _) => Convert.ToSingle(e)
                ),
                Mode = BindingMode.TwoWay
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        return Observable.Return(comboBox);
    }
}
