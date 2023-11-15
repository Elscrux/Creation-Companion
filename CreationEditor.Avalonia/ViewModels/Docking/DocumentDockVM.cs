using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Documents that are organized in a tab view
/// </summary>
public sealed class DocumentDockVM(DockContainerVM dockParent) : TabbedDockVM(dockParent) {
    public override DockMode DockMode => DockMode.Document;

    public static Control CreateControl(IDockedItem dockedItem, DockContainerVM parent) {
        var documentDockVM = new DocumentDockVM(parent);
        documentDockVM.Add(dockedItem, DockConfig.Default);

        return new DocumentDock(documentDockVM);
    }
}
