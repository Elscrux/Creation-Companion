using CreationEditor.Services.DataSource;
namespace CreationEditor.Avalonia.Services.Actions;

public interface IAssetContextActionsProvider {
    void OpenAssets(params IEnumerable<IDataSourceLink> dataSourceLinks);
    Task RenameAsset(IDataSourceLink asset);
    Task DeleteAssets(params IReadOnlyList<IDataSourceLink> deleteAssets);
    Task AddFolder(DataSourceDirectoryLink dir);
    Task CopyAssetPath(IDataSourceLink dataSourceLink);
    Task MoveAssets(DataSourceDirectoryLink dstDirectory, params IReadOnlyList<IDataSourceLink> movingAssets);
}
