using System.Reactive;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace ModCleaner.ViewModels;

public sealed partial class ModCleanerVM : ViewModel {
    public IObservable<bool> IsLoading => RecordReferenceController.IsLoading;//.CombineLatest(AssetReferenceController.IsLoading, (a, b) => a || b);
    public IRecordReferenceController RecordReferenceController { get; }
    // public IAssetReferenceController AssetReferenceController { get; }
    public SingleModPickerVM CleaningModPickerVM { get; }
    public MultiModPickerVM DependenciesModPickerVM { get; }
    [Reactive] public partial bool IsBusy { get; set; }

    public ReactiveCommand<Unit, Unit> Run { get; }

    public ModCleanerVM(
        ILogger logger,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        Services.ModCleaner modCleaner,
        IRecordReferenceController recordReferenceController,
        // IAssetReferenceController assetReferenceController,
        SingleModPickerVM cleaningModPickerVM,
        MultiModPickerVM dependenciesModPickerVM) {
        RecordReferenceController = recordReferenceController;
        // AssetReferenceController = assetReferenceController;
        CleaningModPickerVM = cleaningModPickerVM;
        DependenciesModPickerVM = dependenciesModPickerVM;
        DependenciesModPickerVM.Filter = _ => false;
        CleaningModPickerVM.SelectedModChanged
            .Subscribe(cleanMod => {
                if (cleanMod is null) {
                    DependenciesModPickerVM.Filter = _ => false;
                    return;
                }

                DependenciesModPickerVM.Filter = dependency => editorEnvironment.Environment.ResolveMod(dependency.ModKey)?
                    .ModHeader.MasterReferences.Any(m => cleanMod.ModKey == m.Master) is true;

                // Set all dependencies to selected by default
                foreach (var modItem in DependenciesModPickerVM.Mods) {
                    modItem.IsSelected = true;
                }
            })
            .DisposeWith(this);

        Run = ReactiveCommand.CreateRunInBackground(() => {
            if (CleaningModPickerVM.SelectedMod is null) return;

            var mod = editorEnvironment.ResolveMod(CleaningModPickerVM.SelectedMod.ModKey);
            if (mod is null) {
                logger.Error("{Mod} not found in load order", CleaningModPickerVM.SelectedMod.ModKey);
                return;
            }

            Dispatcher.UIThread.Post(() => IsBusy = true);
            var dependencies = DependenciesModPickerVM.Mods.Select(x => x.ModKey).ToList();
            modCleaner.Start(mod, dependencies);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });
    }
}
