using System.Reactive;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Skyrim;
using QueryPlugin.Views;
using ReactiveUI;
namespace QueryPlugin.ViewModels;

public sealed class QueryPluginVM : ViewModel {
    private readonly PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext;
    /// <summary>
    /// Used to pad the right side of the grid so that the splitter can be dragged at the most right column.
    /// </summary>
    private readonly Control _paddingRight = new StackPanel();

    private readonly IMenuItemProvider _menuItemProvider;

    public Grid ColumnsGrid { get; } = new() {
        ColumnDefinitions = new ColumnDefinitions {
            new()
        }
    };

    public ReactiveCommand<Unit, Unit> AddColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> DuplicateColumn { get; }
    public ReactiveCommand<QueryColumnVM, Unit> DeleteColumn { get; }

    public QueryPluginVM(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;
        _menuItemProvider = _pluginContext.LifetimeScope.Resolve<IMenuItemProvider>();
        ColumnsGrid.Children.Add(_paddingRight);

        AddColumn = ReactiveCommand.Create(() => {
            InsertColumn(ColumnsGrid.ColumnDefinitions.Count - 1);
        });
    }

    private QueryColumn InsertColumn(int index) {
        // Add the new query column
        ColumnsGrid.ColumnDefinitions.Insert(index, new ColumnDefinition(new GridLength(200)));

        var queryColumnVM = new QueryColumnVM(_pluginContext);
        queryColumnVM.MenuItems = GetColumnMenuItems(queryColumnVM);

        var queryColumn = new QueryColumn(queryColumnVM) {
            [Grid.ColumnProperty] = index,
        };
        ColumnsGrid.Children.Add(queryColumn);

        // Add a splitter
        index++;
        ColumnsGrid.ColumnDefinitions.Insert(index, new ColumnDefinition(new GridLength(2.5)));
        ColumnsGrid.Children.Add(new GridSplitter {
            [Grid.ColumnProperty] = index,
            MinWidth = 2.5,
        });

        _paddingRight[Grid.ColumnProperty] = index + 1;

        return queryColumn;
    }

    private IList<MenuItem> GetColumnMenuItems(QueryColumnVM queryColumnVM) {
        return new List<MenuItem> {
            _menuItemProvider.Duplicate(DuplicateColumn, queryColumnVM),
            _menuItemProvider.Delete(DeleteColumn, queryColumnVM),
        };
    }
}
