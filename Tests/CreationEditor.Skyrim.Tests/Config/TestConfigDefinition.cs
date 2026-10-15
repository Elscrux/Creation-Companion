using CreationEditor.Services.Config;
namespace CreationEditor.Skyrim.Tests.Config;

public sealed class TestConfigDefinition(params IConfigOption[] options) : IConfigDefinition {
    public IReadOnlyList<IConfigOption> Options { get; } = options;
}
