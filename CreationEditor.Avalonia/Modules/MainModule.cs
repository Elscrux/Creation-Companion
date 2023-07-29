using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Lifecycle;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.Mutagen.References.Record.Query;
using CreationEditor.Services.Mutagen.Type;
using Noggog.Autofac;
namespace CreationEditor.Avalonia.Modules;

public sealed class MainModule : Module {
    protected override void Load(ContainerBuilder builder) {
        // General
        builder.RegisterType<Lifecycle>()
            .As<ILifecycle>();

        builder.RegisterInstance(new FileSystem())
            .As<IFileSystem>()
            .SingleInstance();

        builder.RegisterType<WildcardSearchFilter>()
            .As<ISearchFilter>()
            .SingleInstance();

        builder.RegisterType<BusyService>()
            .As<IBusyService>()
            .SingleInstance();

        builder.RegisterType<ModSaveService>()
            .As<IModSaveService>()
            .SingleInstance();

        builder.RegisterType<AutoSaveService>()
            .As<IAutoSaveService>()
            .SingleInstance();

        builder.RegisterType<NifModificationService>()
            .As<IModelModificationService>();

        builder.RegisterType<AssetSymbolService>()
            .As<IAssetSymbolService>();

        builder.RegisterType<AssetTypeService>()
            .As<IAssetTypeService>();

        // Controller
        builder.RegisterType<RecordReferenceController>()
            .As<IRecordReferenceController>()
            .SingleInstance();

        builder.RegisterType<RecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();

        builder.RegisterType<AssetController>()
            .As<IAssetController>()
            .SingleInstance();

        builder.RegisterType<AssetReferenceController>()
            .As<IAssetReferenceController>()
            .SingleInstance();

        // Provider
        builder.RegisterType<MenuItemProvider>()
            .As<IMenuItemProvider>()
            .SingleInstance();

        builder.RegisterType<MutagenTypeProvider>()
            .As<IMutagenTypeProvider>()
            .SingleInstance();

        builder.RegisterType<RecordTypeProvider>()
            .AsSelf();

        builder.RegisterType<RecordIdentifiersProvider>()
            .AsSelf();

        builder.RegisterType<RecordFilterProvider>()
            .As<IRecordFilterProvider>()
            .SingleInstance();

        builder.RegisterType<ExtraColumnProvider>()
            .As<IExtraColumnProvider>()
            .SingleInstance();

        builder.RegisterType<ModSaveLocationProvider>()
            .As<IModSaveLocationProvider>()
            .SingleInstance();

        builder.RegisterType<DeleteDirectoryProvider>()
            .As<IDeleteDirectoryProvider>()
            .SingleInstance();

        builder.RegisterType<CacheLocationProvider>()
            .As<ICacheLocationProvider>();

        builder.RegisterType<AssetProvider>()
            .AsSelf()
            .As<IAssetProvider>()
            .SingleInstance();

        // Pipeline
        builder.RegisterType<SavePipeline>()
            .As<ISavePipeline>()
            .SingleInstance();

        // Builder
        builder.RegisterType<RecordFilterBuilder>()
            .As<IRecordFilterBuilder>();

        builder.RegisterType<ExtraColumnsBuilder>()
            .As<IExtraColumnsBuilder>();

        // Factory
        builder.RegisterType<DockFactory>()
            .As<IDockFactory>()
            .SingleInstance();

        builder.RegisterType<BSEViewportFactory>()
            .As<IViewportFactory>()
            .SingleInstance();

        // Query
        builder.RegisterType<ReferenceQuery>()
            .As<IReferenceQuery>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(ModAssetQuery).Assembly)
            .InNamespacesOf(typeof(ModAssetQuery))
            .Where(type => type.Name.EndsWith("Query", StringComparison.Ordinal))
            .AsSelf();

        builder.RegisterType<DirectoryAssetQuery>()
            .AsSelf();

        builder.RegisterType<NifDirectoryAssetQuery>()
            .AsSelf();

        // View Model
        builder.RegisterType<MainVM>()
            .SingleInstance();

        builder.RegisterType<ModSelectionVM>();

        builder.RegisterType<RecordBrowserVM>()
            .As<IRecordBrowserVM>();

        builder.RegisterType<RecordListVM>()
            .As<IRecordListVM>();

        builder.RegisterType<ReferenceBrowserVM>()
            .As<ReferenceBrowserVM>();

        builder.RegisterType<ReferenceRemapperVM>()
            .As<ReferenceRemapperVM>();

        builder.RegisterType<RecordBrowserSettingsVM>()
            .As<IRecordBrowserSettingsVM>();

        builder.RegisterType<ModPickerVM>()
            .As<ModPickerVM>();
    }
}
