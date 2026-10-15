using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class EnumConfigOptionVM(EnumConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => EnumOption;
    public EnumConfigOption EnumOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial Enum? Enum { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Enum).Select(x => (object?) x);

    public object? Value {
        get => Enum ?? (object) 0;
        set {
            if (value is Enum e) {
                Enum = e;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Enum)
        .Select(s => s is null ? null : EnumOption?.Validate?.Invoke(s));
}
