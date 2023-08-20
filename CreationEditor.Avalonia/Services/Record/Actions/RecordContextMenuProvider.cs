using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed class RecordContextMenuProvider<TRecord, TRecordGetter> : IRecordContextMenuProvider
    where TRecord : class, TRecordGetter, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter {
    public static Type? Type => null;

    public IRecordActionsProvider RecordActionsProvider { get; }
    public ICommand PrimaryCommand => RecordActionsProvider.Edit;
    public IEnumerable<object> MenuItems { get; }

    public RecordContextMenuProvider(
        RecordActionsProvider<TRecord, TRecordGetter> recordActionsProvider,
        IMenuItemProvider menuItemProvider,
        IObservable<TRecordGetter?> selectedRecord) {
        RecordActionsProvider = recordActionsProvider;

        var recordBinding = selectedRecord.ToBinding();
        MenuItems = new object[] {
            menuItemProvider.New(recordActionsProvider.New),
            menuItemProvider.Edit(recordActionsProvider.Edit, recordBinding),
            menuItemProvider.Duplicate(recordActionsProvider.Duplicate, recordBinding),
            menuItemProvider.Delete(recordActionsProvider.Delete, recordBinding),
            new Separator(),
            menuItemProvider.References(recordActionsProvider.OpenReferences, recordBinding)
        };
    }
}
