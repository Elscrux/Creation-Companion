using Autofac;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Parser;
using CreationEditor.Services.Mutagen.References.Query;
namespace CreationEditor.Avalonia.Modules;

public sealed class ReferenceModule : Module {
    protected override void Load(ContainerBuilder builder) {
        // Validation
        builder.RegisterType<HashFileSystemValidation>()
            .SingleInstance()
            .As<IFileSystemValidation>();

        builder.RegisterType<BinaryFileSystemValidationSerialization>()
            .SingleInstance()
            .As<IHashFileSystemValidationSerialization>();

        // Cache Controllers
        builder.RegisterType<RecordReferenceCacheController>()
            .AsSelf();

        builder.RegisterGeneric(typeof(DictionaryReferenceCacheController<,,>))
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterGeneric(typeof(AssetDictionaryReferenceCacheController<>))
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterGeneric(typeof(AssetReferenceCacheController<,>))
            .AsSelf();

        // Cache Serialization
        builder.RegisterGeneric(typeof(DictionaryReferenceCacheSerialization<,,,>))
            .AsSelf()
            .AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(AssetReferenceCacheSerialization<,>))
            .As(typeof(AssetReferenceCacheSerialization<,>));

        builder.RegisterType<RecordReferenceCacheSerialization>()
            .AsSelf()
            .AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(AssetReferenceCacheSerializationConfigString<>))
            .AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(AssetReferenceCacheSerializationConfigInt<>))
            .AsImplementedInterfaces();

        builder.RegisterType<ReferenceCacheSerializationConfigLink>()
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterType<AssetReferenceCacheSerializationConfig>()
            .AsImplementedInterfaces();

        builder.RegisterType<ModAssetReferenceCacheSerializationConfig>()
            .AsImplementedInterfaces();

        builder.RegisterType<ReferenceCacheBuilder>()
            .AsSelf();

        // Query
        builder.RegisterType<ModAssetSerializableQuery>()
            .AsSelf();

        builder.RegisterType<RecordReferenceQuery>()
            .AsSelf();

        builder.RegisterGeneric(typeof(FileSystemQuery<,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(ArchiveQuery<,>))
            .AsSelf();

        builder.RegisterType<RecordReferenceQuery>()
            .AsSelf();

        builder.RegisterType<RecordReferenceQueryConfig>()
            .AsSelf();

        builder.RegisterType<RecordGlobalVariableReferenceQuery>()
            .AsSelf();

        builder.RegisterType<RecordGlobalVariableReferenceQueryConfig>()
            .AsSelf();

        builder.RegisterType<RecordAssetReferenceQueryConfig>()
            .AsSelf();

        builder.RegisterGeneric(typeof(AssetReferenceCacheQueryConfig<>))
            .AsSelf();

        // Parser
        builder.RegisterType<NifSoundLinkParser>()
            .AsSelf();

        builder.RegisterType<NifTextureParser>()
            .AsSelf();

        builder.RegisterType<ScriptFileParser>()
            .AsSelf();

        builder.RegisterType<NifAddonNodeLinkParser>()
            .AsSelf();

        // Trigger
        builder.RegisterGeneric(typeof(DataSourceReferenceUpdateTrigger<,,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(ModReferenceUpdateTrigger<,,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(DictionaryAssetReferenceQueryConfig<,,>))
            .AsSelf();

        // Services
        builder.RegisterType<ReferenceService>()
            .As<IReferenceService>()
            .SingleInstance();
    }
}
