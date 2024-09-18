using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public interface ICustomConditionValueDataTemplate {
    public bool Match(Condition.Function function);
    public IObservable<Control?> Build(
        ILinkCache linkCache,
        EditableCondition editableCondition,
        IConditionDataGetter data,
        IObservable<Unit>? conditionDataChanged);
}
