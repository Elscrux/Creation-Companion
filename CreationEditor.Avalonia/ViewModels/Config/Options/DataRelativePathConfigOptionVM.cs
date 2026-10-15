using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Assets;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class DataRelativePathConfigOptionVM(DataRelativePathConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => DataRelativePathOption;
    public DataRelativePathConfigOption DataRelativePathOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial DataRelativePath DataRelativePath { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.DataRelativePath).Select(x => (object?) x);

    public object? Value {
        get => DataRelativePath;
        set {
            if (value is DataRelativePath dataRelativePath) {
                DataRelativePath = dataRelativePath;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.DataRelativePath)
        .Select(s => DataRelativePathOption?.Validate?.Invoke(s));
}
