using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class BoolConfigOptionVM(BoolConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => BoolOption;
    public BoolConfigOption BoolOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial bool State { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.State).Select(x => (object?) x);

    public object? Value {
        get => State;
        set {
            if (value is bool b) {
                State = b;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.State)
        .Select(s => BoolOption?.Validate?.Invoke(s));
}
