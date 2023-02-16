using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins;
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
    [Reactive] private bool IsFiltering { get; set; }
    
    public ReactiveCommand<Unit, Unit> OpenReferences { get; }
    
    public Func<StyledElement, IFormLinkIdentifier> GetFormLink { get; }

    public RecordListVM(
        IRecordProvider recordProvider,
        IRecordListFactory recordListFactory,
        MainWindow mainWindow) {
        RecordProvider = recordProvider;
        DoubleTapCommand = recordProvider.DoubleTapCommand;

        this.WhenAnyValue(
                x => x.RecordProvider.IsBusy,
                x => x.IsFiltering,
                (isBusy, isFiltering) => (IsBusy: isBusy, IsFiltering: isFiltering))
            .ObserveOnGui()
            .Subscribe(busyStates => IsBusy = busyStates.IsBusy || busyStates.IsFiltering);
        
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
            .DoOnGuiAndSwitchBack(_ => IsFiltering = true)
            .Filter(RecordProvider.Filter)
            .DoOnGuiAndSwitchBack(_ => IsFiltering = false)
            .ToObservableCollection(this);

        GetFormLink = element => {
            if (element.DataContext is not IReferencedRecord referencedRecord) return FormLinkInformation.Null;

            return FormLinkInformation.Factory(referencedRecord.Record);
        };
        
        ContextMenuItems.AddRange(RecordProvider.ContextMenuItems);
        ContextMenuItems.Add(new MenuItem { Header = "Open References", Command = OpenReferences });
    }
}
