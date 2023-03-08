using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using Noggog;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Root for a docking system. Contains four side docks in every direction and a layout to customize the view.
/// </summary>
public sealed class DockingManagerVM : DockContainerVM {
    public object EditLock { get; } = new();
    public DisposableCounterLock CleanUpLock { get; }


    private readonly Subject<IDockedItem> _opened = new();
    public IObservable<IDockedItem> Opened => _opened;


    private readonly Subject<IDockedItem> _closed = new();
    public IObservable<IDockedItem> Closed => _closed;


    public LayoutDockVM Layout { get; }
    [Reactive] public Size LayoutSize { get; set; }
    [Reactive] public double LayoutHeight { get; set; }
    [Reactive] public double LayoutWidth { get; set; }
    [Reactive] public double LayoutMinHeight { get; set; }
    [Reactive] public double LayoutMinWidth { get; set; }

    public SideDockVM TopSide { get; }
    public SideDockVM BottomSide { get; }
    public SideDockVM LeftSide { get; }
    public SideDockVM RightSide { get; }

    public override IEnumerable<IDockObject> Children { get; }

    public bool IsReporting { get; set; } = true;

    public DockingManagerVM() {
        CleanUpLock = new DisposableCounterLock(() => CleanUp());

        Layout = new LayoutDockVM(this);
        TopSide = new SideDockVM(this);
        BottomSide = new SideDockVM(this);
        LeftSide = new SideDockVM(this);
        RightSide = new SideDockVM(this);

        Children = new List<IDockObject> { Layout, TopSide, BottomSide, LeftSide, RightSide };
    }

    public void OnDockAdded(IDockedItem dockedItem) {
        if (IsReporting) {
            _opened.OnNext(dockedItem);
        }
    }

    public void OnDockRemoved(IDockedItem dockedItem) {
        if (IsReporting) {
            _closed.OnNext(dockedItem);
        }
    }

    public override bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) {
        outDock = ContainerChildren
            .Select(vm => vm.TryGetDock(control, out var layoutDock) ? layoutDock : null)
            .NotNull()
            .FirstOrDefault();

        return outDock != null;
    }

    public override bool Focus(IDockedItem dockedItem) => ContainerChildren.Any(dockVM => dockVM.Focus(dockedItem));

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockMode) {
            case DockMode.Side:
                AddSideControl(dockedItem, config);
                break;
            case null:
            case DockMode.Document:
            case DockMode.Layout:
                Layout.Add(dockedItem, config);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(config));
        }
    }

    private void AddSideControl(IDockedItem dockedItem, DockConfig config) {
        GetSideDock(config).Add(dockedItem, config);
    }

    public override bool Remove(IDockedItem dockedItem) {
        using (dockedItem.RemovalLock.Lock()) {
            return Layout.Remove(dockedItem)
             || TopSide.Remove(dockedItem)
             || BottomSide.Remove(dockedItem)
             || LeftSide.Remove(dockedItem)
             || RightSide.Remove(dockedItem);
        }
    }

    public override bool CleanUp() {
        var anyChanges = ContainerChildren.Any(child => child.CleanUp());

        LayoutSize = Layout.LayoutGrid.AdjustSize();
        UpdateSize();

        return anyChanges;
    }

    public void UpdateSize() {
        LayoutWidth = Math.Clamp(Math.Max(Layout.LayoutGrid.Bounds.Width, LayoutSize.Width), 0, Layout.LayoutGrid.Bounds.Width);
        LayoutMinWidth = Math.Clamp(LayoutSize.Width, 0, Layout.LayoutGrid.Bounds.Width);

        LayoutHeight = Math.Clamp(Math.Max(Layout.LayoutGrid.Bounds.Height, LayoutSize.Height), 0, Layout.LayoutGrid.Bounds.Height);
        LayoutMinHeight = Math.Clamp(LayoutSize.Height, 0, Layout.LayoutGrid.Bounds.Height);
    }

    public void SetEditMode(bool state) {
        foreach (var containerVM in IterateAllContainerChildren()) {
            containerVM.InEditMode = state;
        }
    }

    private SideDockVM GetSideDock(DockConfig config) {
        return config.Dock switch {
            Dock.Top => TopSide,
            Dock.Bottom => BottomSide,
            Dock.Left => LeftSide,
            Dock.Right => RightSide,
            _ => throw new ArgumentOutOfRangeException(nameof(config))
        };
    }
}
