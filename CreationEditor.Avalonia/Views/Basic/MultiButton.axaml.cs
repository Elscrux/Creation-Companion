using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
namespace CreationEditor.Avalonia.Views.Basic;

[TemplatePart(ItemsControlPartName, typeof(ItemsControl))]
public class MultiButton : SelectingItemsControl {
    private const string ItemsControlPartName = "PART_ItemsControl";

    private ItemsControl _itemsControl = null!;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _itemsControl = e.NameScope.Get<ItemsControl>(ItemsControlPartName);
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e) {
        base.OnPointerPressed(e);

        if (e.Source is not Control source) return;

        var point = e.GetCurrentPoint(source);
        if (!point.Properties.IsLeftButtonPressed) return;

        var listBoxItem = source.FindAncestorOfType<ListBoxItem>();
        if (listBoxItem is null) return;

        var isAlreadySelected = ReferenceEquals(SelectedItem, listBoxItem.Content);
        if (isAlreadySelected) {
            SetIsSelected(listBoxItem, false);
            SelectedItem = SelectedValue = null;
            SelectedIndex = -1;
        } else {
            if (SelectedIndex != -1) {
                var oldControl = _itemsControl.ContainerFromIndex(SelectedIndex).FindDescendantOfType<ListBoxItem>();
                if (oldControl is not null) {
                    SetIsSelected(oldControl, false);
                }
            }

            SelectedItem = SelectedValue = listBoxItem.Content;
            SelectedIndex = _itemsControl.Items.IndexOf(SelectedValue);

            SetIsSelected(listBoxItem, true);
        }
    }
}
