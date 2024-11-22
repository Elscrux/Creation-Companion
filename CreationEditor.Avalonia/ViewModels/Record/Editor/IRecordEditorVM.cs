using System.Reactive;
using Avalonia.Controls;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface IRecordEditorVM : IDisposableDropoff {
    IMajorRecordGetter Record { get; }

    ReactiveCommand<Unit, Unit> Save { get; }
}

public interface IRecordEditorVM<TMajorRecord, TMajorRecordGetter> : ISubRecordEditorVM<TMajorRecord>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {

    Control CreateControl(TMajorRecord record);
}
