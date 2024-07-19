using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Select;

public sealed class ReflectionQueryFieldSelector : ReactiveObject, IQueryFieldSelector, IDisposable {
    private readonly DisposableBucket _disposables = new();

    [Reactive] public Type? RecordType { get; set; }

    private static readonly string[] BlacklistedNames = [
        "BinaryWriteTranslator",
        "Registration",
        "StaticRegistration",
        "FormVersion",
    ];

    private static readonly Type[] BlacklistedTypes = [
        typeof(Type),
    ];

    public IObservableCollection<IQueryField> Fields { get; } = new ObservableCollectionExtended<IQueryField>();
    [Reactive] public IQueryField? SelectedField { get; set; }

    public ReflectionQueryFieldSelector() {
        this.WhenAnyValue(x => x.RecordType)
            .Subscribe(_ => {
                Fields.ReplaceWith(GetFields());
                SelectedField = Fields.FirstOrDefault(field => field.Name == "EditorID") ?? Fields.FirstOrDefault();
            })
            .DisposeWith(_disposables);
    }

    private IEnumerable<IQueryField> GetFields() {
        if (RecordType is null) return [];

        if (!RecordType.IsInterface) {
            return GetQueryFields(RecordType);
        }

        // If the record type is an interface, we need to get the fields from all the interfaces it inherits from.
        var dictionary = new Dictionary<string, IQueryField>();
        var typeQueue = new Queue<Type>();
        typeQueue.Enqueue(RecordType);

        while (typeQueue.Count != 0) {
            var type = typeQueue.Dequeue();
            foreach (var queryField in GetQueryFields(type)) {
                dictionary.TryAdd(queryField.Name, queryField);
            }

            foreach (var @interface in type.GetInterfaces()) {
                typeQueue.Enqueue(@interface);
            }
        }

        return dictionary.Values
            .OrderBy(field => field.Name);

        IEnumerable<ReflectionQueryField> GetQueryFields(Type type) {
            return type.GetProperties()
                .Where(p => !BlacklistedNames.Contains(p.Name) && !BlacklistedTypes.Contains(p.PropertyType))
                .Select(field => {
                    var fieldType = field.PropertyType.InheritsFrom(typeof(Nullable<>))
                        ? field.PropertyType.GetGenericArguments()[0]
                        : field.PropertyType;

                    return new ReflectionQueryField(fieldType, field.Name);
                });
        }
    }

    public QueryFieldSelectorMemento CreateMemento() {
        return new QueryFieldSelectorMemento(
            SelectedField is null ? null : new QueryFieldMemento(SelectedField.Name),
            RecordType?.AssemblyQualifiedName ?? string.Empty);
    }

    public void RestoreMemento(QueryFieldSelectorMemento memento) {
        RecordType = Type.GetType(memento.RecordTypeName);

        if (memento.SelectedField is not null) {
            SelectedField = Fields.FirstOrDefault(field => field.Name == memento.SelectedField.Name);
        }
    }

    public void Dispose() => _disposables.Dispose();
}
