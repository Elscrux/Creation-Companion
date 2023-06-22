using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Threading;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed class ReferenceRemapperVM : ViewModel {
    public IEditorEnvironment EditorEnvironment { get; }
    public object? Context { get; }
    public IReferencedRecord? ReferencedRecordContext { get; }

    public bool ContextCanBeRemapped { get; }
    public Type? ContextType { get; }
    public IList<Type>? ScopedTypes { get; }

    [Reactive] public bool IsRemapping { get; set; }
    public Subject<Unit> ShowReferenceRemapDialog { get; } = new();

    public ReactiveCommand<FormKey, Unit> RemapReferences { get; }

    public ReferenceRemapperVM(
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        object? context = null) {
        EditorEnvironment = editorEnvironment;
        Context = context;

        if (context is IReferencedRecord referencedRecord) {
            ReferencedRecordContext = referencedRecord;
            ContextCanBeRemapped = true;
            ContextType = referencedRecord.Record.Registration.GetterType;
            ScopedTypes = ContextType.AsEnumerable().ToArray();
        }

        RemapReferences = ReactiveCommand.Create<FormKey>(formKey => {
            if (ReferencedRecordContext is null || ContextType is null) return;
            if (!editorEnvironment.LinkCache.TryResolve(formKey, ContextType, out var record)) return;

            IsRemapping = true;
            Task.Run(() => {
                recordController.ReplaceReferences(ReferencedRecordContext, record);
                Dispatcher.UIThread.Post(() => IsRemapping = false);
            });
        });
    }

    public void Remap() {
        if (ContextCanBeRemapped) {
            ShowReferenceRemapDialog.OnNext(Unit.Default);
        }
    }
}
