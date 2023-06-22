using System.Reactive;
using Autofac;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
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
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed class ReferenceBrowserVM : ViewModel {
    private readonly IMenuItemProvider _menuItemProvider;

    public object? Context { get; }
    public ReferenceRemapperVM? ReferenceRemapperVM { get; }

    [Reactive] public bool ShowTree { get; set; }
    [Reactive] public ITreeDataGridSource<IReference> ReferenceSource { get; set; }

    public ReactiveCommand<IMajorRecordGetter, Unit> EditRecord { get; }
    public ReactiveCommand<IMajorRecordGetter, Unit> DuplicateRecord { get; }
    public ReactiveCommand<IMajorRecordGetter, Unit> DeleteRecord { get; }
    public ReactiveCommand<IReferencedRecord, Unit> OpenReferences { get; }

    public ReactiveCommand<Unit, Unit> ToggleTree { get; }
    public ReactiveCommand<Unit, Unit> RemapReferences { get; }

    public ReferenceBrowserVM(
        ILifetimeScope lifetimeScope,
        IMenuItemProvider menuItemProvider,
        IRecordController recordController,
        IRecordEditorController recordEditorController,
        MainWindow mainWindow,
        object? context = null,
        params IReference[] references) {
        _menuItemProvider = menuItemProvider;
        Context = context;

        if (Context is not null) {
            var newScope = lifetimeScope.BeginLifetimeScope();
            var contextParam = TypedParameter.From(context);
            ReferenceRemapperVM = newScope.Resolve<ReferenceRemapperVM>(contextParam);
            newScope.DisposeWith(ReferenceRemapperVM);
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
                typeColumn
            }
        };

        var flatReferenceSource = ReferenceSource = new FlatTreeDataGridSource<IReference>(references) {
            Columns = {
                nameColumn,
                identifierColumn,
                typeColumn
            }
        };

        EditRecord = ReactiveCommand.Create<IMajorRecordGetter>(record => {
            var newOverride = recordController.GetOrAddOverride(record);
            recordEditorController.OpenEditor(newOverride);
        });

        DuplicateRecord = ReactiveCommand.Create<IMajorRecordGetter>(record => {
            recordController.DuplicateRecord(record);
        });

        DeleteRecord = ReactiveCommand.Create<IMajorRecordGetter>(recordController.DeleteRecord);

        OpenReferences = ReactiveCommand.Create<IReferencedRecord>(referencedRecord => {
            var newScope = lifetimeScope.BeginLifetimeScope();
            var editorEnvironment = newScope.Resolve<IEditorEnvironment>();
            var recordReferenceController = newScope.Resolve<IRecordReferenceController>();

            var references = referencedRecord.References
                .Select(identifier => new RecordReference(identifier, editorEnvironment, recordReferenceController))
                .Cast<IReference>()
                .ToArray();

            var identifiersParam = TypedParameter.From(references);
            var contextParam = TypedParameter.From<object?>(referencedRecord);
            var referenceBrowserVM = newScope.Resolve<ReferenceBrowserVM>(contextParam, identifiersParam);
            newScope.DisposeWith(referenceBrowserVM);

            var referenceWindow = new ReferenceWindow(referencedRecord.Record) {
                Content = new ReferenceBrowser(referenceBrowserVM)
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
        if (e.Source is not Control { DataContext: IReference reference } control) return;

        if (reference is RecordReference recordReference) {
            var contextFlyout = new MenuFlyout {
                Items = {
                    _menuItemProvider.Edit(EditRecord, recordReference.Record),
                    _menuItemProvider.Duplicate(DuplicateRecord, recordReference.Record),
                    _menuItemProvider.Delete(DeleteRecord, recordReference.Record),
                }
            };

            if (recordReference.ReferencedRecord.References.Count > 0) {
                var references = _menuItemProvider.References(OpenReferences, recordReference.ReferencedRecord);
                contextFlyout.Items.Add(references);
            }

            contextFlyout.ShowAt(control, true);
        } else if (reference is AssetReference assetReference) {
            
        }

        e.Handled = true;
    }
}
