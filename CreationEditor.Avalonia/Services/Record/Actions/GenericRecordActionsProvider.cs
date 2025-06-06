﻿using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed class GenericRecordActionsProvider : IRecordActionsProvider {
    private readonly IList<RecordAction> _actions;

    public GenericRecordActionsProvider(
        Func<object?, IReferenceVM[], ReferenceBrowserVM> referenceBrowserFactory,
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordEditorController recordEditorController,
        IMenuItemProvider menuItemProvider,
        MainWindow mainWindow) {

        var newCommand = ReactiveCommand.Create<RecordListContext>(context => {
            var recordType = AllSelectedRecordsHaveSameType(context)
                ? context.SelectedRecords[0].Record.Registration.GetterType
                : context.ListTypes.First();

            var newRecord = recordController.CreateRecord(recordType);
            recordEditorController.OpenEditor(newRecord);
        });

        var editCommand = ReactiveCommand.Create<RecordListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                var newOverride = recordController.GetOrAddOverride(referencedRecord.Record);
                recordEditorController.OpenEditor(newOverride);
            }
        });

        var duplicateCommand = ReactiveCommand.Create<RecordListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                recordController.DuplicateRecord(referencedRecord.Record);
            }
        });

        var deleteCommand = ReactiveCommand.Create<RecordListContext>(context => {
            foreach (var referencedRecord in context.SelectedRecords) {
                recordController.DeleteRecord(referencedRecord.Record);
            }
        });

        var openReferencesCommand = ReactiveCommand.Create<RecordListContext>(context => {
            var referencedRecord = context.SelectedRecords[0];
            var references = referencedRecord.RecordReferences
                .Select(identifier => new RecordReferenceVM(identifier, linkCacheProvider, recordReferenceController))
                .Cast<IReferenceVM>()
                .ToArray();

            var referenceBrowserVM = referenceBrowserFactory(referencedRecord, references);
            var referenceWindow = new ReferenceWindow(referencedRecord.Record) {
                Content = new ReferenceBrowser(referenceBrowserVM),
            };

            referenceWindow.Show(mainWindow);
        });

        _actions = [
            new RecordAction(HasOneType,
                100,
                RecordActionGroup.Modification,
                newCommand,
                context => menuItemProvider.New(newCommand, context),
                () => menuItemProvider.New(newCommand).HotKey
            ),
            new RecordAction(context => context.SelectedRecords.Any(),
                50,
                RecordActionGroup.Modification,
                editCommand,
                context => menuItemProvider.Edit(
                    editCommand,
                    context,
                    context.SelectedRecords.Count > 1
                        ? $"Edit All ({context.SelectedRecords.Count}"
                        : "Edit"),
                () => menuItemProvider.Edit(editCommand).HotKey,
                true
            ),
            new RecordAction(context => context.SelectedRecords.Any(),
                40,
                RecordActionGroup.Modification,
                duplicateCommand,
                context => menuItemProvider.Duplicate(
                    duplicateCommand,
                    context,
                    context.SelectedRecords.Count > 1
                        ? $"Duplicate All ({context.SelectedRecords.Count})"
                        : "Duplicate"),
                () => menuItemProvider.Duplicate(duplicateCommand).HotKey
            ),
            new RecordAction(context => context.SelectedRecords.Any(),
                30,
                RecordActionGroup.Modification,
                deleteCommand,
                context => menuItemProvider.Delete(
                    deleteCommand,
                    context,
                    context.SelectedRecords.Count > 1
                        ? $"Delete All ({context.SelectedRecords.Count})"
                        : "Delete"),
                () => menuItemProvider.Delete(deleteCommand).HotKey
            ),
            new RecordAction(context => context.SelectedRecords.Count == 1 && context.SelectedRecords[0].RecordReferences.Any(),
                50,
                RecordActionGroup.Inspection,
                openReferencesCommand,
                context => menuItemProvider.References(openReferencesCommand, context),
                () => menuItemProvider.References(openReferencesCommand).HotKey
            ),
        ];
    }

    private static bool HasOneType(RecordListContext context) {
        // Try to infer the record type from the list types
        var oneListType = context.ListTypes.CountIsExactly(1);
        if (oneListType) return true;

        // Try to infer the record type from the selected records
        // Only allow new if all the selected records are the same type
        return AllSelectedRecordsHaveSameType(context);
    }

    private static bool AllSelectedRecordsHaveSameType(RecordListContext context) {
        return context.SelectedRecords
            .Select(x => x.RecordTypeName)
            .Distinct()
            .CountIsExactly(1);
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
