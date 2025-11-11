namespace CreationEditor.Services.DataSource;

public static class DataSourceServiceExtensions {
    extension(IDataSourceService dataSourceService) {
        public IEnumerable<DataSourceFileLink> EnumerateModFileLinks() {
            return dataSourceService.EnumerateFileLinksInAllDataSources(string.Empty, false)
                .Where(fileLink => fileLink.Extension
                    is ModTypeExtension.MasterFileExtension
                    or ModTypeExtension.PluginFileExtension
                    or ModTypeExtension.LightPluginFileExtension);
        }
    }
}
