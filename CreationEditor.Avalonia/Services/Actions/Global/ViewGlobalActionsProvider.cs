using CreationEditor.Avalonia.Services.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions.Global;

/// <summary>
/// Provides actions to open the dockable views of the editor.
/// </summary>
public sealed class ViewGlobalActionsProvider(
    IDockableRegistry dockableRegistry,
    IDockFactory dockFactory) : IGlobalActionsProvider {
    public IEnumerable<GlobalAction> GetActions() {
        return dockableRegistry.Definitions
            .Select(definition => new GlobalAction(
                $"View.{definition.Type}",
                $"Open {definition.Header}",
                ReactiveCommand.Create(() => dockFactory.OpenFireAndForget(definition)),
                "View",
                definition.Icon,
                75,
                definition.HotKey));
    }
}
