using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Avalonia.Font;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Lifecycle;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views.Startup;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Plugin;
using CreationEditor.Services.Settings;
using CreationEditor.Services.State;
using FluentAvalonia.UI.Windowing;
using Mutagen.Bethesda.Fonts;
using Mutagen.Bethesda.Fonts.DI;
namespace CreationEditor.Avalonia.Modules;

public sealed class MainModule : Module {
    protected override void Load(ContainerBuilder builder) {
        // General
        builder.RegisterType<LifecycleStartup>()
            .As<ILifecycleStartup>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<ILifecycleTask>()
            .As<ILifecycleTask>();

        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<ISetting>()
            .As<ISetting>();

        builder.RegisterInstance(new FileSystem())
            .As<IFileSystem>()
            .SingleInstance();

        builder.RegisterType<WildcardSearchFilter>()
            .As<ISearchFilter>()
            .SingleInstance();

        builder.RegisterType<RecordBrowserSettings>()
            .As<IRecordBrowserSettings>();

        builder.RegisterType<GetFontConfigListing>()
            .As<IGetFontConfigListing>();

        builder.RegisterType<GetFontConfig>()
            .As<IGetFontConfig>();

        builder.RegisterType<FontProvider>()
            .As<IFontProvider>();

        builder.RegisterType<StartupSplashScreen>()
            .As<IApplicationSplashScreen>();

        // Services
        builder.RegisterType<DataSourceService>()
            .As<IDataSourceService>()
            .SingleInstance();

        builder.RegisterType<SharedDataSourceWatcherProvider>()
            .As<IDataSourceWatcherProvider>()
            .SingleInstance();

        builder.RegisterType<FileSystemDataSourceWatcher>()
            .As<IDataSourceWatcher>();

        builder.RegisterType<BusyService>()
            .As<IBusyService>()
            .SingleInstance();

        builder.RegisterType<ModSaveService>()
            .As<IModSaveService>()
            .SingleInstance();

        builder.RegisterDecorator<AutoSaveService, IModSaveService>();
        builder.RegisterType<AutoSaveService>()
            .As<IAutoSaveService>()
            .SingleInstance();

        // Provider
        builder.RegisterType<PluginsFolderAssemblyProvider>()
            .As<IPluginAssemblyProvider>()
            .SingleInstance();

        builder.RegisterType<MutagenTypeProvider>()
            .As<IMutagenTypeProvider>()
            .SingleInstance();

        builder.RegisterType<ModSaveLocationProvider>()
            .As<IModSaveLocationProvider>()
            .SingleInstance();

        builder.RegisterType<DeleteDirectoryProvider>()
            .As<IDeleteDirectoryProvider>()
            .SingleInstance();

        builder.RegisterType<IgnoredDirectoriesProvider>()
            .As<IIgnoredDirectoriesProvider>()
            .SingleInstance();

        builder.RegisterType<CacheLocationProvider>()
            .As<ICacheLocationProvider>();

        builder.RegisterGeneric(typeof(JsonStateRepository<>))
            .As(typeof(IStateRepository<>));

        builder.RegisterType<ModScopeProviderVM>()
            .AsSelf()
            .As<IModScopeProvider>()
            .As<IModScopeProviderVM>();

        builder.RegisterType<MenuItemProvider>()
            .As<IMenuItemProvider>()
            .SingleInstance();

        builder.RegisterType<ExtraColumnProvider>()
            .As<IExtraColumnProvider>()
            .SingleInstance();

        builder.RegisterType<GameFontLoader>()
            .As<IGameFontLoader>()
            .SingleInstance();

        // Factory
        builder.RegisterType<DockFactory>()
            .As<IDockFactory>()
            .SingleInstance();

        builder.RegisterType<BSEViewportFactory>()
            .As<IViewportFactory>()
            .SingleInstance();

        // Pipeline
        builder.RegisterType<SavePipeline>()
            .As<ISavePipeline>()
            .SingleInstance();

        // Builder
        builder.RegisterType<ExtraColumnsBuilder>()
            .As<IExtraColumnsBuilder>();

        builder.RegisterType<RecordListVMBuilder>()
            .As<IRecordListVMBuilder>();

        // View Model
        builder.RegisterType<MainVM>()
            .SingleInstance();

        builder.RegisterType<ModSelectionVM>()
            .AsSelf();

        builder.RegisterType<RecordBrowserVM>()
            .As<IRecordBrowserVM>();

        builder.RegisterType<RecordListVM>()
            .As<IRecordListVM>();

        builder.RegisterType<ReferenceBrowserVM>()
            .AsSelf();

        builder.RegisterType<ReferenceRemapperVM>()
            .AsSelf();

        builder.RegisterType<SingleModPickerVM>()
            .AsSelf();

        builder.RegisterType<MultiModPickerVM>()
            .AsSelf();

        builder.RegisterType<ModCreationVM>()
            .AsSelf();
    }
}
