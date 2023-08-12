using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query;

public interface IRecordFieldSelector {
    IQueryField? SelectedField { get; set; }
    IObservableCollection<IQueryField> Fields { get; }

    Type? RecordType { get; set; }
}

public sealed class RecordFieldSelector : ReactiveObject, IRecordFieldSelector, IDisposable {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    [Reactive] public Type? RecordType { get; set; }

    private static readonly string[] BlacklistedNames = {
        "BinaryWriteTranslator",
        "Registration",
        "StaticRegistration",
        "FormVersion",
    };

    private static readonly Type[] BlacklistedTypes = {
        typeof(Type),
    };

    public IObservableCollection<IQueryField> Fields { get; } = new ObservableCollectionExtended<IQueryField>();
    [Reactive] public IQueryField? SelectedField { get; set; }

    public RecordFieldSelector() {
        this.WhenAnyValue(x => x.RecordType)
            .Subscribe(x => {
                Fields.Clear();
                Fields.AddRange(GetFields());
                SelectedField = Fields.FirstOrDefault(field => field.Name == "EditorID") ?? Fields.FirstOrDefault();
            })
            .DisposeWith(_disposables);
    }

    private IEnumerable<IQueryField> GetFields() {
        if (RecordType is null) return Array.Empty<IQueryField>();

        if (!RecordType.IsInterface) {
            return GetQueryFields(RecordType);
        }

        // If the record type is an interface, we need to get the fields from all the interfaces it inherits from.
        var dictionary = new Dictionary<string, IQueryField>();
        var typeQueue = new Queue<Type>();
        typeQueue.Enqueue(RecordType);

        while (typeQueue.Any()) {
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

        IEnumerable<QueryField> GetQueryFields(Type type) {
            return type.GetProperties()
                .Where(p => !BlacklistedNames.Contains(p.Name) && !BlacklistedTypes.Contains(p.PropertyType))
                .Select(field => {
                    var fieldType = field.PropertyType.InheritsFrom(typeof(Nullable<>))
                        ? field.PropertyType.GetGenericArguments()[0]
                        : field.PropertyType;

                    return new QueryField(fieldType, field.Name);
                });
        }
    }

    public void Dispose() => _disposables.Dispose();
}
