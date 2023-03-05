using Autofac;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Services.Record.Editor;
using CreationEditor.Skyrim.Avalonia.Services.Record.List;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using CreationEditor.Skyrim.Avalonia.ViewModels.Mod;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Skyrim.Services.Environment;
using CreationEditor.Skyrim.Services.Mod;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog.Autofac;
using Module = Autofac.Module;
namespace CreationEditor.Skyrim.Avalonia.Modules;

public sealed class SkyrimModule : Module {
    protected override void Load(ContainerBuilder builder) {
        // General
        builder.RegisterType<SkyrimEditorEnvironment>()
            .As<IEditorEnvironment>()
            .As<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>()
            .SingleInstance();

        builder.RegisterInstance(new GameReleaseInjection(GameRelease.SkyrimSE))
            .AsImplementedInterfaces();

        // Controller
        builder.RegisterType<RecordController<ISkyrimMod, ISkyrimModGetter>>()
            .As<IRecordController>()
            .SingleInstance();

        // Provider
        var environmentProvider = EnvironmentContext.Build(GameRelease.SkyrimSE);

        builder.RegisterInstance(environmentProvider)
            .As<IEnvironmentContext>();

        builder.RegisterType<SkyrimModInfoProvider>()
            .As<IModInfoProvider<IModGetter>>()
            .As<IModInfoProvider<ISkyrimModGetter>>();

        builder.RegisterType<SkyrimRecordBrowserGroupProvider>()
            .As<IRecordBrowserGroupProvider>();

        builder.RegisterGeneric(typeof(RecordProvider<,>))
            .As(typeof(RecordProvider<,>));

        builder.RegisterType<InteriorCellsProvider>()
            .AsSelf();
        builder.RegisterType<ExteriorCellsProvider>()
            .AsSelf();
        builder.RegisterType<PlacedProvider>()
            .AsSelf();

        // Factory
        builder.RegisterType<SkyrimRecordListFactory>()
            .As<IRecordListFactory>();

        builder.RegisterType<SkyrimRecordEditorFactory>()
            .As<IRecordEditorFactory>();

        builder.RegisterType<SkyrimCellBrowserFactory>()
            .As<ICellBrowserFactory>();

        // Service
        builder.RegisterType<BSERuntimeService>()
            .As<IViewportRuntimeService>()
            .SingleInstance();

        // View Model
        builder.RegisterType<SkyrimModGetterVM>()
            .As<IModGetterVM>();

        builder.RegisterType<InteriorCellsVM>()
            .AsSelf();
        builder.RegisterType<ExteriorCellsVM>()
            .AsSelf();
        builder.RegisterType<PlacedListVM>()
            .AsSelf();

        builder.RegisterType<CellBrowserVM>()
            .As<ICellBrowserVM>();

        builder.RegisterAssemblyTypes(typeof(FactionEditorVM).Assembly)
            .InNamespaceOf<FactionEditorVM>()
            .Where(x => x.Name.Contains("EditorVM"))
            .AsImplementedInterfaces();
    }
}
