using Autofac;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Plugin;
using LeveledList.Services;
using LeveledList.ViewModels;
namespace LeveledList;

public class LeveledListModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<LeveledListPlugin>()
            .AsSelf();

        builder.RegisterType<LeveledListVM>()
            .AsSelf();

        builder.RegisterType<LeveledListGenerator>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<FeatureProvider>()
            .As<IFeatureProvider>()
            .SingleInstance();

        builder.RegisterType<TierController>()
            .As<ITierController>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(ArmorFeaturesExtraColumn).Assembly)
            .As<IAutoAttachingExtraColumns>()
            .SingleInstance();
    }
}
