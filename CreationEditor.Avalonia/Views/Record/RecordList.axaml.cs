using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Mutagen.References.Record;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Record;

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public static readonly StyledProperty<IList<DataGridColumn>?> ColumnsProperty
        = AvaloniaProperty.Register<RecordList, IList<DataGridColumn>?>(nameof(Columns));

    public IList<DataGridColumn>? Columns {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public RecordList() {
        InitializeComponent();

        this.WhenActivated(x => {
            this.WhenAnyValue(list => list.ViewModel!.RecordProvider.SelectedRecord)
                .Subscribe(ScrollToItem)
                .DisposeWith(x);
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ColumnsProperty) {
            RecordGrid.Columns.Clear();

            if (Columns is not null) {
                RecordGrid.Columns.AddRange(Columns);
                Sort();
            }
        }
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
