using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Query;
using QueryPlugin.Views;
using ReactiveUI;
namespace QueryPlugin.ViewModels;

public sealed class QueryPluginVM : ViewModel {
    /// <summary>
    /// Used to pad the right side of the grid so that the splitter can be dragged at the most right column.
    /// </summary>
    private readonly Control _paddingRight = new StackPanel();

    private readonly Func<QueryColumnVM> _queryColumnVMFactory;
    private readonly IMenuItemProvider _menuItemProvider;

    public Grid ColumnsGrid { get; } = new() {
        ColumnDefinitions = [new ColumnDefinition()]
    };

    public ReactiveCommand<Unit, Unit> AddColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> SaveColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> DuplicateColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> DeleteColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> CopyColumnText { get; }

    public QueryPluginVM(
        Func<QueryColumnVM> queryColumnVMFactory,
        IMenuItemProvider menuItemProvider,
        IQueryState queryState,
        MainWindow window) {
        _queryColumnVMFactory = queryColumnVMFactory;
        _menuItemProvider = menuItemProvider;
        ColumnsGrid.Children.Add(_paddingRight);

        AddColumn = ReactiveCommand.Create(() => {
            InsertColumn(ColumnsGrid.ColumnDefinitions.Count - 1);
        });

        SaveColumn = ReactiveCommand.Create<QueryColumnVM>(vm => {
            var queryRunner = vm.QueryVM.QueryRunner;
            if (queryRunner is null) return;

            queryState.Save(queryRunner);
        });

        DuplicateColumn = ReactiveCommand.Create<QueryColumnVM>(vm => {
            if (!GetColumnIndex(vm, out var columnIndex)) return;

            var insertedColumn = InsertColumn(columnIndex + 2);
            if (insertedColumn.ViewModel is null) return;

            insertedColumn.ViewModel.Name = vm.Name;
            insertedColumn.ViewModel.QueryVM.QueryRunner.RestoreMemento(vm.QueryVM.QueryRunner.CreateMemento());
        });

        DeleteColumn = ReactiveCommand.Create<QueryColumnVM>(vm => {
            if (GetColumnIndex(vm, out var columnIndex)) {
                RemoveColumn(columnIndex);
            }
        });

        CopyColumnText = ReactiveCommand.Create<QueryColumnVM>(vm => {
            var clipboard = TopLevel.GetTopLevel(window)?.Clipboard;

            var text = string.Join(Environment.NewLine, vm.QueriedFields);
            clipboard?.SetTextAsync(text);
        });
    }

    private QueryColumn InsertColumn(int column) {
        // Move all other columns to the right
        foreach (var child in ColumnsGrid.Children) {
            var childColumn = Grid.GetColumn(child);
            if (childColumn >= column) {
                Grid.SetColumn(child, childColumn + 2);
            }
        }

        // Add the new query column
        ColumnsGrid.ColumnDefinitions.Insert(column, new ColumnDefinition(new GridLength(200)));

        var queryColumnVM = _queryColumnVMFactory();
        queryColumnVM.MenuItems = GetColumnMenuItems(queryColumnVM);

        var queryColumn = new QueryColumn(queryColumnVM) {
            [Grid.ColumnProperty] = column,
        };
        ColumnsGrid.Children.Add(queryColumn);

        // Add a splitter
        column++;
        ColumnsGrid.ColumnDefinitions.Insert(column, new ColumnDefinition(new GridLength(2.5)));
        ColumnsGrid.Children.Add(new GridSplitter {
            [Grid.ColumnProperty] = column,
            MinWidth = 2.5,
        });

        _paddingRight[Grid.ColumnProperty] = column + 1;

        return queryColumn;
    }

    private void RemoveColumn(int column) {
        // Remove column
        ColumnsGrid.ColumnDefinitions.RemoveAt(column);

        // Remove splitter
        ColumnsGrid.ColumnDefinitions.RemoveAt(column);

        // Move all other columns to the left
        var removeChildren = new List<Control>();
        foreach (var child in ColumnsGrid.Children) {
            var childColumn = Grid.GetColumn(child);

            if (childColumn == column || childColumn == column + 1) {
                removeChildren.Add(child);
            } else if (childColumn >= column) {
                Grid.SetColumn(child, childColumn - 2);
            }
        }

        ColumnsGrid.Children.RemoveAll(removeChildren);
    }

    private bool GetColumnIndex(QueryColumnVM queryColumnVM, out int index) {
        var control = ColumnsGrid.Children.FirstOrDefault(control => control.DataContext == queryColumnVM);
        if (control is null) {
            index = -1;
            return false;
        }

        index = Grid.GetColumn(control);
        return true;
    }

    private MenuItem[] GetColumnMenuItems(QueryColumnVM queryColumnVM) {
        return [
            _menuItemProvider.Save(SaveColumn, queryColumnVM),
            _menuItemProvider.Duplicate(DuplicateColumn, queryColumnVM),
            _menuItemProvider.Delete(DeleteColumn, queryColumnVM),
            _menuItemProvider.Copy(CopyColumnText, queryColumnVM).Name("Copy Entries Text")
        ];
    }
}
