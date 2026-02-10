using System.IO.Abstractions;
using System.Reactive.Subjects;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Prefix;
using CreationEditor.Avalonia.ViewModels.Utility;
using CreationEditor.Services.Environment;
using LeveledList.Resources;
using ReactiveUI.SourceGenerators;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace LeveledList.ViewModels;

/// <summary>
/// ViewModel for the generation configuration that holds all configuration options
/// for mod selection, prefix, regex replacements, filtering, and provides generation utilities.
/// </summary>
public sealed partial class GenerationConfigurationVM : ViewModel {
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly ReplaySubject<bool> _isBusy = new(1);

    [Reactive] public partial string? DefinitionsFolderPath { get; set; }
    [Reactive] public partial string FilterText { get; set; } = string.Empty;
    [Reactive] public partial MultiModPickerVM ModPicker { get; set; }
    [Reactive] public partial RecordPrefixVM RecordPrefixContent { get; set; }
    [Reactive] public partial RegexReplacementVM EditorIdReplacement { get; set; }
    [Reactive] public partial RegexReplacementVM? NameReplacement { get; set; }
    [Reactive] public partial bool ShowNameReplacement { get; set; }

    public IEditorEnvironment EditorEnvironment { get; }
    public IObservable<bool> IsBusy => _isBusy;

    public GenerationConfigurationVM(
        MultiModPickerVM modPicker,
        RecordPrefixVM recordPrefixContent,
        ILogger logger,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        RegexReplacementVM editorIdReplacement,
        RegexReplacementVM? nameReplacement = null,
        bool showNameReplacement = false) {
        ModPicker = modPicker;
        RecordPrefixContent = recordPrefixContent;
        EditorIdReplacement = editorIdReplacement;
        NameReplacement = nameReplacement;
        ShowNameReplacement = showNameReplacement;
        _logger = logger;
        _fileSystem = fileSystem;
        EditorEnvironment = editorEnvironment;
        SetBusy(false);
    }

    /// <summary>
    /// Sets the busy state for the preview generation.
    /// </summary>
    public void SetBusy(bool isBusy) => _isBusy.OnNext(isBusy);

    /// <summary>
    /// Gets definitions from YAML files using the provided deserializer function.
    /// </summary>
    public IEnumerable<T> GetDefinitionsFromYaml<T>(
        string directoryPath,
        Func<IDeserializer, StreamReader, string, string, IEnumerable<T>> deserializeFunc) {
        if (!_fileSystem.Directory.Exists(directoryPath)) yield break;

        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        foreach (var file in _fileSystem.Directory.EnumerateFiles(directoryPath, "*.yaml", SearchOption.AllDirectories)) {
            using var fileStream = _fileSystem.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = _fileSystem.Path.GetFileNameWithoutExtension(file);

            IEnumerable<T> definitions;
            try {
                definitions = deserializeFunc(deserializer, new StreamReader(fileStream), file, fileName);
            } catch (Exception e) {
                _logger.Here().Warning(e, "Failed to deserialize definition file: {File}", file);
                continue;
            }

            foreach (var definition in definitions) {
                yield return definition;
            }
        }
    }
}
