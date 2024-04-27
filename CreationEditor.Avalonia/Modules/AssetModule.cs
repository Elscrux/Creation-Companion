using Autofac;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Asset;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Avalonia.Modules;

public sealed class AssetModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assetReferenceQueryType = typeof(IAssetReferenceQuery<,>);
        builder.RegisterAssemblyTypes(assetReferenceQueryType.Assembly)
            .Where(x => !x.IsInterface && Array.Exists(x.GetInterfaces(),
                i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == assetReferenceQueryType))
            .AsSelf();

        builder.RegisterAssemblyTypes(typeof(IArchiveAssetParser).Assembly)
            .AssignableTo<IArchiveAssetParser>()
            .AsSelf();

        builder.RegisterAssemblyTypes(typeof(IFileAssetParser).Assembly)
            .AssignableTo<IFileAssetParser>()
            .AsSelf();

        builder.RegisterGeneric(typeof(BinaryAssetReferenceSerialization<,>))
            .As(typeof(IAssetReferenceSerialization<,>));

        builder.RegisterType<HashFileSystemValidation>()
            .As<IFileSystemValidation>();

        builder.RegisterType<BinaryFileSystemValidationSerialization>()
            .As<IHashFileSystemValidationSerialization>();

        builder.RegisterType<AssetReferenceCacheBuilder>()
            .AsSelf();

        builder.RegisterType<AssetReferenceCacheFactory>()
            .As<IAssetReferenceCacheFactory>();

        builder.RegisterType<NifFileAssetParser>()
            .AsSelf();

        builder.RegisterType<AssetSymbolService>()
            .As<IAssetSymbolService>();

        builder.RegisterType<AssetTypeService>()
            .As<IAssetTypeService>();

        builder.RegisterType<ImageLoader>()
            .As<IImageLoader>();

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
