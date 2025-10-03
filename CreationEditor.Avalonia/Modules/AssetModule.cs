using Autofac;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Asset;
namespace CreationEditor.Avalonia.Modules;

public sealed class AssetModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<AssetTypeService>()
            .As<IAssetTypeService>();

        builder.RegisterType<ImageLoader>()
            .As<IImageLoader>();

        builder.RegisterType<AssetController>()
            .As<IAssetController>()
            .SingleInstance();

        builder.RegisterType<NifModificationService>()
            .As<IModelModificationService>();

        builder.RegisterType<NifService>()
            .As<IModelService>();
    }
}
