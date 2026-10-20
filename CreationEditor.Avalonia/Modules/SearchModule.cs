using Autofac;
using CreationEditor.Avalonia.Services.Search;
using CreationEditor.Avalonia.ViewModels.Search;
using CreationEditor.Services.Search;
namespace CreationEditor.Avalonia.Modules;

public sealed class SearchModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Core
        builder.RegisterType<FuzzySearchRanker>()
            .As<ISearchRanker>()
            .SingleInstance();

        builder.RegisterType<SearchService>()
            .As<ISearchService>()
            .SingleInstance();

        // Sources
        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<ISearchItemSource>()
            .As<ISearchItemSource>()
            .SingleInstance();

        builder.RegisterType<AssetSearchSource>()
            .AsSelf()
            .SingleInstance();

        // Recents
        builder.RegisterType<SearchRecentsService>()
            .As<ISearchRecentsService>()
            .SingleInstance();

        // View Model
        builder.RegisterType<SearchVM>()
            .SingleInstance();
    }
}
