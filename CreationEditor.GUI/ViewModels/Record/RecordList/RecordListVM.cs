using System;
using System.Collections;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using CreationEditor.GUI.Views.Controls.Record.RecordList;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record.RecordList;

public interface IRecordListVM {
    public UserControl View { get; set; }
    
    public Type Type { get; set; }
    [Reactive] public ILinkCache Scope { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
}

public class MajorRecordListVM : ViewModel, IRecordListVM {
    public UserControl View { get; set; }
    
    public Type Type { get; set; }
    [Reactive] public ILinkCache Scope { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
    
    public MajorRecordListVM(ILinkCache scope, Type type) {
        Scope = scope;
        Type = type;
        View = new GenericRecordList(this);

        Records = this.WhenAnyValue(x => x.Scope, x => x.Type)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                return Observable.Create<IMajorRecordIdentifier>((obs, cancel) => {
                    foreach (var recordIdentifier in x.Item1.AllIdentifiers(x.Item2)) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;
                        obs.OnNext(recordIdentifier);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);
    }
}


public abstract class RecordListVM<TMajorRecordGetter> : ViewModel, IRecordListVM
    where TMajorRecordGetter : class, IMajorRecordGetter {
    public UserControl View { get; set; } = null!;
    
    public Type Type { get; set; }
    [Reactive] public ILinkCache Scope { get; set; }
    [Reactive] public IEnumerable Records { get; set; }

    protected RecordListVM(ILinkCache scope) {
        Scope = scope;
        Type = typeof(TMajorRecordGetter);

        Records = this
            .WhenAnyValue(x => x.Scope)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(linkCache => {
                return Observable.Create<IMajorRecordIdentifier>((obs, cancel) => {
                    foreach (var recordIdentifier in linkCache.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;
                        obs.OnNext(recordIdentifier);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);
    }
}