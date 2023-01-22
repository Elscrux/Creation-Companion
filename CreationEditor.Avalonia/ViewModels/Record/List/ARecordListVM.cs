using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public abstract class ARecordListVM<TReferenced> : ViewModel, IRecordListVM
    where TReferenced : IReferencedRecordIdentifier {
    protected readonly IReferenceQuery ReferenceQuery;
    protected readonly IRecordController RecordController;

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    
    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();
    [Reactive] public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; protected init; }

    public IEnumerable Records { get; }
    protected readonly SourceCache<TReferenced, FormKey> RecordCache = new(x => x.Record.FormKey);

    [Reactive] public bool IsBusy { get; set; }
    
    [Reactive] public TReferenced? SelectedRecord { get; set; }

    public ReactiveCommand<Unit, Unit> OpenUseInfo { get; }

    protected Func<TReferenced, bool>? CustomFilter;

    protected ARecordListVM(
        MainWindow mainWindow,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery, 
        IRecordController recordController) {
        ReferenceQuery = referenceQuery;
        RecordController = recordController;
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        OpenUseInfo = ReactiveCommand.Create(() => {
                if (SelectedRecord == null) return;

                var referenceWindow = new ReferenceWindow(SelectedRecord.Record) {
                    Content = recordListFactory.FromIdentifiers(SelectedRecord.References)
                };

                referenceWindow.Show(mainWindow);
            },
            this.WhenAnyValue(x => x.SelectedRecord).Select(selected => selected is { References.Count: > 0 }));

        Records = RecordCache
            .Connect()
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Filter(RecordBrowserSettingsVM.SettingsChanged
                .Select(_ => new Func<TReferenced, bool>(record => RecordBrowserSettingsVM.Filter(record.Record) && (CustomFilter == null || CustomFilter(record)))))
            .DoOnGuiAndSwitchBack(_ => IsBusy = false)
            .ToObservableCollection(this);
        
        ContextMenuItems.Add(new MenuItem { Header = "Open Use Info", Command = OpenUseInfo });
    }
}
