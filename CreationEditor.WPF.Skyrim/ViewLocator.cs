using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.WPF.ViewModels;
using Dock.Model.Core;
namespace CreationEditor.WPF.Skyrim;

public class ViewLocator : IDataTemplate {
    public IControl Build(object? data) {
        var name = data?.GetType().FullName?
            .Replace("ViewModel", "View")
            .Replace("VM", "View");
        var type = name is null ? null : Type.GetType(name) ?? Assembly.GetAssembly(typeof(MainVM))?.GetType(name);

        return type != null ? (Control) Activator.CreateInstance(type)! : new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) {
        return data is MainVM or IDockable;
    }
}