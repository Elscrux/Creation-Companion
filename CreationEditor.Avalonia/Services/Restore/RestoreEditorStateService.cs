using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.State;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Serilog;
namespace CreationEditor.Avalonia.Services.Restore;

public sealed record EditorStateMemento(
    IReadOnlyList<ModKey> LoadOrder,
    ModKey ActiveMod,
    DockLayoutMemento? DockLayout = null);

public class RestoreEditorStateService(
    IEditorEnvironment editorEnvironment,
    IDataSourceService dataSourceService,
    IDockLayoutPersistence dockLayoutPersistence,
    IStateRepositoryFactory<EditorStateMemento, EditorStateMemento, Guid> stateRepositoryFactory) : ILifecycleTask {
    private readonly IStateRepository<EditorStateMemento, EditorStateMemento, Guid> _stateRepository = stateRepositoryFactory.Create("EditorState");

    private EditorStateMemento? _loadedMemento;

    public void PreStartup() {
        var memento = _stateRepository.LoadAll().FirstOrDefault();
        if (memento is null) return;

        _loadedMemento = memento;

        // Only restore the state if all mods in the load order still exist
        if (memento.LoadOrder.Any(modKey => !dataSourceService.FileExists(new DataRelativePath(modKey.FileName)))) return;

        // Check if the active mod exists, if not, create a new one with the same name
        var activeModExists = dataSourceService.FileExists(new DataRelativePath(memento.ActiveMod.FileName));

        editorEnvironment.Update(updater => {
            if (activeModExists) {
                updater.ActiveMod.Existing(memento.ActiveMod);
            } else {
                updater.ActiveMod.New(memento.ActiveMod.Name);
            }

            return updater
                .LoadOrder.SetImmutableMods(memento.LoadOrder)
                .Build();
        });
    }

    public void PostStartupAsync(CancellationToken token) {
        // Restore the docking layout after the UI is initialized
        if (_loadedMemento?.DockLayout is null) return;

        try {
            Task.Run(() => dockLayoutPersistence.Restore(_loadedMemento.DockLayout), token);
        } catch (Exception e) {
            Log.Warning(e, "Failed to restore docking layout");
        }
    }

    public void OnExit() {
        var mementoWithIdentifier = _stateRepository.LoadAllWithIdentifier().FirstOrDefault();

        var id = mementoWithIdentifier.Value is null ? Guid.NewGuid() : mementoWithIdentifier.Key;
        var memento = new EditorStateMemento(
            editorEnvironment.GameEnvironment.LoadOrder.ListedOrder.Select(x => x.ModKey).ToArray(),
            editorEnvironment.ActiveMod.ModKey,
            dockLayoutPersistence.Save());

        _stateRepository.Save(memento, id);
    }
}
