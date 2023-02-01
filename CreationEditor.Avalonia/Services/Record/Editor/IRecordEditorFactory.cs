using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Editor; 

public interface IRecordEditorFactory {
    public bool FromType(IMajorRecord record, [MaybeNullWhen(false)] out Control control, [MaybeNullWhen(false)] out IRecordEditorVM recordEditor);
}
