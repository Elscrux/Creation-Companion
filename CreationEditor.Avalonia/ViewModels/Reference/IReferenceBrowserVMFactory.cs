using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public interface IReferenceBrowserVMFactory {
    ReferenceBrowserVM? GetReferenceBrowserVM(params IReadOnlyList<IDataSourceLink> assets);
    ReferenceBrowserVM? GetReferenceBrowserVM(IReferenced referencedAsset);
}
