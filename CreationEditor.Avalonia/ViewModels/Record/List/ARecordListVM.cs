using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public abstract class ARecordListVM : ViewModel, IRecordListVM {
    protected readonly IReferenceQuery ReferenceQuery;
    protected readonly IRecordController RecordController;

    public abstract Type Type { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public IEnumerable<IReferencedRecord> Records { get; }
    protected readonly SourceCache<IReferencedRecord, FormKey> RecordCache = new(x => x.Record.FormKey);

    [Reactive] public bool IsBusy { get; set; }

    protected ARecordListVM(
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery, 
        IRecordController recordController) {
        ReferenceQuery = referenceQuery;
        RecordController = recordController;
        RecordBrowserSettings = recordBrowserSettings;

        Records = RecordCache
            .Connect()
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Filter(this.WhenAnyValue(x => x.RecordBrowserSettings.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .Select(_ => new Func<IReferencedRecord, bool>(record => RecordBrowserSettings.Filter(record.Record))))
            .DoOnGuiAndSwitchBack(_ => IsBusy = false)
            .ToObservableCollection(this);
    }
}
