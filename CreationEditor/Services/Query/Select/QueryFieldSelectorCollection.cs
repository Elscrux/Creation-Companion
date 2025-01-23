using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query.Select;

public sealed class QueryFieldSelectorCollection : ReactiveObject, IQueryFieldSelectorCollection, IDisposable {
    private readonly DisposableBucket _disposables = new();
    private readonly ObservableCollectionExtended<IQueryFieldSelector> _selectors = [];
    public IObservableCollection<IQueryFieldSelector> Selectors => _selectors;

    private readonly Subject<Unit> _selectionChanged = new();
    public IObservable<Unit> SelectionChanged => _selectionChanged;
    public ReactiveCommand<Unit, Unit> AddNextSelector { get; }

    public QueryFieldSelectorCollection() {
        var selectionChanges = _selectors.ToObservableChangeSet()
            .AutoRefresh(x => x.SelectedField)
            .Unit()
            .Merge(_selectors.WhenCollectionChanges());

        AddNextSelector = ReactiveCommand.Create(
            canExecute: selectionChanges.Select(_ => {
                if (_selectors.Count == 0) return false;

                var selectedField = _selectors[^1].SelectedField;
                if (selectedField is null) return false;

                return selectedField.Type != typeof(string)
                 && selectedField.Type != typeof(bool)
                 && !selectedField.Type.InheritsFrom(typeof(IFormLinkGetter))
                 && !selectedField.Type.InheritsFromOpenGeneric(typeof(INumberBase<>))
                 && !selectedField.Type.InheritsFrom(typeof(Enum));
            }),
            execute: () => {
                if (_selectors.Count == 0) return;

                AddSelector(_selectors[^1].SelectedField?.Type ?? typeof(object));
            });

        selectionChanges
            .Subscribe(_ => {
                UpdateSelectors();
                _selectionChanged.OnNext(Unit.Default);
            })
            .DisposeWith(_disposables);
    }

    public void AddSelector(Type type) {
        var selector = new ReflectionQueryFieldSelector { RecordType = type };
        _selectors.Add(selector);
    }

    public void SetRootType(Type type) {
        if (_selectors.Count == 0) {
            AddSelector(type);
        } else if (_selectors[0].RecordType != type) {
            _selectors[0].RecordType = type;
            UpdateSelectors();
        }
    }

    public object? GetValue(object? obj) {
        if (obj is null) return null;
        if (_selectors.Count == 0) return null;
        if (_selectors[0].SelectedField is null) return null;

        foreach (var field in _selectors) {
            if (field.SelectedField is null) return obj;

            obj = field.SelectedField.GetValue(obj);
            if (obj is null) return null;
        }

        return obj;
    }

    public string GetFieldName() {
        return string.Join(".", _selectors.Select(x => x.SelectedField?.Name ?? string.Empty));
    }

    private void UpdateSelectors() {
        for (var i = 0; i < _selectors.Count; i++) {
            var field = _selectors[i];
            if (field.SelectedField is null || (i < _selectors.Count - 1
             && field.SelectedField.Type != _selectors[i + 1].RecordType)) {
                // Remove all selectors after are not compatible
                while (i < _selectors.Count - 1) {
                    _selectors.RemoveAt(i + 1);
                }
            }
        }
    }

    public QueryFieldSelectorCollectionMemento CreateMemento() {
        return new QueryFieldSelectorCollectionMemento(
            _selectors.Select(x => x.CreateMemento()).ToArray());
    }

    public void RestoreMemento(QueryFieldSelectorCollectionMemento memento) {
        _selectors.Clear();
        foreach (var selectorMemento in memento.Selectors) {
            var selector = new ReflectionQueryFieldSelector();
            selector.RestoreMemento(selectorMemento);
            _selectors.Add(selector);
        }
    }

    public void Dispose() => _disposables.Dispose();
}
