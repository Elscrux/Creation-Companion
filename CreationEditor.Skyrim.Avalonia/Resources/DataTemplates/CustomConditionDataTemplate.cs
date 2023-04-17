using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public interface ICustomConditionDataTemplate {
    public Type Type { get; }
    public Func<ConditionData, string, string, IEnumerable<Type>, Control> GetFormKeyPicker { get; set; }
    public void Apply(IObservable<IMajorRecordGetter?> context, IObservable<IQuestGetter?> questContext, EditableCondition condition, ConditionData data, IList<Control> parameterControls);
}

public abstract class CustomConditionDataTemplate<T> : ICustomConditionDataTemplate
    where T : ConditionData {
    public Type Type => typeof(T);
    public Func<ConditionData, string, string, IEnumerable<Type>, Control> GetFormKeyPicker { get; set; } = null!;

    public void Apply(IObservable<IMajorRecordGetter?> context, IObservable<IQuestGetter?> questContext, EditableCondition condition, ConditionData data, IList<Control> parameterControls) {
        if (data is T t) Apply(context, questContext, condition, t, parameterControls);
    }

    protected abstract void Apply(IObservable<IMajorRecordGetter?> context, IObservable<IQuestGetter?> questContext, EditableCondition condition, T data, IList<Control> parameterControls);
}
