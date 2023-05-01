using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM : ViewModel, IRecordListVM {
    IRecordProvider IRecordListVM.RecordProvider => RecordProvider;
    public IRecordProvider RecordProvider { get; }
    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();

    public IEnumerable? Records { get; }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Unit> OpenReferences { get; }

    public Func<StyledElement, IFormLinkIdentifier> GetFormLink { get; }

    public RecordListVM(
        IRecordProvider recordProvider,
        IRecordListFactory recordListFactory,
        MainWindow mainWindow) {
        RecordProvider = recordProvider;

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
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInInProgressMarker(
                x => x.Filter(RecordProvider.Filter, false),
                out var isFiltering)
            .ToObservableCollection(this);

        IsBusy = isFiltering
            .CombineLatest(
                RecordProvider.IsBusy,
                (filtering, busy) => (Filtering: filtering, Busy: busy))
            .ObserveOnGui()
            .Select(list => list.Filtering || list.Busy);

        GetFormLink = element => {
            if (element.DataContext is not IReferencedRecord referencedRecord) return FormLinkInformation.Null;

            return FormLinkInformation.Factory(referencedRecord.Record);
        };

        ContextMenuItems.AddRange(RecordProvider.ContextMenuItems);
        ContextMenuItems.Add(new MenuItem { Header = "Open References", Command = OpenReferences });
    }

    public override void Dispose() {
        base.Dispose();

        RecordProvider.Dispose();
    }
}
