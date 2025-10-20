using Autofac;
using CreationEditor.Services.Plugin;
using DialogueExporter.Services.VaSynth;
using DialogueExporter.Services.VoiceSheets;
using DialogueExporter.Services.VoiceSheets.Writer;
using DialogueExporter.ViewModels;
namespace DialogueExporter;

public class DialogueExporterModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<DialogueExporterPlugin>()
            .AsSelf();

        builder.RegisterType<DialogueExporterVM>()
            .AsSelf();

        builder.RegisterType<ExportVaSynth>()
            .AsSelf();

        builder.RegisterType<ExportVoiceSheets>()
            .AsSelf();

        builder.RegisterType<WriteCsv>()
            .AsSelf();

        builder.RegisterType<WriteXlsx>()
            .AsSelf();
    }
}
