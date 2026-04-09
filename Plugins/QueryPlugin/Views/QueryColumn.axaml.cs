using Avalonia.Controls;
using CreationEditor;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Services.Query;
using QueryPlugin.ViewModels;
using ReactiveUI.Avalonia;
namespace QueryPlugin.Views;

public partial class QueryColumn : ReactiveUserControl<QueryColumnVM> {
    public QueryColumn() {
        InitializeComponent();
    }

    public QueryColumn(QueryColumnVM vm) : this() {
        DataContext = vm;
    }

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        if (ViewModel is null) return;
        if (e.Source is not Control { DataContext: QueryResult queryResult } control ) return;

        var modKey = ViewModel.LinkCacheProvider.LinkCache.GetWinningOverrideModKey(queryResult.Record);
        using var disposable = ViewModel.ReferenceService.GetReferencedRecord(queryResult.Record, out var record);
        var recordContext = new RecordContext(modKey, record);
        var menuItems = ViewModel.ContextMenuProvider.GetMenuItems(new SelectedListContext([recordContext], [])).ToArray();

        var contextFlyout = new MenuFlyout();
        foreach (var menuItem in menuItems) {
            contextFlyout.Items.Add(menuItem);
        }

        contextFlyout.ShowAt(control, true);
    }
}
