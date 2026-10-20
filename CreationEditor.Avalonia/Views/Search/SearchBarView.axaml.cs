using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.ViewModels.Search;
namespace CreationEditor.Avalonia.Views.Search;

public partial class SearchBarView : UserControl {
    private SearchVM? SearchVM => DataContext as SearchVM;

    public SearchBarView() {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e) {
        base.OnDataContextChanged(e);

        if (SearchVM is null) return;

        SearchVM.OpenRequested
            .Subscribe(_ => OpenSearch());
        SearchVM.CloseRequested
            .Subscribe(_ => CloseSearch());
    }

    private void OpenSearch() {
        SearchBox.Focus();
        ResultsPopup.IsOpen = true;
    }

    private void CloseSearch() {
        ResultsPopup.IsOpen = false;
        SearchBox.Text = null;
    }

    private void OnSearchBoxTextChanged(object? sender, TextChangedEventArgs e) {
        if (SearchBox.IsFocused) ResultsPopup.IsOpen = true;
    }

    private void OnSearchBoxGotFocus(object? sender, RoutedEventArgs e) {
        ResultsPopup.IsOpen = true;
    }

    private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e) {
        switch (e.Key) {
            case Key.Down:
                SearchVM?.MoveSelection(1);
                ScrollSelectedIntoView();
                e.Handled = true;
                break;

            case Key.Up:
                SearchVM?.MoveSelection(-1);
                ScrollSelectedIntoView();
                e.Handled = true;
                break;

            case Key.Enter:
                SearchVM?.ExecuteSelected();
                e.Handled = true;
                break;

            case Key.Escape:
                CloseSearch();
                e.Handled = true;
                break;
        }
    }

    private void OnResultItemClicked(object? sender, RoutedEventArgs e) {
        if (sender is Button { DataContext: SearchItemVM item }) {
            SearchVM?.ExecuteItem(item);
        }
    }

    private void OnClearRecentsClicked(object? sender, RoutedEventArgs e) {
        SearchVM?.ClearRecents();
    }

    private void OnContextActionsClicked(object? sender, RoutedEventArgs e) {
        if (sender is not Button button) return;
        if (button.DataContext is not SearchItemVM item) return;

        var menuItems = SearchVM?.GetContextMenuItems(item)?.ToList();
        if (menuItems is not { Count: > 0 }) return;

        var menu = new ContextMenu {
            ItemsSource = menuItems,
            PlacementTarget = button,
        };
        menu.Open(button);

        e.Handled = true;
    }

    private void ScrollSelectedIntoView() {
        var item = SearchVM?.SelectedItem;
        if (item is null) return;

        FindContainer(this, item)?.BringIntoView();
    }

    private static Control? FindContainer(Visual root, object data) {
        foreach (var child in root.GetVisualChildren()) {
            if (child is Control { DataContext: not null } control && Equals(control.DataContext, data)) return control;

            var result = FindContainer(child, data);
            if (result is not null) return result;
        }

        return null;
    }
}
