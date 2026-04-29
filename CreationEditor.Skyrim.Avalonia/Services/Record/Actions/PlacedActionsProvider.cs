using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed partial class PlacedActionsProvider : IContextActionsProvider {
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordEditorController _recordEditorController;
    private readonly IRecordController _recordController;
    private readonly IDockFactory _dockFactory;
    private readonly IList<ContextAction> _actions;

    public PlacedActionsProvider(
        IMenuItemProvider menuItemProvider,
        ILinkCacheProvider linkCacheProvider,
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        IDockFactory dockFactory) {
        _linkCacheProvider = linkCacheProvider;
        _recordEditorController = recordEditorController;
        _recordController = recordController;
        _dockFactory = dockFactory;

        _actions = [
            new ContextAction(context => context is { SelectedRecords: [{ ReferencedRecord.Record: IPlacedObjectGetter }], SelectedAssets.Count: 0 },
                45,
                ContextActionGroup.Modification,
                EditBaseCommand,
                context => menuItemProvider.View(EditBaseCommand, context),
                () => menuItemProvider.View(EditBaseCommand).HotKey),
            new ContextAction(context => context is { SelectedRecords: [{ ReferencedRecord.Record: IPlacedGetter }], SelectedAssets.Count: 0 },
                50,
                ContextActionGroup.Viewing,
                ViewInParentCellCommand,
                context => {
                    var cell = GetParentCell(context);
                    var header = cell is null
                        ? "View in Parent Cell"
                        : "View in Parent Cell: " + cell.GetHumanReadableName();

                    return menuItemProvider.View(ViewInParentCellCommand, context, header);
                },
                () => menuItemProvider.View(ViewInParentCellCommand).HotKey),
        ];
    }

    [ReactiveCommand]
    private void EditBase(SelectedListContext context) {
        if (context.SelectedRecords[0].ReferencedRecord.Record is not IPlacedObjectGetter placedObject) return;

        var placeable = placedObject.Base.TryResolve(_linkCacheProvider.LinkCache);
        if (placeable is null) return;

        var newOverride = _recordController.GetOrAddOverride<IPlaceableObject, IPlaceableObjectGetter>(placeable);
        _recordEditorController.OpenEditor(newOverride);
    }

    [ReactiveCommand]
    private async Task ViewInParentCell(SelectedListContext context) {
        var cell = GetParentCell(context);
        if (cell is null) return;

        var cellLoadStrategy = context.GetSetting<ICellLoadStrategy>();

        try {
            await _dockFactory.Open(DockElement.Viewport);
        } catch (Exception) {
            // ignored
        }

        cellLoadStrategy.LoadCell(cell);
    }

    private ICellGetter? GetParentCell(SelectedListContext context) {
        if (context.SelectedRecords[0].ReferencedRecord.Record is not IPlacedGetter placed) return null;
        if (!_linkCacheProvider.LinkCache.TryResolveSimpleContext<IPlacedGetter>(placed.FormKey, out var placedContext)) return null;
        if (placedContext.Parent?.Record is not ICellGetter cell) return null;

        return cell;
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
