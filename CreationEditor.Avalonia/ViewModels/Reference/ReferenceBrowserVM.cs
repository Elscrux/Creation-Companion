using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Models.Reference;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed partial class ReferenceBrowserVM : ViewModel {
    private readonly IMenuItemProvider _menuItemProvider;

    public object? Context { get; }
    public ReferenceRemapperVM? ReferenceRemapperVM { get; }

    [Reactive] public partial bool ShowTree { get; set; }
    [Reactive] public partial ITreeDataGridSource<IReference> ReferenceSource { get; set; }

    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> EditRecord { get; }
    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> DuplicateRecord { get; }
    public ReactiveCommand<IEnumerable<IMajorRecordGetter>, Unit> DeleteRecord { get; }
    public ReactiveCommand<IEnumerable<IReferencedRecord>, Unit> OpenReferences { get; }

    public ReactiveCommand<Unit, Unit> ToggleTree { get; }
    public ReactiveCommand<Unit, Unit> RemapReferences { get; }

    public ReferenceBrowserVM(
        Func<object?, IReference[], ReferenceBrowserVM> referenceBrowserFactory,
        Func<object?, ReferenceRemapperVM> referenceRemapperVMFactory,
        ILinkCacheProvider linkCacheProvider,
        IMenuItemProvider menuItemProvider,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordEditorController recordEditorController,
        MainWindow mainWindow,
        object? context = null,
        params IReference[] references) {
        _menuItemProvider = menuItemProvider;
        Context = context;

        if (Context is not null) {
            ReferenceRemapperVM = referenceRemapperVMFactory(context);
        }

        var nameColumn = new TemplateColumn<IReference>(
            "Name",
            new FuncDataTemplate<IReference>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Name };
            }),
            options: new TemplateColumnOptions<IReference> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                IsTextSearchEnabled = true,
                TextSearchValueSelector = reference => reference.Name,
                CompareAscending = (a, b) => IReference.CompareName(b, a),
                CompareDescending = (a, b) => -IReference.CompareName(b, a),
            }
        );
        var identifierColumn = new TemplateColumn<IReference>(
            "Identifier",
            new FuncDataTemplate<IReference>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Identifier };
            }),
            options: new TemplateColumnOptions<IReference> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                CompareAscending = (a, b) => IReference.CompareIdentifier(b, a),
                CompareDescending = (a, b) => -IReference.CompareIdentifier(b, a),
            }
        );
        var typeColumn = new TemplateColumn<IReference>(
            "Type",
            new FuncDataTemplate<IReference>((r, _) => {
                if (r is null) return null;

                return new TextBlock { Text = r.Type };
            }),
            options: new TemplateColumnOptions<IReference> {
                CanUserResizeColumn = true,
                CanUserSortColumn = true,
                CompareAscending = (a, b) => IReference.CompareType(b, a),
                CompareDescending = (a, b) => -IReference.CompareType(b, a),
            }
        );

        var treeReferenceSource = new HierarchicalTreeDataGridSource<IReference>(references) {
            Columns = {
                new HierarchicalExpanderColumn<IReference>(
                    nameColumn,
                    reference => reference.Children,
                    reference => reference.HasChildren
                ),
                identifierColumn,
                typeColumn,
            },
        };
        if (treeReferenceSource.Selection is TreeDataGridRowSelectionModel<IReference> treeRowSelection) treeRowSelection.SingleSelect = false;

        var flatReferenceSource = ReferenceSource = new FlatTreeDataGridSource<IReference>(references) {
            Columns = {
                nameColumn,
                identifierColumn,
                typeColumn,
            },
        };
        if (flatReferenceSource.Selection is TreeDataGridRowSelectionModel<IReference> flatRowSelection) flatRowSelection.SingleSelect = false;

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
                .Select(identifier => new RecordReference(identifier, linkCacheProvider, recordReferenceController))
                .Cast<IReference>()
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

    public void ContextMenu(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control control) return;

        var treeDataGrid = control.FindAncestorOfType<TreeDataGrid>();
        if (treeDataGrid?.RowSelection is null) return;

        var selectedAssetReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<AssetReference>()
            .ToList();

        var selectedRecordReferences = treeDataGrid.RowSelection.SelectedItems
            .OfType<RecordReference>()
            .ToList();

        if (selectedRecordReferences.Count > 0) {
            var records = selectedRecordReferences.Select(reference => reference.Record);
            var contextFlyout = new MenuFlyout {
                Items = {
                    _menuItemProvider.Edit(EditRecord, records),
                    _menuItemProvider.Duplicate(DuplicateRecord, records),
                    _menuItemProvider.Delete(DeleteRecord, records),
                },
            };

            if (selectedRecordReferences.Exists(reference => reference.ReferencedRecord.RecordReferences.Count > 0)) {
                var references = _menuItemProvider.References(OpenReferences, selectedRecordReferences.Select(record => record.ReferencedRecord));
                contextFlyout.Items.Add(references);
            }

            contextFlyout.ShowAt(control, true);
        }

        if (selectedAssetReferences.Count > 0) {
            // TODO: Implement asset reference context menu
        }

        e.Handled = true;
    }
}
