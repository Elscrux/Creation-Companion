using Autofac;
using CreationEditor.Services.Plugin;
using WaterFlowPainter.Services;
using WaterFlowPainter.ViewModels;
namespace WaterFlowPainter;

public class WaterFlowPainterModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<WaterFlowPainterPlugin>()
            .AsSelf();

        builder.RegisterType<WaterFlowPainterVM>()
            .AsSelf();

        builder.RegisterType<DdsDecoder>()
            .AsSelf();

        builder.RegisterType<FlowmapService>()
            .AsSelf();

        builder.RegisterType<CellHeightmapService>()
            .AsSelf();

        builder.RegisterType<FlowmapRenderer>()
            .AsSelf();

        builder.RegisterType<FlowmapPainter>()
            .AsSelf();

        builder.RegisterType<FlowParticleSimulator>()
            .AsSelf();
    }
}
