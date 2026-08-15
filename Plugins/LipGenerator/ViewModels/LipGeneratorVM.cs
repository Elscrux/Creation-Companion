using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using LipGenerator.Services;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace LipGenerator.ViewModels;

public sealed partial class LipGeneratorVM : ViewModel {
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly GameConstants _gameConstants;
    private readonly LipFileGeneratorFactory _lipFileGeneratorFactory;

    public SingleDataSourcePickerVM DataSourcePicker { get; }

    [Reactive] public partial bool CleanupFiles { get; set; } = true;
    [Reactive] public partial string FaceFxWrapperPath { get; set; } = string.Empty;
    [Reactive] public partial string XwmEncoderPath { get; set; } = string.Empty;
    [Reactive] public partial string FonixDataPath { get; set; } = string.Empty;
    [Reactive] public partial string LipGeneratorPath { get; set; } = string.Empty;
    [Reactive] public partial string LipFuzerPath { get; set; } = string.Empty;

    // Tool selection
    [Reactive] public partial LipGeneratorTool SelectedLipGenerator { get; set; } = LipGeneratorTool.LipGenerator;
    [Reactive] public partial LipFuzerTool SelectedLipFuzer { get; set; } = LipFuzerTool.LipFuzer;
    [Reactive] public partial AudioFormatOption SelectedAudioFormat { get; set; } = AudioFormatOption.XWM;

    // Advanced settings
    [Reactive] public partial ReadOnlyMemorySlice<Language> AvailableLanguages { get; private set; }
    [Reactive] public partial Language SelectedLanguage { get; set; } = Language.English;
    [Reactive] public partial float GestureExaggeration { get; set; } = 1.0f;
    [Reactive] public partial float LipAnimSpeed { get; set; } = 1.0f;
    [Reactive] public partial float LipAnimDelay { get; set; } = 0.0f;
    [Reactive] public partial int XwmBitrate { get; set; } = 192000;
    [Reactive] public partial int ParallelizationDegree { get; set; } = Math.Max(1, Environment.ProcessorCount - 1);

    // Slider ranges
    public float MinGestureExaggeration => 0.5f;
    public float MaxGestureExaggeration => 3.0f;
    public float MinLipAnimSpeed => 0.5f;
    public float MaxLipAnimSpeed => 2.0f;
    public float MinLipAnimDelay => 0.0f;
    public float MaxLipAnimDelay => 2.0f;
    public double MinParallelizationDegree => 1;
    public double MaxParallelizationDegree => Environment.ProcessorCount;

    // Available options
    public IReadOnlyList<LipGeneratorTool> AvailableLipGenerators { get; } = Enum.GetValues<LipGeneratorTool>();
    public IReadOnlyList<LipFuzerTool> AvailableLipFuzers { get; } = Enum.GetValues<LipFuzerTool>();
    public IReadOnlyList<AudioFormatOption> AvailableAudioFormats { get; } = Enum.GetValues<AudioFormatOption>();

    // Progress tracking
    [Reactive] public partial bool IsGenerating { get; set; }
    [Reactive] public partial int ProgressValue { get; set; }
    [Reactive] public partial string ProgressText { get; set; } = string.Empty;

    public IObservable<bool> CanGenerateLip { get; }

