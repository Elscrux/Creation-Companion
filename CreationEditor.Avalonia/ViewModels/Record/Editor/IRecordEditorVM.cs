using CreationEditor.Avalonia.Services.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface IRecordEditorVM {
    IRecordEditorCore Core { get; }
}

public interface IRecordEditorVM<TMajorRecord, TMajorRecordGetter> : IRecordEditorVM
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter;
