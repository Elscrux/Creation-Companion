using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
using DynamicData.Binding;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Root for a docking system. Contains four side docks in every direction and a layout to customize the view.
/// </summary>
public sealed partial class DockingManagerVM : DockContainerVM {
    public object EditLock { get; } = new();
    public DisposableCounterLock CleanUpLock { get; }

    private readonly Subject<IDockedItem> _opened = new();
    public IObservable<IDockedItem> Opened => _opened;

    private readonly Subject<IDockedItem> _closed = new();
    public IObservable<IDockedItem> Closed => _closed;


    public LayoutDockVM Layout { get; }
    [Reactive] public partial Size LayoutSize { get; set; }
    [Reactive] public partial double LayoutHeight { get; set; }
    [Reactive] public partial double LayoutWidth { get; set; }
    [Reactive] public partial double LayoutMinHeight { get; set; }
    [Reactive] public partial double LayoutMinWidth { get; set; }

    public SideDockVM TopSide { get; }
    public SideDockVM BottomSide { get; }
    public SideDockVM LeftSide { get; }
    public SideDockVM RightSide { get; }

    public ObservableCollectionExtended<FloatingDockVM> FloatingDocks { get; } = [];

    public override IEnumerable<IDockObject> Children => _children.Concat(FloatingDocks);

    private readonly List<IDockObject> _children;

    public bool IsReporting { get; set; } = true;

    public DockingManagerVM() {
        CleanUpLock = new DisposableCounterLock(() => CleanUp());

        Layout = new LayoutDockVM(this);
        TopSide = new SideDockVM(this);
        BottomSide = new SideDockVM(this);
        LeftSide = new SideDockVM(this);
        RightSide = new SideDockVM(this);

        _children = [Layout, TopSide, BottomSide, LeftSide, RightSide];
    }

    /// <summary>
    /// Move the given items into a new floating dock window.
    /// </summary>
    /// <param name="items">Items to float</param>
    /// <param name="screenPosition">Position of the floating window in screen coordinates</param>
    /// <param name="size">Initial size of the floating window</param>
    public FloatingDockVM Float(IEnumerable<IDockedItem> items, PixelPoint screenPosition, Size size) {
        var floatingDock = new FloatingDockVM(this);
        FloatingDocks.Add(floatingDock);
        floatingDock.Float(items, screenPosition, size);
        return floatingDock;
    }

    /// <summary>
    /// Move the given item into a new floating dock window.
    /// </summary>
    public FloatingDockVM Float(IDockedItem dockedItem, PixelPoint screenPosition, Size size) => Float([dockedItem], screenPosition, size);

    public void OnDockAdded(IDockedItem dockedItem) {
        if (IsReporting) {
            _opened.OnNext(dockedItem);
        }
    }

    public void OnDockRemoved(IDockedItem dockedItem) {
        if (IsReporting) {
            _closed.OnNext(dockedItem);
            if (dockedItem.Control.DataContext is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }

    public override bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) {
        outDock = ContainerChildren
            .Select(vm => vm.TryGetDock(control, out var layoutDock) ? layoutDock : null)
            .WhereNotNull()
            .FirstOrDefault();

        return outDock is not null;
    }

    public override bool Focus(IDockedItem dockedItem) => ContainerChildren.Any(dockVM => dockVM.Focus(dockedItem));

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockMode) {
            case DockMode.Side:
                AddSideControl(dockedItem, config);
                break;
            case null:
            case DockMode.Floating:
                Float(dockedItem, new PixelPoint(100, 100), new Size(400, 300));
                break;
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
             || RightSide.Remove(dockedItem)
             || FloatingDocks.Any(floatingDock => floatingDock.Remove(dockedItem));
        }
    }

    public override bool CleanUp() {
        // Use snapshots since clean up can remove floating docks from the collection.
        // Repeat until no more changes, since nested containers might need multiple passes.
        var anyChanges = false;
        while (ContainerChildren.Any(child => child.CleanUp())) {
            anyChanges = true;
        }

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

    /// <summary>
    /// Creates a serializable memento of the current layout.
    /// </summary>
    public DockLayoutMemento SaveLayout() {
        return new DockLayoutMemento {
            Top = SaveSide(TopSide),
            Bottom = SaveSide(BottomSide),
            Left = SaveSide(LeftSide),
            Right = SaveSide(RightSide),
            Layout = SaveGrid(Layout.LayoutGrid),
            FloatingDocks = [
                .. FloatingDocks.Select(f => new FloatingDockMemento {
                    X = f.Position.X,
                    Y = f.Position.Y,
                    Width = f.Size.Width,
                    Height = f.Size.Height,
                    Content = SaveTabs(f.Content),
                }),
            ],
        };

        static DockTabGroupMemento? SaveSide(SideDockVM side) => side.Tabs.Count == 0 ? null : SaveTabs(side);

        static DockTabGroupMemento SaveTabs(TabbedDockVM dock) {
            return new DockTabGroupMemento {
                Mode = dock.DockMode,
                Items = [
                    .. dock.Tabs.Select(item => new DockItemMemento {
                        Type = item.DockableType,
                        Header = item.Header,
                        Size = item.Size,
                    }),
                ],
                ActiveIndex = dock.ActiveTab is null ? -1 : dock.Tabs.IndexOf(dock.ActiveTab),
            };
        }

        static DockItemMemento SaveItem(IDockedItem item) => new() {
            Type = item.DockableType,
            Header = item.Header,
            Size = item.Size,
        };

        static DockGridMemento? SaveGrid(Grid grid) {
            if (grid.Children.Count == 0) return null;

            return new DockGridMemento {
                Rows = [.. grid.RowDefinitions.Select(def => def.Height.ToString())],
                Columns = [.. grid.ColumnDefinitions.Select(def => def.Width.ToString())],
                Cells = [
                    .. grid.Children
                        .Where(child => child is not GridSplitter)
                        .Select(child => new DockCellMemento {
                            Row = Grid.GetRow(child),
                            Column = Grid.GetColumn(child),
                            RowSpan = Grid.GetRowSpan(child),
                            ColumnSpan = Grid.GetColumnSpan(child),
                            Item = child is DockedControl dockedControl ? SaveItem(dockedControl.DockedItem) : null,
                            Tabs = child switch {
                                DockGrid { DataContext: DocumentDockVM documentDock } => SaveTabs(documentDock),
                                DocumentDock { DataContext: DocumentDockVM documentDock } => SaveTabs(documentDock),
                                _ => null,
                            },
                            Grid = child switch {
                                DockGrid { DataContext: LayoutDockVM layoutDock } => SaveGrid(layoutDock.LayoutGrid),
                                _ => null,
                            },
                        }),
                ],
            };
        }
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
            _ => throw new ArgumentOutOfRangeException(nameof(config)),
        };
    }
}
