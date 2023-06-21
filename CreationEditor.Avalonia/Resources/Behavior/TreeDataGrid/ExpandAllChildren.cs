using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
namespace CreationEditor.Avalonia.Behavior.TreeDataGrid;

public sealed class ExpandAllChildren<T> : Behavior<global::Avalonia.Controls.TreeDataGrid> where T : class {
    protected override void OnAttachedToVisualTree() {
        if (AssociatedObject is null) return;

        AssociatedObject.Tapped += Expand;
    }

    protected override void OnDetachedFromVisualTree() {
        if (AssociatedObject == null) return;

        AssociatedObject.Tapped -= Expand;
    }

    private void Expand(object? sender, TappedEventArgs e) {
        if (AssociatedObject == null) return;

        // Check if Alt is pressed
        if ((e.KeyModifiers & KeyModifiers.Alt) == 0) return;

        // Retrieve row that was pressed
        if (e.Source is not StyledElement styledElement) return;

        var dataContext = styledElement.DataContext;
        var row = AssociatedObject.Source?.Rows.FirstOrDefault(x => ReferenceEquals(x.Model, dataContext));
        if (row is not HierarchicalRow<T> hierarchicalRow) return;

        // Expand self and all children
        foreach (var expanderRow in hierarchicalRow.GetAllChildren(x => x.Children, true)) {
            expanderRow.IsExpanded = true;
        }
    }
}
