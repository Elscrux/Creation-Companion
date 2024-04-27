using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewLocators;

public sealed class ReflectionViewLocator : IDataTemplate {
    public Control Build(object? data) {
        var name = data?.GetType().FullName?
            .Replace("ViewModel", "View")
            .Replace("VM", "View");

        var type = name is null
            ? null
            : Type.GetType(name) ?? Assembly.GetAssembly(typeof(MainVM))?.GetType(name);

        return type is not null
            ? (Control) Activator.CreateInstance(type)!
            : new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is MainVM or ISetting;
}
