using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Plugin;
using CreationEditor.Skyrim.Avalonia.Modules;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace DialogueExporter.Cli.Args;

public static class CommandLineContainerSetup {
    public static IContainer? Setup<TArguments>(TArguments args, string modFilename)
        where TArguments : class, IDataSourceArguments {
        var builder = new ContainerBuilder();

        builder.RegisterModule<MutagenModule>();
        builder.RegisterModule<EditorModule>();
        builder.RegisterModule<SkyrimModule>();
        builder.RegisterModule<DialogueExporterModule>();
        builder.RegisterInstance(args);

        var container = builder.Build();

        // how to load data sources and the load order usings args (reusable??)
        var dataSourceService = container.Resolve<IDataSourceService>();
        var activeDataSourcePath = args.ActiveDataSourcePath;
        if (args.ActiveDataSourcePath.IsNullOrWhitespace()) {
            activeDataSourcePath = dataSourceService.DataDirectoryDataSource.Path;
        }

        var fileSystem = container.Resolve<IFileSystem>();
        var modDataSource = new FileSystemDataSource(fileSystem, activeDataSourcePath);

        var dataSources = ((FileSystemDataSource[]) [
                ..(args.IncludeDataDirectoryDataSource ? (FileSystemDataSource[]) [dataSourceService.DataDirectoryDataSource] : []),
                ..args.AdditionalDataSourcePaths.Select(path => new FileSystemDataSource(fileSystem, path)),
                modDataSource,
            ])
            .DistinctBy(ds => ds.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        dataSourceService.UpdateDataSources(dataSources, modDataSource);

        var modFileLink = dataSourceService.GetFileLink(modFilename);
        if (modFileLink is null) {
            Console.WriteLine("Could not find mod " + modFilename + " in data sources");
            return null;
        }

        var modKey = ModKey.FromFileName(modFilename);

        var modInfoProvider = container.Resolve<IModInfoProvider>();
        var allAvailableModInfos = dataSourceService.EnumerateModFileLinks()
            .Select(modInfoProvider.GetModInfo)
            .WhereNotNull()
            .ToArray();

        // Ensure all masters are available and valid
        var masterInfos = modInfoProvider.GetMasterInfos(allAvailableModInfos);
        var masterInfo = masterInfos[modKey];
        if (!masterInfo.Valid) {
            Console.WriteLine("Not all masters for mod " + modFilename + " are available");
            return container;
        }

        var editorEnvironment = container.Resolve<IEditorEnvironment>();
        editorEnvironment.Update(updater => updater
            .LoadOrder.AddImmutableMods([..masterInfo.Masters, modKey])
            .Build());
        return container;
    }
}
