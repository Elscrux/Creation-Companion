using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Documents that are organized in a tab view
/// </summary>
public sealed class DocumentDockVM : TabbedDockVM {
    public override DockMode DockMode => DockMode.Document;

    public DocumentDockVM(DockContainerVM dockParent) : base(dockParent) {}

    public static Control CreateControl(IDockedItem dockedItem, DockContainerVM parent) {
        var documentDockVM = new DocumentDockVM(parent);
        documentDockVM.Add(dockedItem, DockConfig.Default);

        return new DocumentDock(documentDockVM);
    }
}
