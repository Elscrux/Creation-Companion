using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class MultiModKeyConfigOptionVM(MultiModKeyConfigOption option, MultiModPickerVM multiModPickerVM) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => ModKeyOption;
    public MultiModKeyConfigOption ModKeyOption { get; } = option;

    public MultiModPickerVM ModPicker { get; } = multiModPickerVM;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial ModKey[] ModKeys { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.ModKeys).Select(x => (object?) x);

    public object? Value {
        get => ModKeys;
        set {
            if (value is ModKey[] modKeys) {
                ModKeys = modKeys;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.ModKeys)
        .Select(s => ModKeyOption?.Validate?.Invoke(s));
}
