using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CreationEditor.Avalonia.Extension;
using Serilog.Events;
namespace CreationEditor.Avalonia.Converter;

public sealed class LogLevelToToBrushConverter : IValueConverter {
    public ISolidColorBrush? VerboseBrush { get; set; }
    public ISolidColorBrush? DebugBrush { get; set; }
    public ISolidColorBrush? MessageBrush { get; set; }
    public ISolidColorBrush? WarningBrush { get; set; }
    public ISolidColorBrush? ErrorBrush { get; set; }
    public ISolidColorBrush? FatalBrush { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (!EnumExtension.TryParse<LogEventLevel>(value, out var level)) return null;

        return level switch {
            LogEventLevel.Verbose => VerboseBrush,
            LogEventLevel.Debug => DebugBrush,
            LogEventLevel.Information => MessageBrush,
            LogEventLevel.Warning => WarningBrush,
            LogEventLevel.Error => ErrorBrush,
            LogEventLevel.Fatal => FatalBrush,
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public sealed class LevelVisibilityConverter : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<Dictionary<LogEventLevel,bool>> LevelsVisibilityProperty = AvaloniaProperty.Register<LevelVisibilityConverter, Dictionary<LogEventLevel,bool>>("LevelsVisibility");

    public Dictionary<LogEventLevel,bool> LevelsVisibility {
        get => GetValue(LevelsVisibilityProperty);
        set => SetValue(LevelsVisibilityProperty, value);
    }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (!EnumExtension.TryParse<LogEventLevel>(value, out var level)) return null;

        return LevelsVisibility.TryGetValue(level, out var visibility) && visibility;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}