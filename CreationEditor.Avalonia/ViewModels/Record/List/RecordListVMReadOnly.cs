using System.Reactive.Linq;
using CreationEditor.Avalonia.Extension;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Environment;
using DynamicData;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVMReadOnly : RecordListVM {
    public RecordListVMReadOnly(
        Type type,
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery, 
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordController) {
        Type = type;

        Records = this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm, x => x.Type)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Where(x => x.Item1.ListedOrder.Count > 0)
            .Do(_ => IsBusy = true)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                return Observable.Create<ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>>((obs, cancel) => {
                    foreach (var recordIdentifier in x.Item1.AllIdentifiers(x.Item3)) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;

                        //Skip when browser settings don't match
                        if (!RecordBrowserSettings.Filter(recordIdentifier)) continue;

                        var formLinks = ReferenceQuery.GetReferences(recordIdentifier.FormKey, RecordBrowserSettings.LinkCache);
                        var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(recordIdentifier, formLinks);

                        obs.OnNext(referencedRecord);
                    }

                    obs.OnCompleted();
                    return Task.CompletedTask;
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => IsBusy = false)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .ToObservableCollection(this);
    }
}
