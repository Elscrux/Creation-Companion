using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public interface IRecordEditorFactory {
    /// <summary>
    /// Create a record editor for a given record
    /// </summary>
    /// <param name="record">Record to create a record editor for</param>
    /// <param name="control">Out variable for the record editor control</param>
    /// <param name="recordEditorVm">Out variable for the record editor vm</param>
    /// <returns>True if the record editor could be created, otherwise false</returns>
    bool FromType(IMajorRecord record, [MaybeNullWhen(false)] out Control control, [MaybeNullWhen(false)] out IRecordEditorVM recordEditorVm);
}
