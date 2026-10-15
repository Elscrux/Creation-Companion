using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class LongConfigOptionVM(LongConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => LongOption;
    public LongConfigOption LongOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial long Long { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Long).Select(x => (object?) x);

    public object? Value {
        get => Long;
        set {
            if (value is int or long or short or byte or sbyte or uint or ulong or ushort or float or double) {
                Long = Convert.ToInt64(value);
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Long)
        .Select(s => LongOption?.Validate?.Invoke(s));
}
