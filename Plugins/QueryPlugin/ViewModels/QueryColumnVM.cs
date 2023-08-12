using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Plugin;
using DynamicData.Binding;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI.Fody.Helpers;
namespace QueryPlugin.ViewModels;

public sealed class QueryColumnVM : ViewModel {
    [Reactive] public string Name { get; set; } = string.Empty;
    [Reactive] public IList<MenuItem>? MenuItems { get; set; }

    public IObservableCollection<object?> QueriedFields { get; } = new ObservableCollectionExtended<object?>();
    public QueryVM QueryVM { get; }

    public QueryColumnVM(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        QueryVM = pluginContext.LifetimeScope.Resolve<QueryVM>();

        CancellationTokenSource? lastCancellationTokenSource = null;
        QueryVM.QueryRunner.SettingsChanged
            .Subscribe(_ => {
                lastCancellationTokenSource?.Cancel();
                var cancellationTokenSource = lastCancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => {
                    QueriedFields.Clear();
                    foreach (var obj in QueryVM.QueryRunner.RunQuery()) {
                        if (cancellationTokenSource.Token.IsCancellationRequested) return;

                        QueriedFields.Add(obj);
                    }
                }, cancellationTokenSource.Token);
            })
            .DisposeWith(this);
    }
}
