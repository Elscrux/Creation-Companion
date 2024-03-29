﻿using Autofac;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.Modules;

public sealed class SettingsModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<SettingProvider>()
            .As<ISettingProvider>()
            .SingleInstance();

        builder.RegisterType<SettingPathProvider>()
            .As<ISettingPathProvider>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(JsonSettingImporter<>))
            .As(typeof(ISettingImporter<>));

        builder.RegisterType<JsonSettingExporter>()
            .As<ISettingExporter>();

        builder.RegisterType<SettingsVM>()
            .As<ISettingsVM>();

        builder.RegisterAssemblyTypes(typeof(SettingsVM).Assembly)
            .InNamespaceOf<SettingsVM>()
            .Where(type => type.Name.EndsWith("SettingVM", StringComparison.OrdinalIgnoreCase))
            .AsSelf()
            .SingleInstance();
    }
}
