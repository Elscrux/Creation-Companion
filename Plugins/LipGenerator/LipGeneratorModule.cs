using Autofac;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.Plugin;
using LipGenerator.Services;
using LipGenerator.ViewModels;
namespace LipGenerator;

public sealed class LipGeneratorModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<LipGeneratorPlugin>();
        builder.RegisterType<LipGeneratorVM>();
        builder.RegisterType<SingleDataSourcePickerVM>();

        // Register services
        builder.RegisterType<LipFileGenerator>();
        builder.RegisterType<LipFileGeneratorFactory>();
        
        // Register both lip generator implementations separately (not as ILipGenerator interface)
        builder.RegisterType<FaceFxWrapper>();
        builder.RegisterType<LipGeneratorWrapper>();
        builder.RegisterType<LipFuzerWrapper>();
        builder.RegisterType<XwmEncoderWrapper>();
    }
}
