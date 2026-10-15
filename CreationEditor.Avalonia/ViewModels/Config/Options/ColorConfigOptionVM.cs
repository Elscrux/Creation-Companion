using System.Reactive.Linq;
using Avalonia.Media;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class ColorConfigOptionVM(ColorConfigOption option) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => ColorOption;
    public ColorConfigOption ColorOption { get; } = option;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial Color Color { get; set; } = new(option.DefaultValue.A, option.DefaultValue.R, option.DefaultValue.G, option.DefaultValue.B);

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Color).Select(x => (object?) x);

    public object? Value {
        get => System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
        set {
            if (value is System.Drawing.Color c) {
                Color = Color.FromArgb(c.A, c.R, c.G, c.B);
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Color)
        .Select(s => ColorOption?.Validate?.Invoke(System.Drawing.Color.FromArgb(s.A, s.R, s.G, s.B)) ?? null);
}
