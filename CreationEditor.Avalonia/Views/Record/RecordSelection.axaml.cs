using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Avalonia.Views.Record;

public partial class RecordSelection : ReactiveUserControl<RecordSelectionVM> {
    public RecordSelection() {
        InitializeComponent();
    }

    public RecordSelection(RecordSelectionVM vm) : this() {
        DataContext = vm;
    }
}
