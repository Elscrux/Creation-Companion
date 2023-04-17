using System.Collections.Generic;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public interface IConditionCopyPasteController {
    void Copy(IEnumerable<ICondition> conditions);
    void Paste(IList<EditableCondition> conditionsToPasteInto, int index = -1);
    bool CanPaste { get; }
}
