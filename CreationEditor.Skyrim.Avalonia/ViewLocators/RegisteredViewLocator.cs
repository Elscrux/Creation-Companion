using Avalonia.Controls;
using Avalonia.Controls.Templates;
namespace CreationEditor.Skyrim.Avalonia.ViewLocators;

public sealed class RegisteredViewLocator : IDataTemplate {
    private readonly ReflectionViewLocator _reflectionViewLocator = new();

    public Control Build(object? data) => data switch {
        _ => _reflectionViewLocator.Build(data)
    };

    public bool Match(object? data) => _reflectionViewLocator.Match(data);
}
