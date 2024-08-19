using CreationEditor.Services.Cache.Validation;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.FileSystem.Validation;

public interface IFileSystemValidation : IInternalCacheValidation<string, DataRelativePath>;
