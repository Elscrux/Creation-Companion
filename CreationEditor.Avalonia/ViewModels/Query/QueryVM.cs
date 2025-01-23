using System.ComponentModel;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Where;
using Mutagen.Bethesda.Plugins.Cache;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Query;

public sealed class QueryVM(
    ILinkCacheProvider linkCacheProvider,
    IQueryRunner queryRunner,
    IQueryConditionFactory conditionFactory)
    : ViewModel {

    public string Name {
        get => QueryRunner.Name;
        set {
            (this as IReactiveObject).RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Name)));
            QueryRunner.Name = value;
        }
    }

    public IQueryRunner QueryRunner { get; } = queryRunner;
    public IQueryConditionFactory ConditionFactory { get; } = conditionFactory;
    public IObservable<ILinkCache> LinkCacheChanged { get; } = linkCacheProvider.LinkCacheChanged;
}
