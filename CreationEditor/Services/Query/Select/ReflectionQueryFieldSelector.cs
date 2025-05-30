using System.Reflection;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Services.Query.Select;

public sealed partial class ReflectionQueryFieldSelector : ReactiveObject, IQueryFieldSelector, IDisposable {
    private readonly DisposableBucket _disposables = new();

    [Reactive] public partial Type? RecordType { get; set; }
    [Reactive] public partial IQueryField? SelectedField { get; set; }

    public ReflectionQueryFieldSelector() {
        this.WhenAnyValue(x => x.RecordType)
            .Subscribe(_ => {
                var props = RecordType.GetAllMemberInfos<PropertyInfo>().ToArray();
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
                .GetAllMemberInfos<PropertyInfo>()
                .FirstOrDefault(field => field.Name == memento.SelectedField.Name)?
                .ToQueryField();
        }
    }

    public void Dispose() => _disposables.Dispose();
}
