using System.Collections;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationEditor.Avalonia.Models.DataSource;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.DataSource;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.DataSource;

public sealed partial class DataSourceSelectionVM : ViewModel, IDataSourceSelectionVM {
    private readonly IDataSourceService _dataSourceService;

    public ObservableCollectionExtended<DataSourceItem> DataSources { get; } = [];
    private readonly HierarchicalTreeDataGridSource<DataSourceItem> _dataSourceTreeSource;
    public ITreeDataGridSource DataSourceTreeSource => _dataSourceTreeSource;
    public Func<object, bool> CanRemoveDataSource { get; }
    public Func<IReactiveSelectable, bool> CanSelect => selectable => selectable is DataSourceItem { DataSource.IsReadOnly: false };

    public IBinding DataSourceIsEnabled => new Binding($"!{nameof(DataSourceItem.DataSource)}.{nameof(DataSourceItem.DataSource.IsReadOnly)}");

    [Reactive] public partial string? AddedDataSourcePath { get; set; }
    public ReactiveCommand<IList, Unit> RemoveDataSource { get; }
    public ReactiveCommand<Unit, Unit> RefreshDataSources { get; }
    public ReactiveCommand<Unit, Unit> ApplyDataSourcesChanges { get; }

    private readonly Subject<bool> _anyLocalChanges = new();
    public IObservable<bool> AnyLocalChanges => _anyLocalChanges;

    public IObservable<bool> CanSave { get; }

    public IObservable<string>? SaveButtonContent => null;

