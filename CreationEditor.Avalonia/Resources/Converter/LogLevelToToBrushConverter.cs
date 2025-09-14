using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Serilog.Events;
namespace CreationEditor.Avalonia.Converter;

public sealed class LogLevelToToBrushConverter : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<IBrush?> VerboseBrushProperty =
        AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(VerboseBrush));
    public IBrush? VerboseBrush {
        get => GetValue(VerboseBrushProperty);
        set => SetValue(VerboseBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> DebugBrushProperty =
        AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(DebugBrush));
    public IBrush? DebugBrush {
        get => GetValue(DebugBrushProperty);
        set => SetValue(DebugBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> MessageBrushProperty
        = AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(MessageBrush));
    public IBrush? MessageBrush {
        get => GetValue(MessageBrushProperty);
        set => SetValue(MessageBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> WarningBrushProperty =
        AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(WarningBrush));
    public IBrush? WarningBrush {
        get => GetValue(WarningBrushProperty);
        set => SetValue(WarningBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> ErrorBrushProperty =
        AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(ErrorBrush));
    public IBrush? ErrorBrush {
        get => GetValue(ErrorBrushProperty);
        set => SetValue(ErrorBrushProperty, value);
    }

    public static readonly StyledProperty<IBrush?> FatalBrushProperty =
        AvaloniaProperty.Register<LogLevelToToBrushConverter, IBrush?>(nameof(FatalBrush));
    public IBrush? FatalBrush {
        get => GetValue(FatalBrushProperty);
        set => SetValue(FatalBrushProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (!EnumExtension.TryParse<LogEventLevel>(value, out var level)) return null;

        return level switch {
            LogEventLevel.Verbose => VerboseBrush,
            LogEventLevel.Debug => DebugBrush,
            LogEventLevel.Information => MessageBrush,
            LogEventLevel.Warning => WarningBrush,
            LogEventLevel.Error => ErrorBrush,
            LogEventLevel.Fatal => FatalBrush,
            _ => null,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new InvalidOperationException();
    }
}
