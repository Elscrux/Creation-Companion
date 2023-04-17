using System.Diagnostics.CodeAnalysis;
namespace CreationEditor;

public static class EnumExtension {
    public static bool TryParse<TEnum>([NotNullWhen(true)] object? value, out TEnum outEnum)
        where TEnum : struct, Enum {
        switch (value) {
            case TEnum directLevel:
                outEnum = directLevel;
                return true;
            case string levelString when Enum.TryParse<TEnum>(levelString, out var parsedLevel):
                outEnum = parsedLevel;
                return true;
            default:
                outEnum = default;
                return false;
        }
    }

    public static Enum ToEnum(this Enum @enum) {
        return @enum.ToEnum(@enum.GetType());
    }

    public static Enum ToEnum(this Enum @enum, Type type) {
        return (Enum) Enum.ToObject(type, @enum);
    }

    public static TEnum ToEnum<TEnum>(this Enum @enum)
        where TEnum : Enum {
        return (TEnum) Enum.ToObject(typeof(TEnum), @enum);
    }
}
