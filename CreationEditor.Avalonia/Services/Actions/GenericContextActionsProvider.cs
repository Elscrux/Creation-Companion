using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed class GenericContextActionsProvider : IContextActionsProvider {
    private readonly IList<ContextAction> _actions;

    public GenericContextActionsProvider(
        Func<object?, IReadOnlyList<IReferenceVM>, ReferenceBrowserVM> referenceBrowserFactory,
        Func<DataRelativePath, AssetReferenceVM> assetReferenceVMFactory,
        Func<IFormLinkIdentifier, RecordReferenceVM> recordReferenceVMFactory,
        IRecordController recordController,
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IRecordEditorController recordEditorController,
        IMenuItemProvider menuItemProvider,
        MainWindow mainWindow) {

        var newCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            var recordType = AllSelectedRecordsHaveSameType(context)
                ? context.SelectedRecords[0].Record.Registration.GetterType
                : context.ListTypes.First();

            var newRecord = recordController.CreateRecord(recordType);
            recordEditorController.OpenEditor(newRecord);
        });

        var editCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                var newOverride = recordController.GetOrAddOverride(referencedRecord.Record);
                recordEditorController.OpenEditor(newOverride);
            }

            foreach (var referencedAsset in context.SelectedAssets) {
                var fileLink = dataSourceService.GetFileLink(referencedAsset.AssetLink.DataRelativePath);
                if (fileLink is null) continue;

                assetController.Open(fileLink);
            }
        });

        var duplicateCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                recordController.DuplicateRecord(referencedRecord.Record);
            }
        });

        var deleteCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                recordController.DeleteRecord(referencedRecord.Record);
            }

            foreach (var referencedAsset in context.SelectedAssets) {
                var fileLink = dataSourceService.GetFileLink(referencedAsset.AssetLink.DataRelativePath);
                if (fileLink is null) continue;

                assetController.Delete(fileLink);
            }
        });

        var openReferencesCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            if (context.SelectedRecords.Count > 0) {
                var referencedRecord = context.SelectedRecords[0];
                var referenceWindow = new ReferenceWindow(referencedRecord.Record) {
                    Content =  new ReferenceBrowser(GetReferenceBrowserVM(referencedRecord))
                };

                referenceWindow.Show(mainWindow);
            } else if (context.SelectedAssets.Count > 0) {
                var referencedAsset = context.SelectedAssets[0];
                var referenceWindow = new ReferenceWindow(referencedAsset.AssetLink.DataRelativePath.Path) {
                    Content =  new ReferenceBrowser(GetReferenceBrowserVM(referencedAsset))
                };

                referenceWindow.Show(mainWindow);
            }

            ReferenceBrowserVM GetReferenceBrowserVM(IReferenced referencedAsset) {
                var references = referencedAsset.RecordReferences.Select(recordReferenceVMFactory)
                    .Concat<IReferenceVM>(referencedAsset.AssetReferences.Select(assetReferenceVMFactory))
                    .ToArray();

                return referenceBrowserFactory(referencedAsset, references);
            }
        });

        _actions = [
            new ContextAction(HasOneRecordType,
                100,
                ContextActionGroup.Modification,
                newCommand,
                context => menuItemProvider.New(newCommand, context),
                () => menuItemProvider.New(newCommand).HotKey
            ),
            new ContextAction(context => context.SelectedAssets.Count > 0 || context.SelectedRecords.Count > 0,
                50,
                ContextActionGroup.Modification,
                editCommand,
                context => menuItemProvider.Edit(
                    editCommand,
                    context,
                    context.Count > 1
                        ? $"Edit All ({context.Count}"
                        : "Edit"),
                () => menuItemProvider.Edit(editCommand).HotKey,
                true
            ),
            new ContextAction(context => context.SelectedRecords.Count > 0 && context.SelectedAssets.Count == 0,
                40,
                ContextActionGroup.Modification,
                duplicateCommand,
                context => menuItemProvider.Duplicate(
                    duplicateCommand,
                    context,
                    context.SelectedRecords.Count > 1
                        ? $"Duplicate All ({context.SelectedRecords.Count})"
                        : "Duplicate"),
                () => menuItemProvider.Duplicate(duplicateCommand).HotKey
            ),
            new ContextAction(context => context.SelectedAssets.Count > 0 && context.SelectedRecords.Count > 0,
                30,
                ContextActionGroup.Modification,
                deleteCommand,
                context => menuItemProvider.Delete(
                    deleteCommand,
                    context,
                    context.Count > 1
                        ? $"Delete All ({context.SelectedRecords.Count})"
                        : "Delete"),
                () => menuItemProvider.Delete(deleteCommand).HotKey
            ),
            new ContextAction(context => (context.SelectedRecords.Count == 1 && context.SelectedRecords[0].GetReferenceCount() > 0)
                 || (context.SelectedAssets.Count == 1 && context.SelectedAssets[0].GetReferenceCount() > 0),
                50,
                ContextActionGroup.Inspection,
                openReferencesCommand,
                context => menuItemProvider.References(openReferencesCommand, context),
                () => menuItemProvider.References(openReferencesCommand).HotKey
            ),
        ];
    }

    private static bool HasOneRecordType(SelectedListContext context) {
        if (context.SelectedAssets.Count > 0) return false;

        // Try to infer the record type from the list types
        var oneListType = context.ListTypes.CountIsExactly(1);
        if (oneListType) return true;

        // Try to infer the record type from the selected records
        // Only allow new if all the selected records are the same type
        return AllSelectedRecordsHaveSameType(context);
    }

    private static bool AllSelectedRecordsHaveSameType(SelectedListContext context) {
        if (context.SelectedAssets.Count > 0) return false;
        
        return context.SelectedRecords
            .Select(x => x.RecordTypeName)
            .Distinct()
            .CountIsExactly(1);
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
