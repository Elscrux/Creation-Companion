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
using CreationEditor.Services.Mutagen.Type;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed partial class GenericContextActionsProvider : IContextActionsProvider, IGenericContextActionsProvider {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IRecordController _recordController;
    private readonly IAssetController _assetController;
    private readonly IDataSourceService _dataSourceService;
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
        IMutagenTypeProvider mutagenTypeProvider,
        IReferenceBrowserVMFactory referenceBrowserVMFactory,
        MainWindow mainWindow) {
        _editorEnvironment = editorEnvironment;
        _recordController = recordController;
        _assetController = assetController;
        _dataSourceService = dataSourceService;
        _recordEditorController = recordEditorController;
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _mainWindow = mainWindow;

        _actions = [
            new ContextAction(HasOneRecordType,
                100,
                ContextActionGroup.Modification,
                NewCommand,
                context => menuItemProvider.New(NewCommand, context, $"New {mutagenTypeProvider.GetTypeName(context.ListTypes.First())}"),
                () => menuItemProvider.New(NewCommand).HotKey
            ),
            new ContextAction(context => context.SelectedAssets.Count == 0 && context.SelectedRecords.Count > 0,
                50,
                ContextActionGroup.Modification,
                EditCommand,
                context => menuItemProvider.Edit(
                    EditCommand,
                    context,
                    context.Count > 1
                        ? $"Edit All ({context.Count}"
                        : "Edit"),
                () => menuItemProvider.Edit(EditCommand).HotKey,
                true
            ),
            new ContextAction(context => context.SelectedRecords.Count > 0 && context.SelectedAssets.Count == 0,
                40,
                ContextActionGroup.Modification,
                DuplicateCommand,
                context => menuItemProvider.Duplicate(
                    DuplicateCommand,
                    context,
                    context.SelectedRecords.Count > 1
                        ? $"Duplicate All ({context.SelectedRecords.Count})"
                        : "Duplicate"),
                () => menuItemProvider.Duplicate(DuplicateCommand).HotKey
            ),
            new ContextAction(context => context.SelectedAssets.Count > 0 && context.SelectedRecords.Count > 0,
                30,
                ContextActionGroup.Modification,
                DeleteCommand,
                context => menuItemProvider.Delete(
                    DeleteCommand,
                    context,
                    context.Count > 1
                        ? $"Delete All ({context.SelectedRecords.Count})"
                        : "Delete"),
                () => menuItemProvider.Delete(DeleteCommand).HotKey
            ),
            new ContextAction(context => (context.SelectedRecords.Count == 1 && context.SelectedRecords[0].ReferencedRecord.GetReferenceCount() > 0)
                 || (context.SelectedAssets is [{ ReferencedAsset: {} referencedAsset }] && referencedAsset.GetReferenceCount() > 0),
                20,
                ContextActionGroup.Modification,
                RemapReferencesCommand,
                context => menuItemProvider.Custom(RemapReferencesCommand, "Remap References", context, Symbol.Rename)
            ),
            new ContextAction(context => (context.SelectedRecords.Count == 1 && context.SelectedRecords[0].ReferencedRecord.GetReferenceCount() > 0)
                 || (context.SelectedAssets is [{ ReferencedAsset: {} referencedAsset }] && referencedAsset.GetReferenceCount() > 0),
                50,
                ContextActionGroup.Inspection,
                OpenReferencesCommand,
                context => menuItemProvider.References(OpenReferencesCommand, context),
                () => menuItemProvider.References(OpenReferencesCommand).HotKey
            ),
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
                    CopyFormIdCommand,
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

    [ReactiveCommand]
    private void New(SelectedListContext context) {
        var recordType = AllSelectedRecordsHaveSameType(context) ? context.SelectedRecords[0].ReferencedRecord.Record.Registration.GetterType : context.ListTypes.First();

        CreateNewRecord(recordType);
    }

    [ReactiveCommand]
    private void Edit(SelectedListContext context) {
        foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
            EditRecord(record);
        }
    }

    [ReactiveCommand]
    private void Duplicate(SelectedListContext context) {
        foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
            _recordController.DuplicateRecord(record);
        }
    }

    [ReactiveCommand]
    private void Delete(SelectedListContext context) {
        foreach (var record in context.SelectedRecords.Select(x => x.ReferencedRecord.Record)) {
            _recordController.DeleteRecord(record);
        }

        foreach (var referencedAsset in context.SelectedAssets) {
            var fileLink = _dataSourceService.GetFileLink(referencedAsset.DataSourceLink.DataRelativePath);
            if (fileLink is null) continue;

            _assetController.Delete(fileLink);
        }
    }

    [ReactiveCommand]
    private void OpenReferences(SelectedListContext context) {
        if (context.SelectedRecords.Count > 0) {
            var referencedRecord = context.SelectedRecords[0].ReferencedRecord;
            OpenReferences(referencedRecord);
        } else if (context.SelectedAssets.Count > 0) {
            if (context.SelectedAssets[0].ReferencedAsset is not {} referencedAsset) return;

            OpenReferences(referencedAsset);
        }
    }

    [ReactiveCommand]
    private void RemapReferences(SelectedListContext context) {
        if (context.SelectedRecords.Count > 0) {
            var referencedRecord = context.SelectedRecords[0].ReferencedRecord;
            OpenReferences(referencedRecord, true);
        } else if (context.SelectedAssets.Count > 0) {
            if (context.SelectedAssets[0].ReferencedAsset is not {} referencedAsset) return;

            OpenReferences(referencedAsset, true);
        }
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

    public void OpenReferences(IReferencedRecord referencedRecord, bool remap = false) {
        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(referencedRecord);
        var referenceWindow = new ReferenceWindow(referencedRecord.Record, referenceBrowserVM);

        referenceWindow.Show(_mainWindow);
        if (remap) {
            referenceBrowserVM?.ReferenceRemapperVM?.Remap();
        }
    }

    public void OpenReferences(IReferencedAsset referencedAsset, bool remap = false) {
        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(referencedAsset);
        var referenceWindow = new ReferenceWindow(referencedAsset.AssetLink.DataRelativePath.Path, referenceBrowserVM);


        referenceWindow.Show(_mainWindow);
        if (remap) {
            referenceBrowserVM?.ReferenceRemapperVM?.Remap();
        }
    }

    public void OpenReferences(IDataSourceLink dataSourceLink, bool remap = false) {
        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(dataSourceLink);
        var referenceWindow = new ReferenceWindow(
            dataSourceLink.DataRelativePath.Path,
            referenceBrowserVM);

        referenceWindow.Show(_mainWindow);
        if (remap) {
            referenceBrowserVM?.ReferenceRemapperVM?.Remap();
        }

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

        var modIndex = _editorEnvironment.GameEnvironment.LinkCache.ListedOrder.IndexOf(modKey);
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
