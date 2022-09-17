using Autofac;
using CreationEditor.Environment;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.Skyrim.Services.Record;
using CreationEditor.GUI.Skyrim.ViewModels.Mod;
using CreationEditor.GUI.Skyrim.ViewModels.Record;
using CreationEditor.GUI.ViewModels.Mod;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.Skyrim.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
namespace CreationEditor.GUI.Skyrim.Modules; 

public class SkyrimModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<SkyrimEditorEnvironment>()
            .As<IEditorEnvironment>()
            .As<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>()
            .SingleInstance();

        var environmentProvider = SimpleEnvironmentContext.Build(GameRelease.SkyrimSE);
        builder.RegisterInstance(environmentProvider).As<ISimpleEnvironmentContext>();
        
        builder.RegisterType<SkyrimRecordBrowserVM>().As<IRecordBrowserVM>();
        
        builder.RegisterType<SkyrimModGetterVM>().As<IModGetterVM>();
        
        builder.RegisterType<SkyrimRecordListFactory>().As<IRecordListFactory>();

        builder.RegisterGeneric(typeof(RecordListVM<,>))
            .As(typeof(RecordListVM<,>));

        builder.RegisterAssemblyTypes(typeof(FactionListVM).Assembly)
            .InNamespaceOf<FactionListVM>()
            .Where(x => x.Name.Contains("ListVM"))
            .AsSelf();
        
        builder.RegisterType<RecordController<ISkyrimMod, ISkyrimModGetter>>()
            .As<IRecordController>()
            .SingleInstance();
    }
}
