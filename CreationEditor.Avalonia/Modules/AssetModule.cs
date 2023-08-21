using Autofac;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Avalonia.Modules;

public sealed class AssetModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterAssemblyTypes(typeof(AssetQuery).Assembly)
            .InNamespaceOf<AssetQuery>()
            .AssignableTo<AssetQuery>()
            .AsSelf();

        builder.RegisterType<DirectoryAssetQuery>()
            .AsSelf();

        builder.RegisterType<NifDirectoryAssetQuery>()
            .AsSelf();

        builder.RegisterType<AssetSymbolService>()
            .As<IAssetSymbolService>();

        builder.RegisterType<AssetTypeService>()
            .As<IAssetTypeService>();

        builder.RegisterType<AssetController>()
            .As<IAssetController>()
            .SingleInstance();

        builder.RegisterType<AssetReferenceController>()
            .As<IAssetReferenceController>()
            .SingleInstance();

        builder.RegisterType<AssetProvider>()
            .AsSelf()
            .As<IAssetProvider>()
            .SingleInstance();

        builder.RegisterType<NifModificationService>()
            .As<IModelModificationService>();
    }
}
