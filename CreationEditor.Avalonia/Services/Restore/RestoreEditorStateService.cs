﻿using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.State;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Restore;

public sealed record EditorStateMemento(
    Guid Id,
    IReadOnlyList<ModKey> LoadOrder,
    ModKey ActiveMod);

public class RestoreEditorStateService(
    IEditorEnvironment editorEnvironment,
    IDataSourceService dataSourceService,
    Func<string, IStateRepository<EditorStateMemento>> stateRepositoryFactory) : ILifecycleTask {
    private readonly IStateRepository<EditorStateMemento> _stateRepository = stateRepositoryFactory("EditorState");

    public void PreStartup() {
        var memento = _stateRepository.LoadAll().FirstOrDefault();
        if (memento is null) return;

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
        // No work needed
    }

    public void OnExit() {
        var memento = _stateRepository.LoadAll().FirstOrDefault();

        memento = new EditorStateMemento(
            memento?.Id ?? Guid.NewGuid(),
            editorEnvironment.GameEnvironment.LoadOrder.ListedOrder.Select(x => x.ModKey).ToArray(),
            editorEnvironment.ActiveMod.ModKey);

        _stateRepository.Save(memento, memento.Id);
    }
}
