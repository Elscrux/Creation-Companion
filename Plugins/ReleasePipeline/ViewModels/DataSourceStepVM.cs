using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Wizard step 1: select the data source to release and detect the single mod it contains.
/// </summary>
public sealed partial class DataSourceStepVM : ViewModel {
    private static readonly string[] PluginExtensions = [
        ModTypeExtension.MasterFileExtension,
        ModTypeExtension.PluginFileExtension,
        ModTypeExtension.LightPluginFileExtension,
    ];

    private readonly IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> _editorEnvironment;
    private readonly Func<SingleDataSourcePickerVM> _additionalDataSourcePickerFactory;

    public SingleDataSourcePickerVM DataSourcePickerVM { get; }
    public ObservableCollection<SingleDataSourcePickerVM> AdditionalDataSourcePickers { get; } = [];

    [Reactive] public partial PipelineContext? PipelineContext { get; set; }
    [Reactive] public partial string DataSourceError { get; set; }
    [Reactive] public partial string? DetectedModName { get; set; }
    [Reactive] public partial string ReleaseFolder { get; set; }

    public DataSourceStepVM(
        SingleDataSourcePickerVM dataSourcePickerVM,
        Func<SingleDataSourcePickerVM> additionalDataSourcePickerFactory,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment) {
        DataSourcePickerVM = dataSourcePickerVM;
        DataSourcePickerVM.Filter = d => !d.IsReadOnly;
        _additionalDataSourcePickerFactory = additionalDataSourcePickerFactory;
        _editorEnvironment = editorEnvironment;

        var additionalDataSourcesChanged = AdditionalDataSourcePickers.ToObservableChangeSet()
            .ToCollection()
            .Select(rows => rows.Count == 0
                ? Observable.Return<IReadOnlyList<IDataSource>>([])
                : rows
                    .Select(row => row.SelectedDataSourceChanged)
                    .CombineLatest()
                    .Select(selections => selections.OfType<IDataSource>().ToList()))
            .Switch();

        DataSourcePickerVM.SelectedDataSourceChanged
            .CombineLatest(additionalDataSourcesChanged,
                (dataSource, additionalDataSources) => (DataSource: dataSource, Additional: additionalDataSources))
            .Subscribe(x => {
                UpdateReleaseContext(x.DataSource, x.Additional);
            })
            .DisposeWith(this);

        // Auto-select the first data source with a valid setup (one detected mod).
        var firstValid = DataSourcePickerVM.DataSources.FirstOrDefault(IsValidDataSource);
        if (firstValid is not null) {
            DataSourcePickerVM.SelectDataSource(firstValid);
        }
    }

    [ReactiveCommand]
    private void AddAdditionalDataSource() {
        var picker = _additionalDataSourcePickerFactory();
        picker.Filter = d => !d.IsReadOnly;
        AdditionalDataSourcePickers.Add(picker);
    }

    [ReactiveCommand]
    private void RemoveAdditionalDataSource(SingleDataSourcePickerVM picker) {
        AdditionalDataSourcePickers.Remove(picker);
        picker.Dispose();
    }

    /// <summary>True when the data source contains exactly one resolvable plugin mod</summary>
    private bool IsValidDataSource(IDataSource dataSource) {
        var pluginFiles = dataSource.EnumerateFiles(
                new DataRelativePath(string.Empty),
                includeSubDirectories: false)
            .Where(file => PluginExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (pluginFiles.Count != 1) return false;

        var modKey = ModKey.FromFileName(pluginFiles[0].Name);
        return _editorEnvironment.ResolveMod(modKey) is not null;
    }

    private void UpdateReleaseContext(IDataSource? dataSource, IReadOnlyList<IDataSource> additionalDataSources) {
        PipelineContext = null;
        DetectedModName = null;
        DataSourceError = string.Empty;
        ReleaseFolder = string.Empty;

        if (dataSource is null) return;

        var pluginFiles = dataSource.EnumerateFiles(
                new DataRelativePath(string.Empty),
                includeSubDirectories: false)
            .Where(file => PluginExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (pluginFiles.Count == 0) {
            DataSourceError = "The selected data source contains no plugin (esp/esm/esl) file.";
            return;
        }

        if (pluginFiles.Count > 1) {
            DataSourceError = "The selected data source must contain exactly one plugin (esp/esm/esl) file.";
            return;
        }

        var modKey = ModKey.FromFileName(pluginFiles[0].Name);
        var mod = _editorEnvironment.ResolveMod(modKey);
        if (mod is null) {
            DataSourceError = $"The mod '{modKey}' is not in the load order.";
            return;
        }

        DetectedModName = modKey.FileName;

        var releaseFolder = dataSource.FileSystem.Path.Combine(dataSource.Path, ReleasePipelinePlugin.ReleaseDirectory);

        ReleaseFolder = releaseFolder;
        PipelineContext = new PipelineContext(mod, dataSource, additionalDataSources, releaseFolder);
    }
}
