using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public abstract class DockContainerVM : ViewModel, IDockObject {
    public abstract IEnumerable<IDockObject> Children { get; }
    public IEnumerable<DockContainerVM> ContainerChildren => Children.OfType<DockContainerVM>();
    public virtual int ChildrenCount => Children.Count();

    public DockContainerVM? DockParent { get; set; }

    [Reactive] public bool InEditMode { get; set; }

    public IEnumerable<IDockObject> IterateAllChildren() {
        foreach (var child in Children) {
            yield return child;

            if (child is not DockContainerVM dockContainerVM) continue;

            foreach (var grandchild in dockContainerVM.IterateAllChildren()) {
                yield return grandchild;
            }
        }
    }

    public IEnumerable<DockContainerVM> IterateAllContainerChildren() {
        foreach (var child in ContainerChildren) {
            yield return child;

            foreach (var grandchild in child.IterateAllContainerChildren()) {
                yield return grandchild;
            }
        }
    }

    public abstract bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock);
    
    public abstract bool Focus(IDockedItem dockedItem);

    public abstract void Add(IDockedItem dockedItem, DockConfig config);
    public abstract bool Remove(IDockedItem dockedItem);
    public abstract bool CleanUp();
}
// public interface IDockContainerVM : IDockObject {
//     public IEnumerable<IDockObject> Children { get; }
//     public int ChildrenCount { get; }
//
//     public new IDockContainerVM? DockParent { get; set; }
//
//     public bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock);
//     
//     public bool Focus(IDockedItem dockedItem);
//
//     public void Add(IDockedItem dockedItem, DockConfig config);
//     public bool Remove(IDockedItem dockedItem);
//     public bool CleanUp();
//
//     public bool InEditMode { get; set; }
// }