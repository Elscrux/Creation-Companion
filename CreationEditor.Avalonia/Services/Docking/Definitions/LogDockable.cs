using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Logging;
using CreationEditor.Avalonia.Views.Logging;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

public sealed class LogDockable(
    Func<ILogVM> logVMFactory)
    : IDockableDefinition {
    public string Type => DockableTypes.Log;
    public string Header => "Log";
    public object Icon => FASymbol.Message;
    public string Category => "View";
    public KeyGesture HotKey => new(Key.L, KeyModifiers.Control | KeyModifiers.Alt);
    public double? Size => 150;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Side;
    public Dock Dock => Dock.Bottom;
    public GridLength GridSize => new(1, GridUnitType.Star);

    public Task<Control?> CreateControl(object? parameter = null) {
        var logVM = logVMFactory();
        return Task.FromResult<Control?>(new LogView(logVM));
    }
}
