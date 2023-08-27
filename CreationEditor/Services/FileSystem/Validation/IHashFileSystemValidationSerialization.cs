using CreationEditor.Services.Mutagen.References.Asset.Cache;
namespace CreationEditor.Services.FileSystem.Validation;

public interface IFileSystemValidationSerialization<T> {
    bool Validate(string rootDirectoryPath);

    T Deserialize(string rootDirectoryPath);
    void Serialize(T t, string rootDirectoryPath);
}

public interface IHashFileSystemValidationSerialization : IFileSystemValidationSerialization<HashFileSystemCacheData> {}
