using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData.Binding;
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

            RecordGrid.Columns.ObserveCollectionChanges()
                .Subscribe(_ => Sort());
        });
    }

    public RecordList(IRecordListVM vm) : this() {
        ViewModel = vm;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        Sort();
    }

    private void ScrollToItem(IReferencedRecord? referencedRecord) {
        if (RecordGrid is null || referencedRecord is null) return;

        RecordGrid.SelectedItem = referencedRecord;
        RecordGrid.ScrollIntoView(RecordGrid.SelectedItem, RecordGrid.Columns.First());
    }

    private void Sort() {
        if (!RecordGrid.Columns.Any()) return;

        var column = RecordGrid.Columns.First();
        column.ClearSort();
        column.Sort();
    }

    private void RecordGrid_ContextRequested(object? sender, ContextRequestedEventArgs e) {
        if (ViewModel is null) return;
        if (e.Source is not Control control) return;
        if (sender is not DataGrid dataGrid) return;
        if (dataGrid.SelectedItems is null) return;

        var selectedRecords = dataGrid.SelectedItems
            .OfType<IReferencedRecord>()
            .ToList();

        var recordListContext = ViewModel.GetRecordListContext(selectedRecords);
        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel?.RecordContextMenuProvider.GetMenuItems(recordListContext)
        };

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
    }

    private void RecordGrid_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (ViewModel is null) return;
        if (sender is not DataGrid dataGrid) return;
        if (dataGrid.SelectedItems is null) return;

        var selectedRecords = dataGrid.SelectedItems
            .OfType<IReferencedRecord>()
            .ToList();

        var recordListContext = ViewModel.GetRecordListContext(selectedRecords);
        using var disposable = ViewModel.PrimaryCommand.Execute(recordListContext).Subscribe();

        e.Handled = true;
    }

    private void RecordGrid_OnKeyDown(object? sender, KeyEventArgs e) {
        if (ViewModel is null) return;
        if (sender is not DataGrid dataGrid) return;
        if (dataGrid.SelectedItems is null) return;

        var keyGesture = new KeyGesture(e.Key, e.KeyModifiers);
        ViewModel.RecordContextMenuProvider.TryToExecuteHotkey(keyGesture, () => {
            var selectedRecords = dataGrid.SelectedItems
                .OfType<IReferencedRecord>()
                .ToList();

            return ViewModel.GetRecordListContext(selectedRecords);
        });

        e.Handled = true;
    }
}
