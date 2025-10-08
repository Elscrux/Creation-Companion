namespace CreationEditor.Services.DataSource;

public static class DataSourceServiceExtensions {
    public static IEnumerable<DataSourceFileLink> EnumerateModFileLinks(this IDataSourceService dataSourceService) {
        return dataSourceService.EnumerateFileLinksInAllDataSources(string.Empty, false)
            .Where(fileLink => fileLink.Extension
                is ModTypeExtension.MasterFileExtension
                or ModTypeExtension.PluginFileExtension
                or ModTypeExtension.LightPluginFileExtension);
    }
}
