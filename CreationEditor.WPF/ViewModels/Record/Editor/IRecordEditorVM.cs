using System.Reactive;
using System.Windows.Controls;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.WPF.ViewModels.Record;

public interface IRecordEditorVM {
    public IMajorRecordGetter Record { get; }
}

public interface ISubRecordEditorVM<TRecord> : IRecordEditorVM {
    public new TRecord Record { get; set; }
}

public interface IRecordEditorVM<TMajorRecord, TMajorRecordGetter> : ISubRecordEditorVM<TMajorRecord>, IDisposable
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    
    public ReactiveCommand<Unit, Unit> Save { get; }

    public UserControl CreateUserControl(TMajorRecord record);
}