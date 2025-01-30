using CreationEditor.Avalonia.Models.Mod.Editor;
using CreationEditor.Services.Mutagen.Record;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public sealed class RecordEditorValidator<TEditableRecord, TMajorRecord, TMajorRecordGetter>
    where TEditableRecord : class, IEditableRecord<TMajorRecord>, TMajorRecord
    where TMajorRecord : class, IMajorRecordInternal, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly IRecordValidator _recordValidator;

    private readonly SourceCache<string, string> _errors = new(static x => x);
    public IObservableCollection<string> Errors { get; set; }

    public RecordEditorValidator(
        IRecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter> vm,
        IRecordValidator recordValidator) {
        _recordValidator = recordValidator;

        Errors = _errors
            .Connect()
            .ToObservableCollection(vm);

        vm.RecordChanged
            .ObserveOnTaskpool()
            .Subscribe(UpdateValidation)
            .DisposeWith(vm);
    }

    private void UpdateValidation(RecordUpdate<TMajorRecordGetter> recordUpdate) {
        var errors = _recordValidator.Validate(recordUpdate.Record).ToList();
        _errors.Edit(updater => {
            updater.Clear();
            foreach (var error in errors) {
                updater.AddOrUpdate(error);
            }
        });
    }
}
