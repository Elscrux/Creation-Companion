using CreationEditor;
using CreationEditor.Services.DataSource;
using NifPlugin.ViewModels;
using Serilog;
namespace NifPlugin.Services;

public sealed class NifVMFactory(
    ILogger logger,
    Func<DataSourceFileLink, NifVM> nifVMFactory) {
    public NifVM? Create(
        IDataSourceLink link) {
        if (link is not DataSourceFileLink dataSourceFileLink) return null;
        if (!dataSourceFileLink.Exists()) return null;

        try {
            return nifVMFactory(dataSourceFileLink);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to load nif file for asset {AssetLink}: {Exception}", dataSourceFileLink, e);
        }

        return null;
    }
}
