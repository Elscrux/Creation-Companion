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
    }
}
