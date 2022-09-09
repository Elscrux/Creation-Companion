using Autofac;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using MutagenLibrary.Core.Plugins;
namespace CreationEditor.GUI.Modules; 

public class SkyrimModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var environmentProvider = SimpleEnvironmentContext.Build(GameRelease.SkyrimSE);
        builder.RegisterInstance(environmentProvider).As<ISimpleEnvironmentContext>();
        
        builder.RegisterType<SkyrimRecordBrowserVM>().As<IRecordBrowserVM>();
    }
}
