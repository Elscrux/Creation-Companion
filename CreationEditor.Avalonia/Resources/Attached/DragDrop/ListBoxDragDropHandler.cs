using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FluentAvalonia.Core;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public sealed class ListBoxDragDropHandler : DragDropHandler<ListBox, ListBoxItem> {
    protected override IEnumerable GetItems(ListBox control) {
        return control.ItemsSource ?? throw new InvalidOperationException(
            "ListBox does not have ItemsSource set.");
    }
    protected override IList GetSelectedItems(ListBox control) {
        return control.SelectedItems ?? throw new InvalidOperationException(
            "ListBox does not have SelectedItems set. Ensure you are using a SelectionMode that supports selection.");
    }
    protected override int IndexOfElement(ListBox control, ListBoxItem element) => control.ItemsSource.IndexOf(element.DataContext);
    protected override IEnumerable<ListBoxItem> GetElements(ListBox control) {
        return control
            .GetLogicalChildren()
            .OfType<ListBoxItem>();
    }
    protected override ListBoxItem? GetElementAt(ListBox control, int index) {
        return control
            .GetLogicalChildren()
            .OfType<ListBoxItem>()
            .FirstOrDefault(i => control.ItemsSource.IndexOf(i.DataContext) == index);
    }
    protected override ListBoxItem? GetElement(ListBox control, object? item) {
        return control
            .GetLogicalChildren()
            .OfType<ListBoxItem>()
            .FirstOrDefault(i => i.DataContext == item);
    }
    protected override ListBoxItem? GetLastElement(ListBox control) {
        if (control.ItemsSource is not IList list) return null;

        return control
            .GetLogicalChildren()
            .OfType<ListBoxItem>()
            .MaxBy(i => list.IndexOf(i.DataContext));
    }
}
