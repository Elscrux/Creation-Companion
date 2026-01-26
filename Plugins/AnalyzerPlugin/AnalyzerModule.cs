using AnalyzerPlugin.ViewModels;
using Autofac;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Analyzers.Skyrim;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
namespace AnalyzerPlugin;

public class AnalyzerModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterGeneric(typeof(AnalyzerPlugin<,>))
            .AsSelf();

        builder.RegisterType<AnalyzerVM>()
            .AsSelf();

        builder.RegisterModule<SkyrimAnalyzerModule>();

        builder.Register(context => {
            var gameReleaseContext = context.Resolve<IGameReleaseContext>();
            return GameConstants.Get(gameReleaseContext.Release);
        });
    }
}
