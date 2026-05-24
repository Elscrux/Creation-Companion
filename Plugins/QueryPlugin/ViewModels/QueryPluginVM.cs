using Avalonia.Controls;
using Avalonia.Input.Platform;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Query;
using CreationEditor.Services.State;
using QueryPlugin.Views;
using ReactiveUI.SourceGenerators;
namespace QueryPlugin.ViewModels;

public sealed partial class QueryPluginVM : ViewModel {
    /// <summary>
    /// Used to pad the right side of the grid so that the splitter can be dragged at the most right column.
    /// </summary>
    private readonly Control _paddingRight = new StackPanel();

    private readonly Func<QueryColumnVM> _queryColumnVMFactory;
    private readonly IMenuItemProvider _menuItemProvider;
    private readonly MainWindow _window;

    public Grid ColumnsGrid { get; } = new() {
        ColumnDefinitions = [new ColumnDefinition()],
    };

    public int QueryCount { get; }
    public IStateRepository<QueryRunnerMemento, QueryRunnerMemento, Guid> StateRepository { get; }

    public QueryPluginVM(
        Func<QueryColumnVM> queryColumnVMFactory,
        IStateRepositoryFactory<QueryRunnerMemento, QueryRunnerMemento, Guid> stateRepositoryFactory,
        IMenuItemProvider menuItemProvider,
        MainWindow window) {
        _queryColumnVMFactory = queryColumnVMFactory;
        _menuItemProvider = menuItemProvider;
        _window = window;
        ColumnsGrid.Children.Add(_paddingRight);

        StateRepository = stateRepositoryFactory.Create("Query");

        QueryCount = StateRepository.Count();
    }

    [ReactiveCommand]
    private void AddColumn() {
        InsertColumn(ColumnsGrid.ColumnDefinitions.Count - 1);
    }

    [ReactiveCommand]
    private void SaveColumn(QueryColumnVM vm) {
        var memento = vm.QueryVM.QueryRunner.CreateMemento();
        StateRepository.Save(memento, memento.Id);
    }

    [ReactiveCommand]
    private void DuplicateColumn(QueryColumnVM vm) {
        if (!GetColumnIndex(vm, out var columnIndex)) return;

        var insertedColumn = InsertColumn(columnIndex + 2);
        if (insertedColumn.ViewModel is null) return;

        insertedColumn.ViewModel.QueryVM.QueryRunner.RestoreMemento(vm.QueryVM.QueryRunner.CreateMemento());
    }

    [ReactiveCommand]
    private void DeleteColumn(QueryColumnVM vm) {
        if (GetColumnIndex(vm, out var columnIndex)) {
            RemoveColumn(columnIndex);
        }
    }

    [ReactiveCommand]
    private async Task CopyColumnText(QueryColumnVM vm) {
        var clipboard = TopLevel.GetTopLevel(_window)?.Clipboard;
        if (clipboard is not null) {
            var text = string.Join(Environment.NewLine, vm.QueriedFields.Select(x => x.QueriedField));
            await clipboard.SetTextAsync(text);
        }
    }

    [ReactiveCommand]
    private void RestoreColumns() {
        foreach (var queryMemento in StateRepository.LoadAll()) {
            var queryColumn = InsertColumn(ColumnsGrid.ColumnDefinitions.Count - 1);
            queryColumn.ViewModel?.QueryVM.QueryRunner.RestoreMemento(queryMemento);
        }
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
            _menuItemProvider.Save(SaveColumnCommand, queryColumnVM),
            _menuItemProvider.Duplicate(DuplicateColumnCommand, queryColumnVM),
            _menuItemProvider.Delete(DeleteColumnCommand, queryColumnVM),
            _menuItemProvider.Copy(CopyColumnTextCommand, queryColumnVM).Name("Copy Entries Text"),
        ];
    }
}
