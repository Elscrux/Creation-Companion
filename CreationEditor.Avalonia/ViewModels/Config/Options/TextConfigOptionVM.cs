using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class TextConfigOptionVM(TextConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => TextOption;
    public TextConfigOption TextOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial string Text { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Text).Select(object? (x) => x);

    public object? Value {
        get => Text;
        set {
            if (value is string s) {
                Text = s;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Text)
        .Select(s => TextOption?.Validate?.Invoke(s));
}
