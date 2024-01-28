using Avalonia.Controls;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Query;
using DynamicData.Binding;
using Noggog;
using ReactiveUI.Fody.Helpers;
namespace QueryPlugin.ViewModels;

public sealed class QueryColumnVM : ViewModel {
    [Reactive] public IList<MenuItem>? MenuItems { get; set; }

    public IObservableCollection<object?> QueriedFields { get; } = new ObservableCollectionExtended<object?>();
    public QueryVM QueryVM { get; }

    public QueryColumnVM(
        QueryVM queryVM) {
        QueryVM = queryVM;

        CancellationTokenSource? lastCancellationTokenSource = null;
        QueryVM.QueryRunner.SettingsChanged
            .ThrottleMedium()
            .Subscribe(_ => {
                lastCancellationTokenSource?.Cancel();
                var cancellationTokenSource = lastCancellationTokenSource = new CancellationTokenSource().DisposeWith(this);
                var cancellationToken = cancellationTokenSource.Token;
                Task.Run(() => {
                    QueriedFields.Clear();
                    foreach (var obj in QueryVM.QueryRunner.RunQuery()) {
                        if (cancellationToken.IsCancellationRequested) return;

                        QueriedFields.Add(obj);
                    }
                }, cancellationToken);
            })
            .DisposeWith(this);
    }
}
