using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed partial class GenericContextActionsProvider : IContextActionsProvider, IGenericContextActionsProvider {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IRecordController _recordController;
    private readonly IRecordEditorController _recordEditorController;
    private readonly IReferenceBrowserVMFactory _referenceBrowserVMFactory;
    private readonly MainWindow _mainWindow;
    private readonly IList<ContextAction> _actions;

    public GenericContextActionsProvider(
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IRecordEditorController recordEditorController,
        IMenuItemProvider menuItemProvider,
        IReferenceBrowserVMFactory referenceBrowserVMFactory,
        MainWindow mainWindow) {
        _editorEnvironment = editorEnvironment;
        _recordController = recordController;
        _recordEditorController = recordEditorController;
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _mainWindow = mainWindow;

        var newCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            var recordType = AllSelectedRecordsHaveSameType(context)
                ? context.SelectedRecords[0].ReferencedRecord.Record.Registration.GetterType
                : context.ListTypes.First();

            CreateNewRecord(recordType);
        });

        var editCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
                EditRecord(record);
            }
        });

        var duplicateCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
                _recordController.DuplicateRecord(record);
            }
        });

        var deleteCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
                _recordController.DeleteRecord(record);
            }

            foreach (var referencedAsset in context.SelectedAssets) {
                var fileLink = dataSourceService.GetFileLink(referencedAsset.DataSourceLink.DataRelativePath);
                if (fileLink is null) continue;

                assetController.Delete(fileLink);
            }
        });

        var openReferencesCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            if (context.SelectedRecords.Count > 0) {
                var referencedRecord = context.SelectedRecords[0].ReferencedRecord;
                OpenReferences(referencedRecord);
            } else if (context.SelectedAssets.Count > 0) {
                if (context.SelectedAssets[0].ReferencedAsset is not {} referencedAsset) return;

                OpenReferences(referencedAsset);
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
            new ContextAction(context => context.SelectedAssets.Count == 0 && context.SelectedRecords.Count > 0,
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
            new ContextAction(context => (context.SelectedRecords.Count == 1 && context.SelectedRecords[0].ReferencedRecord.GetReferenceCount() > 0)
                 || (context.SelectedAssets is [{ ReferencedAsset: {} referencedAsset }] && referencedAsset.GetReferenceCount() > 0),
                50,
                ContextActionGroup.Inspection,
                openReferencesCommand,
                context => menuItemProvider.References(openReferencesCommand, context),
                () => menuItemProvider.References(openReferencesCommand).HotKey
            ),
            // TODO remap references context action
            new ContextAction(context => context.SelectedRecords.Count == 1 && context.SelectedAssets.Count == 0,
                53,
                ContextActionGroup.Copy,
                CopyFormKeyCommand,
                context => menuItemProvider.Copy(
                    CopyFormKeyCommand,
                    context,
                    "Copy FormKey " + context.SelectedRecords[0].ReferencedRecord.Record.FormKey,
                    false)
            ),
            new ContextAction(context => context.SelectedRecords.Count == 1 && context.SelectedAssets.Count == 0,
                52,
                ContextActionGroup.Copy,
                CopyFormIdCommand,
                context => menuItemProvider.Copy(
                    CopyFormKeyCommand,
                    context,
                    "Copy FormID " + GetFormId(context),
                    false)
            ),
            new ContextAction(context => context.SelectedRecords.Count == 1 && context.SelectedAssets.Count == 0,
                51,
                ContextActionGroup.Copy,
                CopyEditorIdCommand,
                context => menuItemProvider.Copy(
                    CopyEditorIdCommand,
                    context,
                    "Copy EditorID " + (context.SelectedRecords[0].ReferencedRecord.Record.EditorID is { Length: var length } editorId
                        ? editorId[..Math.Min(length, 20)] + (length > 20 ? "..." : string.Empty)
                        : string.Empty),
                    false)
            ),
        ];
    }

    public IMajorRecord CreateNewRecord(Type recordType) {
        var newRecord = _recordController.CreateRecord(recordType);
        _recordEditorController.OpenEditor(newRecord);
        return newRecord;
    }

    public IMajorRecord EditRecord(IMajorRecordGetter record) {
        var newOverride = _recordController.GetOrAddOverride(record);
        _recordEditorController.OpenEditor(newOverride);
        return newOverride;
    }

    public void OpenReferences(IReferencedRecord referencedRecord) {
        var referenceWindow = new ReferenceWindow(
            referencedRecord.Record,
            _referenceBrowserVMFactory.GetReferenceBrowserVM(referencedRecord));

        referenceWindow.Show(_mainWindow);
    }

    public void OpenReferences(IReferencedAsset referencedAsset) {
        var referenceWindow = new ReferenceWindow(
            referencedAsset.AssetLink.DataRelativePath.Path,
            _referenceBrowserVMFactory.GetReferenceBrowserVM(referencedAsset));

        referenceWindow.Show(_mainWindow);
    }

    public void OpenReferences(IDataSourceLink dataSourceLink) {
        var referenceWindow = new ReferenceWindow(
            dataSourceLink.DataRelativePath.Path,
            _referenceBrowserVMFactory.GetReferenceBrowserVM(dataSourceLink));

        referenceWindow.Show(_mainWindow);
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
            .Select(x => x.ReferencedRecord.RecordTypeName)
            .Distinct()
            .CountIsExactly(1);
    }

    [ReactiveCommand]
    public async Task CopyFormKey(SelectedListContext context) {
        if (context.SelectedRecords is not [{ ReferencedRecord.Record: var record }, ..]) return;

        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;
        if (clipboard is null) return;

        var formKey = record.FormKey.ToString();
        await clipboard.SetTextAsync(formKey);
    }

    [ReactiveCommand]
    public async Task CopyFormId(SelectedListContext context) {
        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;
        if (clipboard is null) return;

        var formId = GetFormId(context);
        if (formId is null) return;

        await clipboard.SetTextAsync(formId);
    }

    private string? GetFormId(SelectedListContext context) {
        if (context.SelectedRecords is not [{ ReferencedRecord.Record: var record, Origin: {} modKey }, ..]) return null;

        var modIndex = _editorEnvironment.GameEnvironment.LoadOrder.IndexOf(modKey);
        var formId = modIndex.ToString("D2") + record.FormKey.IDString();
        return formId;
    }

    [ReactiveCommand]
    public async Task CopyEditorId(SelectedListContext context) {
        if (context.SelectedRecords is not [{ ReferencedRecord.Record.EditorID: var editorId }, ..]) return;

        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;
        if (clipboard is null) return;

        await clipboard.SetTextAsync(editorId);
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