    public LipGeneratorVM(
        IFileSystem fileSystem,
        ILogger logger,
        GameConstants gameConstants,
        IGameDirectoryProvider gameDirectoryProvider,
        LipFileGeneratorFactory lipFileGeneratorFactory,
        SingleDataSourcePickerVM dataSourcePicker) {
        _fileSystem = fileSystem;
        _logger = logger;
        _gameConstants = gameConstants;
        _lipFileGeneratorFactory = lipFileGeneratorFactory;
        DataSourcePicker = dataSourcePicker;
        AvailableLanguages = GetCurrentSupportedLanguages();

        DataSourcePicker.Filter = dataSource => !dataSource.IsReadOnly;

        // Auto-detect paths in the game folder (only if they exist)
        var gameDirectory = gameDirectoryProvider.Path;
        if (gameDirectory is not null) {
            FaceFxWrapperPath = GetIfExists(gameDirectory, "Tools", "Audio", "FaceFXWrapper.exe");
            XwmEncoderPath = GetIfExists(gameDirectory, "Tools", "Audio", "xwmaencode.exe");
            FonixDataPath = GetIfExists(gameDirectory, "Data", "Sound", "Voice", "Processing", "FonixData.cdf");
            LipGeneratorPath = GetIfExists(gameDirectory, "Tools", "LipGen", "LipGenerator", "LipGenerator.exe");
            LipFuzerPath = GetIfExists(gameDirectory, "Tools", "LipGen", "LipFuzer", "LIPFuzer.exe");
        }

        // Update available languages when the selected lip generator changes
        this.WhenAnyValue(x => x.SelectedLipGenerator)
            .Subscribe(_ => {
                AvailableLanguages = GetCurrentSupportedLanguages();
                if (!AvailableLanguages.Contains(SelectedLanguage)) {
                    SelectedLanguage = AvailableLanguages.FirstOrDefault();
                }
            })
            .DisposeWith(this);

        // Validation for enabling the Generate button
        var dataSourceValid = this.WhenAnyValue(x => x.DataSourcePicker.SelectedDataSource)
            .Select(ds => ds is not null);
        var faceFxWrapperValid = this.WhenAnyValue(
            x => x.SelectedLipGenerator,
            x => x.FaceFxWrapperPath,
            x => x.FonixDataPath,
            (selected, path, fonix) => selected != LipGeneratorTool.FaceFXWrapper
             || (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(fonix)));
        var lipGeneratorValid = this.WhenAnyValue(
            x => x.SelectedLipGenerator,
            x => x.LipGeneratorPath,
            (selected, path) => selected != LipGeneratorTool.LipGenerator
             || !string.IsNullOrWhiteSpace(path));
        var lipFuzerValid = this.WhenAnyValue(
            x => x.SelectedLipFuzer,
            x => x.LipFuzerPath,
            (selected, path) => selected != LipFuzerTool.LipFuzer
             || !string.IsNullOrWhiteSpace(path));
        var xwmEncoderValid = this.WhenAnyValue(
            x => x.SelectedAudioFormat,
            x => x.XwmEncoderPath,
            (selected, path) => selected != AudioFormatOption.XWM
             || !string.IsNullOrWhiteSpace(path));

        CanGenerateLip = dataSourceValid
            .CombineLatest(
                faceFxWrapperValid,
                lipGeneratorValid,
                lipFuzerValid,
                xwmEncoderValid,
                (a, b, c, d, e) => a && b && c && d && e)
            .ObserveOnGui();
    }

    [ReactiveCommand(CanExecute = nameof(CanGenerateLip))]
    private void GenerateLip() {
        var dataSource = DataSourcePicker.SelectedDataSource;
        if (dataSource is null) {
            _logger.Here().Error("No data source selected");
            return;
        }

        if (dataSource.Path is null || !_fileSystem.Directory.Exists(dataSource.Path)) {
            _logger.Here().Error("Data source path '{Path}' does not exist", dataSource.Path ?? "(null)");
            return;
        }

        // Create argument objects based on selected tools with all settings
        ILipGeneratorArgs lipGenArgs = SelectedLipGenerator switch {
            LipGeneratorTool.FaceFXWrapper => new FaceFxWrapperArgs(FaceFxWrapperPath, FonixDataPath),
            LipGeneratorTool.LipGenerator => new LipGeneratorArgs(LipGeneratorPath, SelectedLanguage, GestureExaggeration, LipAnimSpeed, LipAnimDelay),
            _ => throw new NotSupportedException($"Unsupported lip generator tool: {SelectedLipGenerator}")
        };

        IFuzGeneratorArgs fuzGenArgs = SelectedLipFuzer switch {
            LipFuzerTool.LipFuzer => new LipFuzerArgs(LipFuzerPath),
            _ => throw new NotSupportedException($"Unsupported fuz generator tool: {SelectedLipFuzer}")
        };

        IAudioEncoderArgs? xwmEncoderArgs = SelectedAudioFormat switch {
            AudioFormatOption.XWM => new XwmEncoderArgs(XwmEncoderPath, XwmBitrate),
            AudioFormatOption.WAV => null, // Skip encoding - wav is already what we start with
            _ => throw new NotSupportedException($"Unsupported audio format: {SelectedAudioFormat}")
        };

        var lipFileGenerator = _lipFileGeneratorFactory.Create(lipGenArgs, fuzGenArgs, xwmEncoderArgs);

        IsGenerating = true;
        ProgressValue = 0;
        ProgressText = "Initializing...";

        Task.Run(() => {
                lipFileGenerator.Run(
                    dataSource,
                    CleanupFiles,
                    parallelizationDegree: ParallelizationDegree,
                    onProgress: (value, text) => {
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

    private string GetIfExists(params IEnumerable<string> paths) {
        var combinedPath = _fileSystem.Path.Combine(paths.ToArray());
        if (_fileSystem.File.Exists(combinedPath)) {
            return combinedPath;
        }

        return string.Empty;
    }

    private ReadOnlyMemorySlice<Language> GetCurrentSupportedLanguages() {
        var supportedLanguages = SelectedLipGenerator switch {
            LipGeneratorTool.FaceFXWrapper => FaceFxWrapper.SupportedLanguages,
            LipGeneratorTool.LipGenerator => LipGeneratorWrapper.SupportedLanguages,
            _ => throw new ArgumentOutOfRangeException()
        };

        return _gameConstants.Languages
            .Intersect(supportedLanguages)
            .ToArray();
    }
}
