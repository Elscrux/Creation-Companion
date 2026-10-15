using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class FloatConfigOptionVM(FloatConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => FloatOption;
    public FloatConfigOption FloatOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial float Float { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Float).Select(x => (object?) x);

    public object? Value {
        get => Float;
        set {
            if (value is float f) {
                Float = f;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Float)
        .Select(s => FloatOption?.Validate?.Invoke(s));
}
