using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewLocators;

public sealed class ReflectionViewLocator : IDataTemplate {
    public Control Build(object? data) {
        var type = GetViewType(data);

        return type is not null
            ? (Control) Activator.CreateInstance(type)!
            : new TextBlock { Text = "Not Found: " + GetViewName(data) };
    }

    public bool Match(object? data) => data is MainVM or ISetting || GetViewType(data) is not null;

    private static Type? GetViewType(object? data) {
        var name = GetViewName(data);
        if (name is null) return null;

        // Search all loaded assemblies so plugin views (e.g. ReleasePipeline) resolve too.
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            var type = SafeGetType(assembly, name);
            if (type is not null) return type;
        }

        return null;
    }

    private static Type? SafeGetType(Assembly assembly, string name) {
        try {
            return assembly.GetType(name);
        } catch (ReflectionTypeLoadException) {
            return null;
        }
    }

    private static string? GetViewName(object? data) {
        var originalName = data?.GetType().FullName;
        var updatedName = originalName?
            .Replace("ViewModel", "View")
            .Replace("VM", "View");

        if (updatedName is null || updatedName == originalName) return null;

        return updatedName;
    }
}
