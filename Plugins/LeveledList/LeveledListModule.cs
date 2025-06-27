using Autofac;
using CreationEditor.Services.Plugin;
using LeveledList.ViewModels;
namespace LeveledList;

public class MapperModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<LeveledListPlugin>()
            .AsSelf();

        builder.RegisterType<LeveledListVM>()
            .AsSelf();
    }
}
