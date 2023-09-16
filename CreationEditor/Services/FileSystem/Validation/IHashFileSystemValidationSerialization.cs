using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
namespace CreationEditor.Services.FileSystem.Validation;

public interface IFileSystemValidationSerialization<T> {
    bool Validate(string rootDirectoryPath);

    bool TryDeserialize(string rootDirectoryPath, [MaybeNullWhen(false)] out T t);
    void Serialize(T t, string rootDirectoryPath);
}

public interface IHashFileSystemValidationSerialization : IFileSystemValidationSerialization<HashFileSystemCacheData> {}
