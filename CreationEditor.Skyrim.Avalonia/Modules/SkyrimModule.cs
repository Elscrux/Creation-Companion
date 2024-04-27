using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Scripting;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Skyrim.Avalonia.Services.Asset;
using CreationEditor.Skyrim.Avalonia.Services.Avalonia;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Services.Record.Editor;
using CreationEditor.Skyrim.Avalonia.Services.Record.List;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Mod;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;
using CreationEditor.Skyrim.Services.Mod;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Modules;

public sealed class SkyrimModule : GameSpecificModule<ISkyrimMod, ISkyrimModGetter> {
    protected override GameRelease GameRelease => GameRelease.SkyrimSE;

    protected override IReg<IModInfoProvider<ISkyrimModGetter>> ModInfoProvider => Register<SkyrimModInfoProvider>();
    protected override IReg<IRecordBrowserGroupProvider> RecordBrowserGroupProvider => Register<SkyrimRecordBrowserGroupProvider>();
    protected override IReg<IRecordProviderFactory> RecordProviderFactory => Register<SkyrimRecordProviderFactory>();
    protected override IReg<IRecordEditorFactory> RecordEditorFactory => Register<SkyrimRecordEditorFactory>();
    protected override IReg<IModGetterVM<ISkyrimModGetter>> ModGetterVM => Register<SkyrimModGetterVM>();

    protected override IReg<ICellBrowserFactory> CellBrowserFactory => Register<SkyrimCellBrowserFactory>();
    protected override IReg<IApplicationIconProvider> ApplicationIconProvider => Register<SkyrimApplicationIconProvider>();

    protected override IReg<IArchiveService> ArchiveService => Register<BsaArchiveService>();
    protected override IReg<IAssetBrowserVM> AssetBrowserVM => Register<AssetBrowserVM>();
    protected override IReg<IAssetTypeProvider> AssetTypeProvider => Register<SkyrimAssetTypeProvider>();

    protected override IReg<IScriptVM> ScriptVM => Register<PapyrusScriptVM>();

    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        // Controller
        builder.RegisterType<ConditionCopyPasteController>()
            .As<IConditionCopyPasteController>()
            .SingleInstance();

        // Provider
        builder.RegisterType<CellActionsProvider>()
            .AsSelf();
        builder.RegisterType<InteriorCellsProvider>()
            .AsSelf();
        builder.RegisterType<ExteriorCellsProvider>()
            .AsSelf();
        builder.RegisterType<PlacedProvider>()
            .AsSelf();

        // Service
        builder.RegisterType<BSERuntimeService>()
            .As<IViewportRuntimeService>()
            .SingleInstance();

        // View Model
        builder.RegisterType<InteriorCellsVM>()
            .AsSelf();
        builder.RegisterType<ExteriorCellsVM>()
            .AsSelf();
        builder.RegisterType<PlacedListVM>()
            .AsSelf();
        builder.RegisterType<CellBrowserVM>()
            .As<ICellBrowserVM>();

        builder.RegisterAssemblyTypes(typeof(FactionEditorVM).Assembly)
            .Where(x => x.Name.Contains("EditorVM"))
            .AsImplementedInterfaces();

        // Converter
        builder.RegisterType<HtmlConverter>()
            .AsSelf();
    }
}
