using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.ViewModels.Reference;
namespace CreationEditor.Avalonia.Views.Reference;

public partial class ReferenceBrowser : ReactiveUserControl<ReferenceBrowserVM> {
    public ReferenceBrowser() {
        InitializeComponent();
    }

    public ReferenceBrowser(ReferenceBrowserVM vm) : this() {
        ViewModel = vm;
    }

    private void ReferenceDataGrid_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (e.Source is not Visual visual) return;

        var expanderCell = visual.FindAncestorOfType<TreeDataGridExpanderCell>();
        if (expanderCell != null) {
            expanderCell.IsExpanded = !expanderCell.IsExpanded;
        }
    }

    private void ReferenceDataGrid_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        ViewModel?.ContextMenu(sender, e);
    }
}
