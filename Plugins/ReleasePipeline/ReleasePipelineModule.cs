using Autofac;
using ReleasePipeline.Services;
using ReleasePipeline.Services.Actions;
using ReleasePipeline.Services.Checks;
using ReleasePipeline.Services.ConfigEditor;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline;

public class ReleasePipelineModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<ReleasePipelinePlugin>()
            .AsSelf();

        builder.RegisterType<PipelineService>()
            .As<IPipelineService>()
            .SingleInstance();

        builder.RegisterType<PipelineRunner>()
            .AsSelf();

        builder.RegisterType<PipelineActionCatalog>()
            .As<IPipelineActionCatalog>();

        builder.RegisterType<ConfigEditorService>()
            .As<IConfigEditorService>();

        // Register each pipeline action so Autofac can inject the Func<IPipelineAction>[] catalog.
        builder.RegisterType<ClearReleaseFolderAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<DebugInfoAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<BSAArchiveAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<SuspiciousEditorIdCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<DuplicateEditorIdCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<ReleasePipelineWizardVM>()
            .AsSelf();

        builder.RegisterType<DataSourceStepVM>()
            .AsSelf();

        builder.RegisterType<ChecksStepVM>()
            .AsSelf();

        builder.RegisterType<PipelineSelectionStepVM>()
            .AsSelf();

        builder.RegisterType<ReleasePipelineEditorVM>()
            .AsSelf();

        builder.RegisterType<PipelineRunVM>()
            .AsSelf();
    }
}
