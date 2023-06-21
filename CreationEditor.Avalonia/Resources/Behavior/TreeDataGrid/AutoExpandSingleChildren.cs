using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
namespace CreationEditor.Avalonia.Behavior.TreeDataGrid;

public sealed class AutoExpandSingleChildren<T> : Behavior<global::Avalonia.Controls.TreeDataGrid> where T : class {
    protected override void OnAttachedToVisualTree() {
        if (AssociatedObject == null) return;

        AssociatedObject.CellPrepared += AutoExpand;
    }
    private void AutoExpand(object? sender, TreeDataGridCellEventArgs e) {
        if (AssociatedObject is not { Rows: {} rows }) return;
        if (e.Cell is not TreeDataGridExpanderCell { IsExpanded: false, Model: ExpanderCell<T> expanderCell }) return;

        // Search for parent row
        foreach (var row in rows.OfType<HierarchicalRow<T>>()) {
            // Parent must have exactly one child
            if (row.Children is not { Count: 1 }) continue;

            // Parent must have the same model as the expander cell
            var parentRow = row.Children?.FirstOrDefault(c => ReferenceEquals(c.Model, expanderCell.Value));
            if (parentRow == null) continue;

            expanderCell.Row.IsExpanded = true;
            return;
        }
    }

    protected override void OnDetachedFromVisualTree() {
        if (AssociatedObject == null) return;

        AssociatedObject.CellPrepared -= AutoExpand;
    }
}
