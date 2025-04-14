using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.FileSystem.Validation;

public interface IFileSystemValidation : IInternalCacheValidation<FileSystemLink, DataRelativePath>;
