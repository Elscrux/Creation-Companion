using System.Reactive;
using CreationEditor.Core;
using DynamicData.Binding;
using ReactiveUI;
namespace CreationEditor.Services.Query.Select;

public sealed record QueryFieldSelectorCollectionMemento(IReadOnlyList<QueryFieldSelectorMemento> Selectors);

public interface IQueryFieldSelectorCollection : IReactiveObject, IMementoProvider<QueryFieldSelectorCollectionMemento> {
    IObservableCollection<IQueryFieldSelector> Selectors { get; }
    IObservable<Unit> SelectionChanged { get; }

    ReactiveCommand<Unit, Unit> AddNextSelector { get; }

    void SetRootType(Type type);

    object? GetValue(object? obj);
    string GetFieldName();
}
