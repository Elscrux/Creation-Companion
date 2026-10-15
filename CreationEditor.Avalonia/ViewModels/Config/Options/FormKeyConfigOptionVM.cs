using System.Reactive.Linq;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class FormKeyConfigOptionVM(FormKeyConfigOption option, ILinkCacheProvider linkCacheProvider) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => FormKeyOption;
    public FormKeyConfigOption FormKeyOption { get; } = option;
    public ILinkCacheProvider LinkCacheProvider { get; } = linkCacheProvider;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial FormKey FormKey { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.FormKey).Select(x => (object?) x);

    public object? Value {
        get => FormKey;
        set {
            if (value is FormKey formKey) {
                FormKey = formKey;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.FormKey)
        .Select(s => FormKeyOption?.Validate?.Invoke(s));
}