    public DataSourceSelectionVM(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IDataSourceService dataSourceService) {
        _dataSourceService = dataSourceService;

        CanRemoveDataSource = o => o is DataSourceItem { DataSource: { IsReadOnly: false } dataSource }
         && !DataRelativePath.PathComparer.Equals(dataSource.Path, dataDirectoryProvider.Path);

        RefreshDataSources = ReactiveCommand.Create<Unit>(_ => ResetDataSources(_dataSourceService.ListedOrder));

        ApplyDataSourcesChanges = ReactiveCommand.CreateFromTask<Unit>(_ => Save());

        _dataSourceTreeSource = new HierarchicalTreeDataGridSource<DataSourceItem>(DataSources) {
            Columns = {
                new HierarchicalExpanderColumn<DataSourceItem>(
                    new TemplateColumn<DataSourceItem>(null,
                        new FuncDataTemplate<DataSourceItem>((item, _) => {
                            if (item is null) return null;

                            return new CheckBox {
                                [!ToggleButton.IsCheckedProperty] = new Binding(nameof(DataSourceItem.IsSelected), BindingMode.TwoWay),
                                VerticalAlignment = VerticalAlignment.Center,
                                IsEnabled = !item.DataSource.IsReadOnly
                            };
                        })),
                    dataSourceItem => dataSourceItem.Children
                ),
                new TemplateColumn<DataSourceItem>(null,
                    new FuncDataTemplate<DataSourceItem>((item, _) => {
                        if (item is null) return null;

                        return new FontIcon {
                            Glyph = item.DataSource.IsReadOnly ? "\uE72E" : "\uE785",
                            Foreground = item.DataSource.IsReadOnly ? Brushes.Orange : Brushes.ForestGreen,
                        };
                    })),
                new TemplateColumn<DataSourceItem>("Name",
                    new FuncDataTemplate<DataSourceItem>((item, _) => {
                        if (item is null) return null;

                        return new TextBlock {
                            Text = item.DataSource.Name,
                            [ToolTip.TipProperty] = item.DataSource.Path,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                    })),
                new TemplateColumn<DataSourceItem>("Is Active",
                    new FuncDataTemplate<DataSourceItem>((item, _) => {
                        if (item is null) return null;

                        return new CheckBox {
                            [!ToggleButton.IsCheckedProperty] = new Binding(nameof(DataSourceItem.IsActive), BindingMode.TwoWay),
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                    })),
            }
        };

        _dataSourceTreeSource.RowSelection!.SingleSelect = false;

        _dataSourceService.DataSourcesChanged
            .Subscribe(ResetDataSources)
            .DisposeWith(this);

        DataSources
            .ObserveCollectionChanges()
            .Subscribe(_ => _anyLocalChanges.OnNext(true))
            .DisposeWith(this);

        DataSources
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Subscribe(updatedItems => {
                // Forward selection state to children
                foreach (var item in updatedItems) {
                    foreach (var child in item.Children) {
                        child.IsSelected = item.IsSelected;
                    }

                    // Deactivate item if it is not selected anymore
                    if (item is { IsActive: true, IsSelected: false }) {
                        item.IsActive = false;
                    }
                }

                _anyLocalChanges.OnNext(true);
            })
            .DisposeWith(this);

        var dataSourceActivated = DataSources
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsActive);

        dataSourceActivated
            .Subscribe(changes => {
                var active = changes.FirstOrDefault(item => item is
                    { Type: ChangeType.Item, Reason: ListChangeReason.Refresh, Item.Current.IsActive: true })?.Item.Current;
                if (active is null) return;

                active.IsSelected = true;

                // Set all other data sources to inactive
                foreach (var item in DataSources) {
                    if (item == active) continue;

                    item.IsActive = false;
                }

                _anyLocalChanges.OnNext(true);
            })
            .DisposeWith(this);

        var anyDataSourceActive = dataSourceActivated
            .Select(x => x.Any(dataSource => dataSource is { Type: ChangeType.Item, Item.Current.IsActive: true }));

        var anyDataSourceSelected = DataSources.WhenCollectionChanges().Select(_ => DataSources.Count > 0);

        CanSave = anyDataSourceSelected.CombineLatest(anyDataSourceActive, (anySelected, anyActive) => anySelected && anyActive);

        RemoveDataSource = ReactiveCommand.Create<IList>(dataSources => {
            var removeDataSources = dataSources
                .Cast<DataSourceItem>()
                .ToList();

            DataSources.RemoveRange(removeDataSources);

            var archiveDataSources = _dataSourceService
                .GetNestedArchiveDataSources(removeDataSources.Select(x => x.DataSource))
                .Order(_dataSourceService.DataSourceComparer);
            foreach (var archiveDataSource in archiveDataSources) {
                DataSources.RemoveWhere(x => x.DataSource.Equals(archiveDataSource));
            }

            _anyLocalChanges.OnNext(true);
        });

        this.WhenAnyValue(x => x.AddedDataSourcePath)
            .NotNull()
            .Subscribe(dataSourcePath => {
                AddedDataSourcePath = null;

                if (DataSources.Any(ds => ds.DataSource.Path.Equals(dataSourcePath, DataRelativePath.PathComparison))) return;

                var dataSource = new FileSystemDataSource(fileSystem, dataSourcePath);
                var archiveDataSources = _dataSourceService.GetNestedArchiveDataSources([dataSource]);
                DataSources.Add(new DataSourceItem(dataSource, archiveDataSources) { IsSelected = true });

                SortDataSources();

                _anyLocalChanges.OnNext(true);
            })
            .DisposeWith(this);
    }

    public Task<bool> Save() {
        var newDataSources = DataSources
            .Where(d => d.IsSelected)
            .Select(d => d.DataSource)
            .ToList();
        
        var activeDataSource = DataSources.FirstOrDefault(d => d.IsActive)?.DataSource;
        if (activeDataSource is null) return Task.FromResult(false);

        _dataSourceService.UpdateDataSources(newDataSources, activeDataSource);

        _anyLocalChanges.OnNext(false);

        return Task.FromResult(true);
    }

    private void ResetDataSources(IReadOnlyList<IDataSource> dataSources) {
        // Update selected state of existing data sources
        foreach (var item in DataSources) {
            item.IsSelected = dataSources.Contains(item.DataSource);
        }

        // Add missing data sources
        foreach (var dataSource in dataSources.OfType<FileSystemDataSource>()) {
            if (DataSources.Any(item => item.DataSource.Equals(dataSource))) continue;

            var archiveDataSources = _dataSourceService
                .GetNestedArchiveDataSources([dataSource])
                .Order(_dataSourceService.DataSourceComparer);

            var newItem = new DataSourceItem(dataSource, archiveDataSources) { IsSelected = true };
            DataSources.Add(newItem);
        }

        // Sort data sources by priority
        SortDataSources();

        // Update active data source
        foreach (var item in DataSources) {
            item.IsActive = Equals(item.DataSource, _dataSourceService.ActiveDataSource);
        }

        _anyLocalChanges.OnNext(false);
    }

    private void SortDataSources() {
        var ordering = DataSources.OrderBy(x => x.DataSource, _dataSourceService.DataSourceComparer);
        DataSources.ApplyOrderNoMove(ordering);
    }
}
