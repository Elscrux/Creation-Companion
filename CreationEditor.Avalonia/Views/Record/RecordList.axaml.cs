using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Mutagen.References;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Record;

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public RecordList() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(list => list.ViewModel!.SelectedRecord)
                .Subscribe(ScrollToItem)
                .DisposeWithComposite(disposables);

            this.WhenAnyValue(list => list.ViewModel!.Records)
                .Subscribe(records => RecordGrid.SelectedItem = records?.OfType<IReferencedRecord>().FirstOrDefault())
                .DisposeWithComposite(disposables);

            this.WhenAnyValue(list => list.ViewModel!.Columns)
                .SyncTo(RecordGrid.Columns)
                .DisposeWithComposite(disposables);

            RecordGrid.Columns.ObserveCollectionChanges()
                .Subscribe(_ => Sort())
                .DisposeWithComposite(disposables);
        });
    }

    public RecordList(IRecordListVM vm) : this() {
        ViewModel = vm;
    }

    public object? SearchItems(string search) {
        return ViewModel?.Records?
            .OfType<IReferencedRecord>()
            .FirstOrDefault(record =>
                record.Record.EditorID is not null
             && record.Record.EditorID.StartsWith(search, StringComparison.OrdinalIgnoreCase));
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        Sort();
    }

    private void ScrollToItem(IReferencedRecord? referencedRecord) {
        if (RecordGrid is not { Columns: [var firstColumn, ..] } || referencedRecord is null) return;

        RecordGrid.SelectedItem = referencedRecord;
        RecordGrid.ScrollIntoView(RecordGrid.SelectedItem, firstColumn);
    }

    private void Sort() {
        if (RecordGrid is not { Columns: [var firstColumn, ..] } ) return;

        firstColumn.ClearSort();
        firstColumn.Sort();
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
            ItemsSource = ViewModel?.ContextMenuProvider.GetMenuItems(recordListContext),
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
        ViewModel.ContextMenuProvider.TryToExecuteHotkey(
            keyGesture,
            () => {
                var selectedRecords = dataGrid.SelectedItems
                    .OfType<IReferencedRecord>()
                    .ToList();

                return ViewModel.GetRecordListContext(selectedRecords);
            });

        e.Handled = true;
    }
}
