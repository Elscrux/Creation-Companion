using System.Reactive;
using Avalonia.Threading;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace BSAssetsTrimmer.ViewModels;

public sealed partial class BSAssetsTrimmerVM : ViewModel {
    public IRecordReferenceController RecordReferenceController { get; }
    public MultiModPickerVM ModPickerVM { get; }
    [Reactive] public partial bool IsBusy { get; set; }

    public ReactiveCommand<Unit, Unit> Run { get; }

    public BSAssetsTrimmerVM(
        ILogger logger,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        Services.BSAssetsTrimmer bsAssetsTrimmer,
        IRecordReferenceController recordReferenceController,
        MultiModPickerVM modPickerVM) {
        RecordReferenceController = recordReferenceController;
        ModPickerVM = modPickerVM;
        ModPickerVM.Filter = mod => BSAssetsTrimmerPlugin.BeyondSkyrimPlugins.Contains(mod.ModKey);

        Run = ReactiveCommand.CreateRunInBackground(() => {
            var bsAssets = editorEnvironment.ResolveMod(BSAssetsTrimmerPlugin.BSAssets);
            if (bsAssets is null) {
                logger.Error("BSAssets.esm not found in load order");
                return;
            }

            Dispatcher.UIThread.Post(() => IsBusy = true);
            var mods = ModPickerVM.Mods.Select(x => x.ModKey).ToList();
            bsAssetsTrimmer.Start(bsAssets, mods);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });
    }
}
