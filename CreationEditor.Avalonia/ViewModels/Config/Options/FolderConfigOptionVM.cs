using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class FolderConfigOptionVM(FolderConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => FolderOption;
    public FolderConfigOption FolderOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial string FolderPath { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.FolderPath).Select(x => (object?) x);

    public object? Value {
        get => FolderPath;
        set {
            if (value is string s) {
                FolderPath = s;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.FolderPath)
        .Select(s => FolderOption?.Validate?.Invoke(s));
}
