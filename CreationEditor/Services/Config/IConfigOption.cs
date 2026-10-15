namespace CreationEditor.Services.Config;

public interface IConfigOption {
    string Name { get; }
    string? Description { get; }
    bool IsOptional { get; }
    object DefaultObjectValue { get; }
    Type ValueType { get; }
}

public interface IConfigOption<T> : IConfigOption {
    T DefaultValue { get; }
    Func<T, ConfigOptionValidationError?>? Validate { get; }
}
