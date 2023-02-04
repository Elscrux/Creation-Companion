using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public class RecordListVM : ViewModel, IRecordListVM {
    IRecordProvider IRecordListVM.RecordProvider => RecordProvider;
    public IRecordProvider RecordProvider { get; }
    
    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();
    
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }

    public IEnumerable Records { get; }

    [Reactive] public bool IsBusy { get; set; }
    [Reactive] private bool IsBusyInternal { get; set; }
    
    public ReactiveCommand<Unit, Unit> OpenReferences { get; }

    public RecordListVM(
        IRecordProvider recordProvider,
        IRecordListFactory recordListFactory,
        MainWindow mainWindow) {
        RecordProvider = recordProvider;
        DoubleTapCommand = recordProvider.DoubleTapCommand;

        this.WhenAnyValue(
                x => x.RecordProvider.IsBusy,
                x => x.IsBusyInternal,
                (isBusy, isBusyInternal) => (IsBusy: isBusy, IsBusyInternal: isBusyInternal))
            .ObserveOnGui()
            .Subscribe(busyStates => IsBusy = busyStates.IsBusy || busyStates.IsBusyInternal);
        
        OpenReferences = ReactiveCommand.Create(() => {
                if (RecordProvider.SelectedRecord == null) return;

                var fromIdentifiers = recordListFactory.FromIdentifiers(RecordProvider.SelectedRecord.References);
                var referenceWindow = new ReferenceWindow(RecordProvider.SelectedRecord.Record) {
                    Content = fromIdentifiers
                };

                referenceWindow.Show(mainWindow);
            },
            this.WhenAnyValue(x => x.RecordProvider.SelectedRecord).Select(selected => selected is { References.Count: > 0 }));

        Records = RecordProvider.RecordCache
            .Connect()
            .DoOnGuiAndSwitchBack(_ => IsBusyInternal = true)
            .Filter(RecordProvider.Filter)
            .DoOnGuiAndSwitchBack(_ => IsBusyInternal = false)
            .ToObservableCollection(this);
        
        ContextMenuItems.AddRange(RecordProvider.ContextMenuItems);
        ContextMenuItems.Add(new MenuItem { Header = "Open References", Command = OpenReferences });
    }
}
