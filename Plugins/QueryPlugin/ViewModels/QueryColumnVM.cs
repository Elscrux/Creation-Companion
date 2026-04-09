using Avalonia.Controls;
using CreationEditor;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Query;
using DynamicData.Binding;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace QueryPlugin.ViewModels;

public sealed partial class QueryColumnVM : ViewModel {
    [Reactive] public partial IList<MenuItem>? MenuItems { get; set; }

    public IObservableCollection<QueryResult> QueriedFields { get; } = new ObservableCollectionExtended<QueryResult>();
    public QueryVM QueryVM { get; }
    public ILinkCacheProvider LinkCacheProvider { get; }
    public IReferenceService ReferenceService { get; }
    public IContextMenuProvider ContextMenuProvider { get; }

    public QueryColumnVM(
        QueryVM queryVM,
        ILinkCacheProvider linkCacheProvider,
        IReferenceService referenceService,
        IContextMenuProvider contextMenuProvider) {
        QueryVM = queryVM;
        LinkCacheProvider = linkCacheProvider;
        ReferenceService = referenceService;
        ContextMenuProvider = contextMenuProvider;

        CancellationTokenSource? lastCancellationTokenSource = null;
        QueryVM.QueryRunner.SettingsChanged
            .ThrottleMedium()
            .Subscribe(_ => {
                lastCancellationTokenSource?.Cancel();
                var cancellationTokenSource = lastCancellationTokenSource = new CancellationTokenSource().DisposeWith(this);
                var cancellationToken = cancellationTokenSource.Token;
                Task.Run(
                    () => {
                        QueriedFields.Clear();
                        foreach (var obj in QueryVM.QueryRunner.RunQuery()) {
                            if (cancellationToken.IsCancellationRequested) return;

                            QueriedFields.Add(obj);
                        }
                    },
                    cancellationToken);
            })
            .DisposeWith(this);
    }
}
