using System.Collections;
using System.Reactive.Linq;
using System.Windows.Controls;
using CreationEditor.GUI.Models.Record;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Views.Record;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using MutagenLibrary.References.ReferenceCache;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record;

public class MajorRecordListVM : ViewModel, IRecordListVM {
    private readonly IReferenceQuery _referenceQuery;
    public UserControl View { get; set; }
    
    public Type Type { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }
    
    [Reactive] public bool IsBusy { get; set; }

    public MajorRecordListVM(Type type, IRecordBrowserSettings recordBrowserSettings, IReferenceQuery referenceQuery) {
        _referenceQuery = referenceQuery;
        Type = type;
        RecordBrowserSettings = recordBrowserSettings;
        View = new RecordList(this);

        Records = this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm, x => x.Type)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                IsBusy = true;
                return Observable.Create<ReferencedRecord<IMajorRecordIdentifier>>((obs, cancel) => {
                    try {
                        foreach (var recordIdentifier in x.Item1.AllIdentifiers(x.Item3)) {
                            if (cancel.IsCancellationRequested) return Task.CompletedTask;

                            //Skip when browser settings don't match
                            if (!RecordBrowserSettings.Filter(recordIdentifier)) continue;

                            var formLinks = _referenceQuery.GetReferences(recordIdentifier.FormKey, RecordBrowserSettings.LinkCache);
                            var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier>(recordIdentifier, formLinks);

                            obs.OnNext(referencedRecord);
                        }

                        obs.OnCompleted();
                        return Task.CompletedTask;
                    } finally {
                        IsBusy = false;
                    }
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);
    }
}

public abstract class RecordListVM<TMajorRecordGetter> : ViewModel, IRecordListVM
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly IReferenceQuery _referenceQuery;
    public UserControl View { get; set; } = null!;
    
    public Type Type { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }
    
    [Reactive] public bool IsBusy { get; set; }
    
    protected RecordListVM(IRecordBrowserSettings recordBrowserSettings, IReferenceQuery referenceQuery) {
        _referenceQuery = referenceQuery;
        RecordBrowserSettings = recordBrowserSettings;
        Type = typeof(TMajorRecordGetter);

        Records = this
            .WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                IsBusy = true;
                return Observable.Create<ReferencedRecord<TMajorRecordGetter>>((obs, cancel) => {
                    try {
                        foreach (var recordIdentifier in x.Item1.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                            if (cancel.IsCancellationRequested) return Task.CompletedTask;

                            //Skip when browser settings don't match
                            if (!RecordBrowserSettings.Filter(recordIdentifier)) continue;

                            var formLinks = _referenceQuery.GetReferences(recordIdentifier.FormKey, RecordBrowserSettings.LinkCache);
                            var referencedRecord = new ReferencedRecord<TMajorRecordGetter>(recordIdentifier, formLinks);

                            obs.OnNext(referencedRecord);
                        }
                        obs.OnCompleted();
                        return Task.CompletedTask;
                    } finally {
                        IsBusy = false;
                    }
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);
    }
}