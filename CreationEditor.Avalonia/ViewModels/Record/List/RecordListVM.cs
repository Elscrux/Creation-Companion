using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Reference;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM : ViewModel, IRecordListVM {
    IRecordProvider IRecordListVM.RecordProvider => RecordProvider;
    public IRecordProvider RecordProvider { get; }

    public IList<DataGridColumn> Columns { get; } = new List<DataGridColumn>();
    public IList<MenuItem> ContextMenuItems { get; } = new List<MenuItem>();

    public IEnumerable? Records { get; }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Unit> OpenReferences { get; }

    public Func<StyledElement, IFormLinkIdentifier> GetFormLink { get; }

    public RecordListVM(
        IMenuItemProvider menuItemProvider,
        IRecordProvider recordProvider,
        ILifetimeScope lifetimeScope,
        MainWindow mainWindow) {
        RecordProvider = recordProvider;

        OpenReferences = ReactiveCommand.Create(() => {
                if (RecordProvider.SelectedRecord is null) return;

                var newScope = lifetimeScope.BeginLifetimeScope();
                var editorEnvironment = newScope.Resolve<IEditorEnvironment>();
                var recordReferenceController = newScope.Resolve<IRecordReferenceController>();

                var references = RecordProvider.SelectedRecord.References
                    .Select(identifier => new RecordReference(identifier, editorEnvironment, recordReferenceController))
                    .Cast<IReference>()
                    .ToArray();

                var identifiersParam = TypedParameter.From(references);
                var contextParam = TypedParameter.From<object?>(RecordProvider.SelectedRecord);
                var referenceBrowserVM = newScope.Resolve<ReferenceBrowserVM>(contextParam, identifiersParam);
                newScope.DisposeWith(referenceBrowserVM);

                var referenceWindow = new ReferenceWindow(RecordProvider.SelectedRecord.Record) {
                    Content = new ReferenceBrowser(referenceBrowserVM)
                };

                referenceWindow.Show(mainWindow);
            },
            this.WhenAnyValue(x => x.RecordProvider.SelectedRecord).Select(selected => selected is { References.Count: > 0 }));

        Records = RecordProvider.RecordCache
            .Connect()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInInProgressMarker(
                x => x.Filter(RecordProvider.Filter, false),
                out var isFiltering)
            .ToObservableCollection(this);

        IsBusy = isFiltering
            .CombineLatest(
                RecordProvider.IsBusy,
                (filtering, busy) => (Filtering: filtering, Busy: busy))
            .ObserveOnGui()
            .Select(list => list.Filtering || list.Busy);

        GetFormLink = element => {
            if (element.DataContext is not IReferencedRecord referencedRecord) return FormLinkInformation.Null;

            return FormLinkInformation.Factory(referencedRecord.Record);
        };

        ContextMenuItems.AddRange(RecordProvider.ContextMenuItems);
        ContextMenuItems.Add(menuItemProvider.References(OpenReferences));
    }

    public override void Dispose() {
        base.Dispose();

        RecordProvider.Dispose();
    }
}
