using Autofac;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Plugin;
using LeveledList.Services;
using LeveledList.Services.Enchantments;
using LeveledList.Services.LeveledList;
using LeveledList.Services.Record;
using LeveledList.Services.Record.List.ExtraColumns;
using LeveledList.ViewModels;
namespace LeveledList;

public class LeveledListModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<LeveledListPlugin>()
            .AsSelf();

        builder.RegisterType<LeveledListGenerator>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<LeveledListImplementer>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<LeveledListVM>()
            .AsSelf();

        builder.RegisterType<ListsVM>()
            .AsSelf();

        builder.RegisterType<TiersVM>()
            .AsSelf();

        builder.RegisterType<EnchantmentsVM>()
            .AsSelf();

        builder.RegisterType<FeatureProvider>()
            .As<IFeatureProvider>()
            .SingleInstance();

        builder.RegisterType<TierController>()
            .As<ITierController>()
            .SingleInstance();

        builder.RegisterType<LeveledListRecordTypeProvider>()
            .As<ILeveledListRecordTypeProvider>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(ArmorFeaturesExtraColumn).Assembly)
            .As<IExtraColumns>()
            .SingleInstance();

        builder.RegisterType<EnchantmentsGenerator>()
            .SingleInstance()
            .AsSelf();

        builder.RegisterType<EnchantmentProvider>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<EnchantmentsImplementer>()
            .AsSelf()
            .SingleInstance();
    }
}
