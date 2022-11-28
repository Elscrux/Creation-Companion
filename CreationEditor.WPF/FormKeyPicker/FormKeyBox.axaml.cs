using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
namespace CreationEditor.WPF.FormKeyPicker;

public class FormKeyBox : TemplatedControl {
    private bool _blockSync = false;

    public FormKey FormKey {
        get => GetValue(FormKeyProperty);
        set => SetValue(FormKeyProperty, value);
    }
    public static readonly StyledProperty<FormKey> FormKeyProperty = AvaloniaProperty.Register<FormKeyBox, FormKey>(nameof(FormKey), defaultBindingMode: BindingMode.TwoWay);

    public string RawString {
        get => GetValue(RawStringProperty);
        set => SetValue(RawStringProperty, value);
    }
    public static readonly StyledProperty<string> RawStringProperty = AvaloniaProperty.Register<FormKeyBox, string>(nameof(RawString), defaultBindingMode: BindingMode.TwoWay);

    private ErrorResponse _error;
    public ErrorResponse Error {
        get => _error;
        protected set => SetAndRaise(ErrorProperty, ref _error, value);
    }
    public static readonly DirectProperty<FormKeyBox, ErrorResponse> ErrorProperty = AvaloniaProperty.RegisterDirect<FormKeyBox, ErrorResponse>(nameof(Error), box => box.Error, defaultBindingMode: BindingMode.TwoWay);

    public string Watermark {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }
    public static readonly StyledProperty<string> WatermarkProperty = AvaloniaProperty.Register<FormKeyBox, string>(nameof(Watermark), "FormKey");

    static FormKeyBox() {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyBox), new FrameworkPropertyMetadata(typeof(FormKeyBox))); todo
    }

    protected override void OnLoaded() {
        base.OnLoaded();
        this.WhenAnyValue(x => x.FormKey)
            .DistinctUntilChanged()
            .Subscribe(x => {
                if (_blockSync) return;

                _blockSync = true;
                if (x.IsNull) {
                    if (RawString != String.Empty) {
                        RawString = string.Empty;
                    }
                } else {
                    var str = x.ToString();
                    if (!str.Equals(RawString, StringComparison.OrdinalIgnoreCase)) {
                        RawString = str;
                    }
                }
                _blockSync = false;
            });
            // .DisposeWith(_unloadDisposable); todo
        this.WhenAnyValue(x => x.RawString)
            .DistinctUntilChanged()
            .Subscribe(x => {
                if (_blockSync) return;

                _blockSync = true;
                if (FormKey.TryFactory(x, out var formKey)) {
                    if (formKey != FormKey) {
                        FormKey = formKey;
                    }
                    if (Error.Failed) {
                        Error = ErrorResponse.Success;
                    }
                } else {
                    if (x.IsNullOrWhitespace()) {
                        if (Error.Failed) {
                            Error = ErrorResponse.Success;
                        }
                    } else {
                        if (Error.Succeeded) {
                            Error = ErrorResponse.Fail("Could not convert to FormKey");
                        }
                    }
                    if (!FormKey.IsNull) {
                        FormKey = FormKey.Null;
                    }
                }
                _blockSync = false;
            });
            // .DisposeWith(_unloadDisposable); todo
    }
}
