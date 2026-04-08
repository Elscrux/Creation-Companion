using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed partial class ReferenceBrowserVM : ViewModel {
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IDataSourceService _dataSourceService;
    private readonly IContextMenuProvider _contextMenuProvider;

    public object? Context { get; }
    public ReferenceRemapperVM? ReferenceRemapperVM { get; }

    [Reactive] public partial bool ShowTree { get; set; }
    [Reactive] public partial ITreeDataGridSource<IReferenceVM> ReferenceSource { get; set; }

    public ReactiveCommand<Unit, Unit> ToggleTree { get; }
    public ReactiveCommand<Unit, Unit> RemapReferences { get; }

    public ReferenceBrowserVM(
        Func<object?, ReferenceRemapperVM> referenceRemapperVMFactory,
        ILinkCacheProvider linkCacheProvider,
        IDataSourceService dataSourceService,
        IContextMenuProvider contextMenuProvider,
        object? context = null,
        params IReadOnlyList<IReferenceVM> references) {
        _linkCacheProvider = linkCacheProvider;
        _dataSourceService = dataSourceService;
        _contextMenuProvider = contextMenuProvider;
        Context = context;

        if (Context is not null) {
            ReferenceRemapperVM = referenceRemapperVMFactory(context);
        }

        var nameColumn = new TemplateColumn<IReferenceVM>(
            "Name",
            new FuncDataTemplate<IReferenceVM>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Name };
            }),
            options: new TemplateColumnOptions<IReferenceVM> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                IsTextSearchEnabled = true,
                TextSearchValueSelector = reference => reference.Name,
                CompareAscending = (a, b) => IReferenceVM.CompareName(b, a),
                CompareDescending = (a, b) => -IReferenceVM.CompareName(b, a),
            }
        );
        var identifierColumn = new TemplateColumn<IReferenceVM>(
            "Identifier",
            new FuncDataTemplate<IReferenceVM>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Identifier };
            }),
            options: new TemplateColumnOptions<IReferenceVM> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                CompareAscending = (a, b) => IReferenceVM.CompareIdentifier(b, a),
                CompareDescending = (a, b) => -IReferenceVM.CompareIdentifier(b, a),
            }
        );
        var typeColumn = new TemplateColumn<IReferenceVM>(
            "Type",
            new FuncDataTemplate<IReferenceVM>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Type };
            }),
            options: new TemplateColumnOptions<IReferenceVM> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                CompareAscending = (a, b) => IReferenceVM.CompareType(b, a),
                CompareDescending = (a, b) => -IReferenceVM.CompareType(b, a),
            }
        );

        var treeReferenceSource = new HierarchicalTreeDataGridSource<IReferenceVM>(references) {
            Columns = {
                new HierarchicalExpanderColumn<IReferenceVM>(
                    nameColumn,
                    reference => reference.Children,
                    reference => reference.HasChildren
                ),
                identifierColumn,
                typeColumn,
            },
        };
        if (treeReferenceSource.Selection is TreeDataGridRowSelectionModel<IReferenceVM> treeRowSelection) treeRowSelection.SingleSelect = false;

        var flatReferenceSource = ReferenceSource = new FlatTreeDataGridSource<IReferenceVM>(references) {
            Columns = {
                nameColumn,
                identifierColumn,
                typeColumn,
            },
        };
        if (flatReferenceSource.Selection is TreeDataGridRowSelectionModel<IReferenceVM> flatRowSelection) flatRowSelection.SingleSelect = false;

        ToggleTree = ReactiveCommand.Create(() => {
            ShowTree = !ShowTree;
            ReferenceSource = ShowTree ? treeReferenceSource : flatReferenceSource;
        });

        RemapReferences = ReactiveCommand.Create(() => ReferenceRemapperVM?.Remap());
    }

    public void ContextMenu(ContextRequestedEventArgs e) {
        if (e.Source is not Control control) return;

        var recordListContext = GetRecordListContext(control);
        if (recordListContext is null) return;

        e.Handled = true;

        var menuItems = _contextMenuProvider.GetMenuItems(recordListContext);

        var contextFlyout = new MenuFlyout();
        foreach (var menuItem in menuItems) {
            contextFlyout.Items.Add(menuItem);
        }

        contextFlyout.ShowAt(control, true);
    }

    public void Primary(TappedEventArgs e) {
        if (e.Source is not Control control) return;

        var recordListContext = GetRecordListContext(control);
        if (recordListContext is null) return;

        e.Handled = true;

        _contextMenuProvider.ExecutePrimary(recordListContext);
    }

    private SelectedListContext? GetRecordListContext(Control control) {
        var treeDataGrid = control.FindAncestorOfType<TreeDataGrid>();
        if (treeDataGrid?.RowSelection is null) return null;

        var selectedAssetReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<AssetReferenceVM>()
            .ToList();

        var selectedRecordReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<RecordReferenceVM>()
            .ToList();

        if (selectedRecordReferences.Count == 0 && selectedAssetReferences.Count == 0) return null;

        var recordReferences = selectedRecordReferences
            .Select(x => {
                var modKey = _linkCacheProvider.LinkCache.GetWinningOverrideModKey(x.ReferencedRecord.Record);
                return new RecordContext(modKey, x.ReferencedRecord);
            })
            .WhereNotNull()
            .ToArray();
        var assetReferences = selectedAssetReferences
            .Select(x => x.ReferencedAsset)
            .Select(x => _dataSourceService.TryGetFileLink(x.AssetLink.DataRelativePath, out var link)
                ? new AssetContext(link, x)
                : null)
            .WhereNotNull()
            .ToArray();
        return new SelectedListContext(recordReferences, assetReferences);
    }
}
