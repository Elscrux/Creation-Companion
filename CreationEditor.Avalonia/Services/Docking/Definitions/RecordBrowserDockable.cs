using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

public sealed class RecordBrowserDockable(
    Func<IRecordBrowserVM> recordBrowserVMFactory)
    : IDockableDefinition {
    public string Type => DockableTypes.RecordBrowser;
    public string Header => "Record Browser";
    public object Icon => FASymbol.BulletList;
    public string Category => "View";
    public KeyGesture HotKey => new(Key.R, KeyModifiers.Control | KeyModifiers.Alt);
    public double? Size => 500;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Side;
    public Dock Dock => Dock.Left;
    public GridLength GridSize => new(3, GridUnitType.Star);

    public Task<Control?> CreateControl(object? parameter = null) {
        var recordBrowserVM = recordBrowserVMFactory();
        return Task.FromResult<Control?>(new RecordBrowser(recordBrowserVM));
    }
}
