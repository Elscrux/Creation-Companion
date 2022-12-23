using System.Reactive;
using Avalonia.Controls;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface IRecordEditorVM {
    public IMajorRecordGetter Record { get; }
}
public interface IRecordEditorVM<TMajorRecord, TMajorRecordGetter> : ISubRecordEditorVM<TMajorRecord>, IDisposable
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    
    public ReactiveCommand<Unit, Unit> Save { get; }

    public Control CreateControl(TMajorRecord record);
}