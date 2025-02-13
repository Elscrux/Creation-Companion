﻿using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public sealed partial class ConditionCopyPasteController : ReactiveObject, IConditionCopyPasteController {
    private List<ICondition> _copiedConditions = [];

    [Reactive] public partial bool CanPaste { get; private set; }

    public void Copy(IEnumerable<ICondition> conditions) {
        _copiedConditions = conditions.ToList();
        CanPaste = _copiedConditions.Count > 0;
    }

    public void Paste(IList<EditableCondition> conditionsToPasteInto, int index = -1) {
        var editableConditions = _copiedConditions.Select(c => new EditableCondition(c)).ToList();
        if (index >= 0 && index < conditionsToPasteInto.Count) {
            conditionsToPasteInto.AddOrInsertRange(editableConditions, index);
        } else {
            conditionsToPasteInto.AddRange(editableConditions);
        }
    }
}
