using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Lifecycle;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Settings;
using CreationEditor.Services.State;
using FluentAvalonia.UI.Windowing;
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

        // Services
        builder.RegisterType<DataDirectoryService>()
            .As<IDataDirectoryService>()
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

        // Provider
        builder.RegisterType<MutagenTypeProvider>()
            .As<IMutagenTypeProvider>()
            .SingleInstance();

        builder.RegisterType<ModSaveLocationProvider>()
            .As<IModSaveLocationProvider>()
            .SingleInstance();

        builder.RegisterType<DeleteDirectoryProvider>()
            .As<IDeleteDirectoryProvider>()
            .SingleInstance();

        builder.RegisterType<CacheLocationProvider>()
            .As<ICacheLocationProvider>();

        builder.RegisterType<StatePathProvider>()
            .As<IStatePathProvider>();

        builder.RegisterType<ModScopeProvider>()
            .AsSelf()
            .As<IModScopeProvider>();

        builder.RegisterType<MenuItemProvider>()
            .As<IMenuItemProvider>()
            .SingleInstance();

        builder.RegisterType<ExtraColumnProvider>()
            .As<IExtraColumnProvider>()
            .SingleInstance();

        // Factory
        builder.RegisterType<DockFactory>()
            .As<IDockFactory>()
            .SingleInstance();

        // Pipeline
        builder.RegisterType<SavePipeline>()
            .As<ISavePipeline>()
            .SingleInstance();

        // Builder
        builder.RegisterType<ExtraColumnsBuilder>()
            .As<IExtraColumnsBuilder>();

        // Factory
        builder.RegisterType<BSEViewportFactory>()
            .As<IViewportFactory>()
            .SingleInstance();

        // View Model
        builder.RegisterType<MainVM>()
            .SingleInstance();

        builder.RegisterType<StartupSplashScreen>()
            .As<ISplashScreenVM>()
            .As<IApplicationSplashScreen>();

        builder.RegisterType<ModSelectionVM>();

        builder.RegisterType<RecordBrowserVM>()
            .As<IRecordBrowserVM>();

        builder.RegisterType<RecordListVM>()
            .As<IRecordListVM>();

        builder.RegisterType<ReferenceBrowserVM>()
            .As<ReferenceBrowserVM>();

        builder.RegisterType<ReferenceRemapperVM>()
            .As<ReferenceRemapperVM>();

        builder.RegisterType<ModPickerVM>()
            .As<ModPickerVM>();
    }
}
