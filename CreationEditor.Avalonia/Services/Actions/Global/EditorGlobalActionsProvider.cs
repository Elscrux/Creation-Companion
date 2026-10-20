using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Avalonia.Views.Setting;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions.Global;

/// <summary>
/// Provides core editor actions, e.g. saving or opening folders and dialogs.
/// </summary>
public sealed class EditorGlobalActionsProvider(
    IEditorEnvironment editorEnvironment,
    IModSaveService modSaveService,
    IFileSystem fileSystem,
    IMainWindowProvider mainWindowProvider,
    Func<ISettingsVM> settingsVMFactory) : IGlobalActionsProvider {

    public IEnumerable<GlobalAction> GetActions() {
        var canSave = editorEnvironment.ActiveModChanged
            .Select(modKey => !modKey.IsNull)
            .StartWith(!editorEnvironment.ActiveMod.ModKey.IsNull);

        return [
            new GlobalAction(
                "Editor.Save",
                "Save",
                ReactiveCommand.Create(Save, canSave),
                "Save all mods in the load order",
                FASymbol.Save,
                100),

            new GlobalAction(
                "Editor.OpenGameFolder",
                "Open Game Folder",
                ReactiveCommand.Create(OpenGameFolder),
                Icon: FASymbol.Folder),

            new GlobalAction(
                "Editor.OpenDataFolder",
                "Open Data Folder",
                ReactiveCommand.Create(OpenDataFolder),
                Icon: FASymbol.Folder),

            new GlobalAction(
                "Editor.OpenSettings",
                "Open Settings",
                ReactiveCommand.Create(OpenSettings),
                Icon: FASymbol.Settings,
                Priority: 75),
        ];
    }

    private void Save() => Parallel.ForEach(editorEnvironment.MutableMods, modSaveService.SaveMod);

    private void OpenGameFolder() {
        var gameFolder = fileSystem.Directory.GetParent(editorEnvironment.GameEnvironment.DataFolderPath);
        if (gameFolder is not null) {
            Process.Start(new ProcessStartInfo {
                FileName = gameFolder.FullName,
                UseShellExecute = true,
                Verb = "open",
            });
        }
    }

    private void OpenDataFolder() {
        Process.Start(new ProcessStartInfo {
            FileName = editorEnvironment.GameEnvironment.DataFolderPath,
            UseShellExecute = true,
            Verb = "open",
        });
    }

    private void OpenSettings() {
        if (mainWindowProvider.Current is not {} mainWindow) return;

        var settingsVM = settingsVMFactory();
        var settingsWindow = new SettingsWindow(settingsVM);
        settingsWindow.ShowDialog(mainWindow);
    }
}
