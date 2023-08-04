using System.Collections.Generic;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public interface IConditionCopyPasteController {
    /// <summary>
    /// Copies the given conditions to the conditions clipboard.
    /// </summary>
    /// <param name="conditions">Conditions to copy</param>
    void Copy(IEnumerable<ICondition> conditions);

    /// <summary>
    /// Pastes the conditions from the conditions clipboard into the given conditions.
    /// </summary>
    /// <param name="conditionsToPasteInto">List to paste into</param>
    /// <param name="index">Index in the list to paste into, make sure the index is not out of bounds</param>
    void Paste(IList<EditableCondition> conditionsToPasteInto, int index = -1);

    /// <summary>
    /// Determines if the conditions clipboard can be pasted.
    /// </summary>
    bool CanPaste { get; }
}
