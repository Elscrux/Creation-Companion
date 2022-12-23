using Autofac;
using CreationEditor.Avalonia.Services.Record;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Environment;
using CreationEditor.Skyrim.Avalonia.Services.Record;
using CreationEditor.Skyrim.Avalonia.Services.Record.List;
using CreationEditor.Skyrim.Avalonia.ViewModels.Mod;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Skyrim.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
namespace CreationEditor.Skyrim.Avalonia.Modules; 

public class SkyrimModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<SkyrimEditorEnvironment>()
            .As<IEditorEnvironment>()
            .As<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>()
            .SingleInstance();

        var environmentProvider = SimpleEnvironmentContext.Build(GameRelease.SkyrimSE);
        
        builder.RegisterInstance(new GameReleaseInjection(GameRelease.SkyrimSE)).AsImplementedInterfaces();
        
        builder.RegisterInstance(environmentProvider).As<ISimpleEnvironmentContext>();
        
        builder.RegisterType<SkyrimRecordBrowserVM>().As<IRecordBrowserVM>();
        
        builder.RegisterType<SkyrimModGetterVM>().As<IModGetterVM>();
        
        builder.RegisterType<SkyrimRecordListFactory>().As<IRecordListFactory>();

        builder.RegisterGeneric(typeof(RecordListVM<,>))
            .As(typeof(RecordListVM<,>));
        
        builder.RegisterType<RecordController<ISkyrimMod, ISkyrimModGetter>>()
            .As<IRecordController>()
            .SingleInstance();
        
        builder.RegisterAssemblyTypes(typeof(FactionEditorVM).Assembly)
            .InNamespaceOf<FactionEditorVM>()
            .Where(x => x.Name.Contains("EditorVM"))
            .AsImplementedInterfaces();
    }
}
