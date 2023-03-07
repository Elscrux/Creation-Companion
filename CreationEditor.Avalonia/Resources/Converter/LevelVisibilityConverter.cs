using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CreationEditor.Extension;
using Serilog.Events;
namespace CreationEditor.Avalonia.Converter;

public sealed class LevelVisibilityConverter : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<Dictionary<LogEventLevel,bool>> LevelsVisibilityProperty
        = AvaloniaProperty.Register<LevelVisibilityConverter, Dictionary<LogEventLevel,bool>>(nameof(LevelsVisibility));

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
