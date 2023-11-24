using System.Reactive.Disposables;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Mutagen.References.Record;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Record;

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public RecordList() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(list => list.ViewModel!.SelectedRecord)
                .Subscribe(ScrollToItem)
                .DisposeWith(disposables);

            this.WhenAnyValue(list => list.ViewModel!.Records)
                .Subscribe(records => RecordGrid.SelectedItem = records?.OfType<IReferencedRecord>().FirstOrDefault())
                .DisposeWith(disposables);

            this.WhenAnyValue(list => list.ViewModel!.Columns)
                .SyncTo(RecordGrid.Columns)
                .DisposeWith(disposables);
        });
    }

    private void ScrollToItem(IReferencedRecord? referencedRecord) {
        if (RecordGrid is null || referencedRecord is null) return;

        RecordGrid.SelectedItem = referencedRecord;
        RecordGrid.ScrollIntoView(RecordGrid.SelectedItem, RecordGrid.Columns.First());
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        Sort();
    }

    private void Sort() {
        if (!RecordGrid.Columns.Any()) return;

        RecordGrid.Columns.First().ClearSort();
        RecordGrid.Columns.First().Sort();
    }
}
