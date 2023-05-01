using System.Reactive;
using Avalonia.Controls;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface IRecordEditorVM : IDisposableDropoff {
    public IMajorRecordGetter Record { get; }

    public ReactiveCommand<Unit, Unit> Save { get; }
}

public interface IRecordEditorVM<TMajorRecord, TMajorRecordGetter> : ISubRecordEditorVM<TMajorRecord>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {

    public Control CreateControl(TMajorRecord record);
}
