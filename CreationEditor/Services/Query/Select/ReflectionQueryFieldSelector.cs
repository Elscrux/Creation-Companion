using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Select;

public sealed class ReflectionQueryFieldSelector : ReactiveObject, IQueryFieldSelector, IDisposable {
    private readonly DisposableBucket _disposables = new();

    [Reactive] public Type? RecordType { get; set; }
    [Reactive] public IQueryField? SelectedField { get; set; }

    public ReflectionQueryFieldSelector() {
        this.WhenAnyValue(x => x.RecordType)
            .Subscribe(_ => {
                var props = RecordType.GetAllPropertyInfos().ToArray();
                SelectedField = props.FirstOrDefault()?.ToQueryField();
            })
            .DisposeWith(_disposables);
    }

    public QueryFieldSelectorMemento CreateMemento() {
        return new QueryFieldSelectorMemento(
            SelectedField is null ? null : new QueryFieldMemento(SelectedField.Name),
            RecordType?.AssemblyQualifiedName ?? string.Empty);
    }

    public void RestoreMemento(QueryFieldSelectorMemento memento) {
        RecordType = Type.GetType(memento.RecordTypeName);

        if (memento.SelectedField is not null) {
            SelectedField = RecordType
                .GetAllPropertyInfos()
                .FirstOrDefault(field => field.Name == memento.SelectedField.Name)?
                .ToQueryField();
        }
    }

    public void Dispose() => _disposables.Dispose();
}
