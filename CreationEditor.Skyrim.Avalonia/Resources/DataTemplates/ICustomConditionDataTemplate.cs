using Avalonia.Controls;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public interface ICustomConditionDataTemplate {
    Type Type { get; }
    Func<ConditionData, string, string, IEnumerable<Type>, Control> GetFormKeyPicker { get; set; }
    void Apply(
        IObservable<IMajorRecordGetter?> context,
        IObservable<IQuestGetter?> questContext,
        EditableCondition condition,
        ConditionData data,
        IList<Control> parameterControls);
}
