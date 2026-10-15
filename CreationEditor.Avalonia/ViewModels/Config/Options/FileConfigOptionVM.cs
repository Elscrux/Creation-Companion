using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class FileConfigOptionVM(FileConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => FileOption;
    public FileConfigOption FileOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial string FilePath { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.FilePath).Select(x => (object?) x);

    public object? Value {
        get => FilePath;
        set {
            if (value is string s) {
                FilePath = s;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.FilePath)
        .Select(s => FileOption?.Validate?.Invoke(s));
}
