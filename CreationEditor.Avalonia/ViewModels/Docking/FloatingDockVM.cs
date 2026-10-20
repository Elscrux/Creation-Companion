using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// A floating dock: a separate top level window hosting a tabbed dock of documents.
/// Items can be dragged back into the main docking system or into other floating docks.
/// </summary>
public sealed partial class FloatingDockVM : DockContainerVM {
    public DocumentDockVM Content { get; }

    public FloatingDockWindow? Window { get; private set; }

    public override IEnumerable<IDockObject> Children { get; } = [];
    public override int ChildrenCount => Content.ChildrenCount;

    [Reactive] public partial PixelPoint Position { get; set; }
    [Reactive] public partial Size Size { get; set; }
    [Reactive] public partial string Title { get; set; } = string.Empty;

    public DockingManagerVM DockingManager => (DockingManagerVM) DockParent!;

    public FloatingDockVM(DockingManagerVM dockParent) {
        DockParent = dockParent;
        Content = new DocumentDockVM(this);
        Children = [Content];
    }

    /// <summary>
    /// Move the given items into this floating dock and show its window.
    /// </summary>
    /// <param name="items">Items to float</param>
    /// <param name="screenPosition">Position of the window in screen coordinates</param>
    /// <param name="size">Initial size of the window</param>
    public void Float(IEnumerable<IDockedItem> items, PixelPoint screenPosition, Size size) {
        Position = screenPosition;
        Size = size;

        var itemList = items.ToList();
        if (itemList.Count == 0) return;

        // Stop reporting so the items don't get disposed when removed from their old parent.
        // Hold the clean up lock so the (still empty) floating dock isn't cleaned up in between.
        using (DockingManager.CleanUpLock.Lock()) {
            DockingManager
                .StartEdit()
                .StopReporting()
                .Do(() => {
                    foreach (var item in itemList) {
                        item.DockParent?.Remove(item);
                    }

                    // Add in order so the tabs keep their original order
                    foreach (var item in itemList) {
                        Content.Add(item, DockConfig.Default);
                    }
                })
                .StartReporting()
                .FinishEdit();
        }

        // Floating dock might have been cleaned up in the meantime
        if (!DockingManager.FloatingDocks.Contains(this)) return;

        Dispatcher.UIThread.Post(Show);
    }

    private void Show() {
        if (Window is not null) {
            Window.Show();
            return;
        }

        try {
            Window = new FloatingDockWindow(this) {
                WindowStartupLocation = WindowStartupLocation.Manual,
            };
            Window.Show();
        } catch (Exception e) {
            // Window creation is not supported in every environment (e.g. headless unit tests)
            // - the dock state stays intact, only the visual is skipped
            Log.Warning(e, "Failed to create floating dock window");
            Window = null;
        }
    }

    /// <summary>
    /// Close the window and close all contained documents.
    /// </summary>
    [ReactiveCommand]
    private async Task Close() {
        // Close contained documents
        foreach (var tab in Content.Tabs.ToList()) {
            if (tab.CanClose) {
                await tab.CloseCommand.Execute();
            } else {
                Content.Remove(tab);
            }
        }

        // Ensure the window is closed and the container removed
        CleanUp();
    }

    public override bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) => Content.TryGetDock(control, out outDock);

    public override bool Focus(IDockedItem dockedItem) {
        if (Window is null) return false;
        if (!Content.Focus(dockedItem)) return false;

        Window.Activate();
        return true;
    }

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        Content.Add(dockedItem, config);
    }

    public override bool Remove(IDockedItem dockedItem) => Content.Remove(dockedItem);

    public override bool CleanUp() {
        Content.CleanUp();
        if (Content.ChildrenCount != 0) return false;

        // Floating dock is empty - close window and remove from manager
        if (Window is not null) {
            var window = Window;
            Window = null;
            window.Close();
        }

        return DockingManager.FloatingDocks.Remove(this);
    }
}
