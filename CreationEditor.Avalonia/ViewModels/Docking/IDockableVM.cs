using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public interface IDockableVM {
    public static abstract Control Create(IDockedItem dockedItem);

    public int ChildrenCount { get; }

    public bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock);
    
    public void Focus(Control control);
    public bool RemoveDockedControl(Control control);
}
