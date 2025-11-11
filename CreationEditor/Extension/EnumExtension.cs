using System.Diagnostics.CodeAnalysis;
namespace CreationEditor;

public static class EnumExtension {
    public static bool TryParse<TEnum>([NotNullWhen(true)] object? value, out TEnum outEnum)
        where TEnum : struct, Enum {
        switch (value) {
            case TEnum directLevel:
                outEnum = directLevel;
                return true;
            case string enumString when Enum.TryParse<TEnum>(enumString, out var parsedEnum):
                outEnum = parsedEnum;
                return true;
            default:
                outEnum = default;
                return false;
        }
    }

    extension(Enum @enum) {
        public Enum ToEnum() => @enum.ToEnum(@enum.GetType());
        public Enum ToEnum(Type type) => (Enum) Enum.ToObject(type, @enum);
        public TEnum ToEnum<TEnum>() where TEnum : Enum => (TEnum) Enum.ToObject(typeof(TEnum), @enum);
    }
}
