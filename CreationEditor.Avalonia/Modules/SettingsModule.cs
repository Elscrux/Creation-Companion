using Autofac;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Settings;
namespace CreationEditor.Avalonia.Modules; 

public sealed class SettingsModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<SettingProvider>()
            .As<ISettingProvider>()
            .SingleInstance();
        
        builder.RegisterType<SettingPathProvider>()
            .As<ISettingPathProvider>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(SettingImporter<>))
            .As(typeof(ISettingImporter<>));

        builder.RegisterType<SettingExporter>()
            .As<ISettingExporter>();

        builder.RegisterType<StartupDocksSettingVM>()
            .SingleInstance();

        builder.RegisterType<SettingsVM>()
            .As<ISettingsVM>();
    }
}
