using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Config;
using CreationEditor.Avalonia.ViewModels.Config.Options;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.Config;
using LipGenerator.Services;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace LipGenerator.ViewModels;

public sealed partial class LipGeneratorVM : ViewModel {
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly LipFileGenerator _lipFileGenerator;

    public SingleDataSourcePickerVM DataSourcePicker { get; }

    /// <summary>Config view model for the lip generator, lip fuzer and audio encoder integrations</summary>
    public ConfigVM ConfigVM { get; }

    [Reactive] public partial bool CleanupFiles { get; set; } = true;
    [Reactive] public partial int ParallelizationDegree { get; set; } = Math.Max(1, Environment.ProcessorCount - 1);

    public double MinParallelizationDegree => 1;
    public double MaxParallelizationDegree => Environment.ProcessorCount;

    // Progress tracking
    [Reactive] public partial bool IsGenerating { get; set; }
    [Reactive] public partial int ProgressValue { get; set; }
    [Reactive] public partial string ProgressText { get; set; } = string.Empty;

    public IObservable<bool> CanGenerateLip { get; }

    public LipGeneratorVM(
        IFileSystem fileSystem,
        ILogger logger,
        LipFileGenerator lipFileGenerator,
        IConfigVMFactory configVMFactory,
        SingleDataSourcePickerVM dataSourcePicker) {
        _fileSystem = fileSystem;
        _logger = logger;
        _lipFileGenerator = lipFileGenerator;
        DataSourcePicker = dataSourcePicker;

        ConfigVM = configVMFactory.Create(new LipGeneratorConfig());
        ConfigVM.DisposeWith(this);

        DataSourcePicker.Filter = dataSource => !dataSource.IsReadOnly;

        CanGenerateLip = this.WhenAnyValue(x => x.DataSourcePicker.SelectedDataSource)
            .CombineLatest(ConfigVM.ConfigChanged,
                (dataSource, _) => dataSource is not null
                 && ConfigVM.OptionVMs
                        .OfType<IntegrationConfigOptionVM>()
                        .All(x => x.SelectedIntegration is not null))
            .ObserveOnGui();
    }

    [ReactiveCommand(CanExecute = nameof(CanGenerateLip))]
    private void GenerateLip() {
        var dataSource = DataSourcePicker.SelectedDataSource;
        if (dataSource is null) {
            _logger.Here().Error("No data source selected");
            return;
        }

        var configValues = ConfigVM.GetConfigValues();
        var lipGeneratorConfig = configValues.TryGet(LipGeneratorConfig.LipGeneratorOption);
        var fuzGeneratorConfig = configValues.TryGet(LipGeneratorConfig.FuzGeneratorOption);
        var audioEncoderConfig = configValues.TryGet(LipGeneratorConfig.AudioEncoderOption);

        if (lipGeneratorConfig is null || fuzGeneratorConfig is null) {
            _logger.Here().Error("No lip generator or lip fuzer integration selected");
            return;
        }

        if (dataSource.Path is null || !_fileSystem.Directory.Exists(dataSource.Path)) {
            _logger.Here().Error("Data source path '{Path}' does not exist", dataSource.Path ?? "(null)");
            return;
        }

        IsGenerating = true;
        ProgressValue = 0;
        ProgressText = "Initializing...";

        Task.Run(async () => {
                await _lipFileGenerator.Run(
                    dataSource,
                    CleanupFiles,
                    lipGeneratorConfig,
                    fuzGeneratorConfig,
                    audioEncoderConfig,
                    ParallelizationDegree,
                    (value, text) => {
                        ProgressValue = value;
                        ProgressText = text;
                    });

                ProgressText = "Completed!";
                IsGenerating = false;
            })
            .FireAndForget(e => {
                _logger.Here().Error(e, "Error generating lips: {Message}", e.Message);
                ProgressText = $"Error: {e.Message}";
                IsGenerating = false;
            });
    }
}
