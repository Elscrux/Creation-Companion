using CreationEditor.Services.Cache.Validation;
namespace CreationEditor.Services.FileSystem.Validation;

public interface IFileSystemValidation : IInternalCacheValidation<string, string>;
