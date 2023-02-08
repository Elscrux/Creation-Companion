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
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Skyrim.Services.Environment;
using CreationEditor.Skyrim.Services.Mod;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Modules; 

public sealed class SkyrimModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<SkyrimEditorEnvironment>()
            .As<IEditorEnvironment>()
            .As<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>()
            .SingleInstance();

        var environmentProvider = EnvironmentContext.Build(GameRelease.SkyrimSE);
        
        builder.RegisterInstance(new GameReleaseInjection(GameRelease.SkyrimSE)).AsImplementedInterfaces();
        
        builder.RegisterInstance(environmentProvider).As<IEnvironmentContext>();
        
        builder.RegisterType<BSERuntimeService>()
            .As<IViewportRuntimeService>()
            .SingleInstance();

        builder.RegisterType<SkyrimModInfoProvider>()
            .As<IModInfoProvider<IModGetter>>()
            .As<IModInfoProvider<ISkyrimModGetter>>();
        
        builder.RegisterType<SkyrimRecordBrowserVM>().As<IRecordBrowserVM>();
        
        builder.RegisterType<SkyrimModGetterVM>().As<IModGetterVM>();
        
        builder.RegisterType<SkyrimRecordListFactory>().As<IRecordListFactory>();
        
        builder.RegisterType<SkyrimRecordEditorFactory>().As<IRecordEditorFactory>();
        
        builder.RegisterType<SkyrimCellBrowserFactory>().As<ICellBrowserFactory>();

        builder.RegisterGeneric(typeof(RecordProvider<,>))
            .As(typeof(RecordProvider<,>));
        
        builder.RegisterType<RecordController<ISkyrimMod, ISkyrimModGetter>>()
            .As<IRecordController>()
            .SingleInstance();
        
        builder.RegisterAssemblyTypes(typeof(FactionEditorVM).Assembly)
            .InNamespaceOf<FactionEditorVM>()
            .Where(x => x.Name.Contains("EditorVM"))
            .AsImplementedInterfaces();
        
        builder.RegisterType<InteriorCellsProvider>().AsSelf();
        builder.RegisterType<ExteriorCellsProvider>().AsSelf();
        builder.RegisterType<PlacedProvider>().AsSelf();
        
        builder.RegisterType<InteriorCellsVM>().AsSelf();
        builder.RegisterType<ExteriorCellsVM>().AsSelf();
        builder.RegisterType<PlacedListVM>().AsSelf();
        
        builder.RegisterType<CellBrowserVM>().As<ICellBrowserVM>();
    }
}
