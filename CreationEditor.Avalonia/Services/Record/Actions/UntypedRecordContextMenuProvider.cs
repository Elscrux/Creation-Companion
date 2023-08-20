using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed class UntypedRecordContextMenuProvider : ReactiveObject, IRecordContextMenuProvider {
    public static Type? Type => null;

    public IRecordActionsProvider RecordActionsProvider { get; }
    public ICommand PrimaryCommand => RecordActionsProvider.Edit;
    public IEnumerable<object> MenuItems { get; }

    public UntypedRecordContextMenuProvider(
        UntypedRecordActionsProvider recordActionsProvider,
        IMenuItemProvider menuItemProvider,
        IObservable<IMajorRecordGetter?> selectedRecord) {
        RecordActionsProvider = recordActionsProvider;

        var recordBinding = selectedRecord.ToBinding();
        var recordTypeBinding = selectedRecord
            .Select(record => record?.Registration.GetterType)
            .ToBinding();
        MenuItems = new object[] {
            menuItemProvider.New(recordActionsProvider.New, recordTypeBinding),
            menuItemProvider.Edit(recordActionsProvider.Edit, recordBinding),
            menuItemProvider.Duplicate(recordActionsProvider.Duplicate, recordBinding),
            menuItemProvider.Delete(recordActionsProvider.Delete, recordBinding),
            new Separator(),
            menuItemProvider.References(recordActionsProvider.OpenReferences, recordBinding)
        };
    }
}
