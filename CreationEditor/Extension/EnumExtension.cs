using System.Diagnostics.CodeAnalysis;
namespace CreationEditor.Extension;

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
}
