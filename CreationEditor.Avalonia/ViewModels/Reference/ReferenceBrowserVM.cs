using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed partial class ReferenceBrowserVM : ViewModel {
    private readonly IContextMenuProvider _contextMenuProvider;

    public object? Context { get; }
    public ReferenceRemapperVM? ReferenceRemapperVM { get; }

    [Reactive] public partial bool ShowTree { get; set; }
    [Reactive] public partial ITreeDataGridSource<IReferenceVM> ReferenceSource { get; set; }

    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> EditRecord { get; }
    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> DuplicateRecord { get; }
    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> DeleteRecord { get; }
    public ReactiveCommand<IEnumerable<IReferencedRecord>, Unit> OpenReferences { get; }

    public ReactiveCommand<Unit, Unit> ToggleTree { get; }
    public ReactiveCommand<Unit, Unit> RemapReferences { get; }

    public ReferenceBrowserVM(
        Func<object?, IReadOnlyList<IReferenceVM>, ReferenceBrowserVM> referenceBrowserFactory,
        Func<object?, ReferenceRemapperVM> referenceRemapperVMFactory,
        ILinkCacheProvider linkCacheProvider,
        IContextMenuProvider contextMenuProvider,
        IMenuItemProvider menuItemProvider,
        IRecordController recordController,
        IReferenceService referenceService,
        IRecordEditorController recordEditorController,
        MainWindow mainWindow,
        object? context = null,
        params IReadOnlyList<IReferenceVM> references) {
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

        EditRecord = ReactiveCommand.Create<IEnumerable<IMajorRecordGetter>>(records => {
            foreach (var record in records) {
                var newOverride = recordController.GetOrAddOverride(record);
                recordEditorController.OpenEditor(newOverride);
            }
        });

        DuplicateRecord = ReactiveCommand.Create<IEnumerable<IMajorRecordGetter>>(records => {
            foreach (var record in records) {
                recordController.DuplicateRecord(record);
            }
        });

        DeleteRecord = ReactiveCommand.Create<IEnumerable<IMajorRecordGetter>>(records => {
            foreach (var record in records) {
                recordController.DeleteRecord(record);
            }
        });

        OpenReferences = ReactiveCommand.Create<IEnumerable<IReferencedRecord>>(referencedRecords => {
            var records = referencedRecords.ToArray();

            var recordReferences = records
                .SelectMany(record => record.RecordReferences)
                .Select(identifier => new RecordReferenceVM(identifier, linkCacheProvider, referenceService))
                .Cast<IReferenceVM>()
                .ToArray();

            var referenceBrowserVM = referenceBrowserFactory(referencedRecords, recordReferences);
            var referenceWindow = new ReferenceWindow(records.Select(record => record.Record)) {
                Content = new ReferenceBrowser(referenceBrowserVM),
            };

            referenceWindow.Show(mainWindow);
        });

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

    private static SelectedListContext? GetRecordListContext(Control control) {
        var treeDataGrid = control.FindAncestorOfType<TreeDataGrid>();
        if (treeDataGrid?.RowSelection is null) return null;

        var selectedAssetReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<AssetReferenceVM>()
            .ToList();

        var selectedRecordReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<RecordReferenceVM>()
            .ToList();

        if (selectedRecordReferences.Count == 0 && selectedAssetReferences.Count == 0) return null;

        var recordReferences = selectedRecordReferences.Select(x => x.ReferencedRecord).ToArray();
        var assetReferences = selectedAssetReferences.Select(x => x.ReferencedAsset).ToArray();
        return new SelectedListContext(recordReferences, assetReferences);
    }
}
