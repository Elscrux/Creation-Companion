﻿using Autofac;
using CreationEditor.Services.Plugin;
using MapperPlugin.ViewModels;
namespace MapperPlugin;

public class MapperModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<MapperPlugin>()
            .AsSelf();

        builder.RegisterType<MapperVM>()
            .AsSelf();
    }
}
