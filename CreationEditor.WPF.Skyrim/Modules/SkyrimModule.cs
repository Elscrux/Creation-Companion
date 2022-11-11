using Autofac;
using CreationEditor.Environment;
using CreationEditor.Skyrim.Environment;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.Skyrim.Services.Record;
using CreationEditor.WPF.Skyrim.ViewModels.Mod;
using CreationEditor.WPF.Skyrim.ViewModels.Record;
using CreationEditor.WPF.ViewModels.Mod;
using CreationEditor.WPF.ViewModels.Record;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
namespace CreationEditor.WPF.Skyrim.Modules; 

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
        
        builder.RegisterType<SkyrimRecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();
        
        builder.RegisterAssemblyTypes(typeof(FactionEditorVM).Assembly)
            .InNamespaceOf<FactionEditorVM>()
            .Where(x => x.Name.Contains("EditorVM"))
            .AsImplementedInterfaces();
    }
}
