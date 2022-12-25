using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public interface IDockContainerVM {
    public int ChildrenCount { get; }

    public bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock);
    
    public void Focus(IDockedItem dockedItem);

    public void Add(IDockedItem dockedItem, DockConfig config);
    public bool Remove(IDockedItem dockedItem);
}
