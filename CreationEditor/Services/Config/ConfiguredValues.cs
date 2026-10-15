namespace CreationEditor.Services.Config;

/// <summary>
/// Pairs a config definition with the currently set values for its options, e.g. as the item of
/// a list config option. Editors edit the values through the definition's options.
/// </summary>
public interface IConfiguredValues {
    /// <summary>The config definition whose options the values belong to</summary>
    IConfigDefinition ConfigDefinition { get; }

    /// <summary>Currently set values for the definition's options</summary>
    ConfigValues ConfigValues { get; set; }
}

/// <summary>Typed pairing of a config definition with its configured values.</summary>
/// <typeparam name="TDefinition">The config definition the values are configured for</typeparam>
public sealed class ConfiguredValues<TDefinition> : IConfiguredValues
    where TDefinition : IConfigDefinition, new() {

    /// <summary>The config definition whose options the values belong to</summary>
    public TDefinition ConfigDefinition { get; set; } = new();

    /// <summary>Currently set values for the definition's options</summary>
    public ConfigValues ConfigValues { get; set; } = new();

    IConfigDefinition IConfiguredValues.ConfigDefinition => ConfigDefinition;
}
