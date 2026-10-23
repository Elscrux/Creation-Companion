using Autofac;
using CreationEditor.Avalonia.Services.Analyzer;
using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using ReleasePipeline.Services;
using ReleasePipeline.Services.Actions;
using ReleasePipeline.Services.Checks;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline;

public class ReleasePipelineModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<FormattedTopicFormatter>()
            .As<IFormattedTopicFormatter>();

        builder.RegisterType<AnalyzerResultInfoFactory>()
            .As<IAnalyzerResultInfoFactory>();

        builder.RegisterType<ReleasePipelinePlugin>()
            .AsSelf();

        builder.RegisterType<PipelineService>()
            .As<IPipelineService>()
            .SingleInstance();

        builder.RegisterType<PipelineRunner>()
            .AsSelf();

        builder.RegisterType<PipelineActionCatalog>()
            .As<IPipelineActionCatalog>();

        // Register each pipeline action so Autofac can inject the Func<IPipelineAction>[] catalog.
        builder.RegisterType<ClearReleaseFolderAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<CopyFilesAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<CompressArchiveAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<CompressTexturesAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<RemoveEditorMarkersAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<EncodeAudioAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<ExcludePathAction>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<DeletedRecordCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<UnfinishedEditorIDCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<MissingOcclusionCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterAssemblyTypes(typeof(ReleasePipelineModule).Assembly)
            .AssignableTo<IAnalyzer>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<DuplicateEditorIDCheck>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<ReleasePipelineWizardVM>()
            .AsSelf();

        builder.RegisterType<DataSourceStepVM>()
            .AsSelf();

        builder.RegisterType<ChecksStepVM>()
            .AsSelf();

        builder.RegisterType<CheckResultVM>()
            .AsSelf();

        builder.RegisterType<PipelineSelectionStepVM>()
            .AsSelf();

        builder.RegisterType<PipelineCardVM>()
            .AsSelf();

        builder.RegisterType<PipelineEditorVM>()
            .AsSelf();

        builder.RegisterType<PipelineActionVM>()
            .AsSelf();

        builder.RegisterType<PipelineRunVM>()
            .AsSelf();
    }
}
