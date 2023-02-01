using Avalonia.Controls;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Editor; 

public interface IRecordEditorFactory {
    public Control? FromType(IMajorRecord record);
}
