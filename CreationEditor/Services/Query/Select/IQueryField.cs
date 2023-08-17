namespace CreationEditor.Services.Query.Select;

public sealed record QueryFieldMemento(string Name);

public interface IQueryField {
    Type Type { get; }
    string Name { get; }
    object? GetValue(object? obj);
}
