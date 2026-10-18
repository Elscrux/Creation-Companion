using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Scripting;
using CreationEditor.Avalonia.Views.Scripting;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

public sealed class ScriptEditorDockable(
    Func<IScriptVM> scriptVMFactory)
    : IDockableDefinition {
    public string Type => DockableTypes.ScriptEditor;
    public string Header => "Script Editor";
    public object Icon => FASymbol.Code;
    public string Category => "View";
    public KeyGesture? HotKey => null;
    public double? Size => 300;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Document;
    public Dock Dock => Dock.Left;
    public GridLength GridSize => new(1, GridUnitType.Star);

    public Task<Control?> CreateControl(object? parameter = null) {
        var scriptVM = scriptVMFactory();
        return Task.FromResult<Control?>(new ScriptEditor(scriptVM));
    }
}
