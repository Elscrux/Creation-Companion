using Avalonia.Data.Converters;
using Loqui;
namespace CreationEditor.Avalonia.Converter;

public abstract class RecordTypeConverters {
    public static readonly FuncValueConverter<Type, string?> ToName
        = new(type => {
            if (type == null) return null;

            return LoquiRegistration.TryGetRegister(type, out var register)
                ? register.ClassType.Name
                : type.Name;
        });

    public static readonly FuncValueConverter<Type, string?> ToGame
        = new(type => {
            if (type == null) return null;

            return LoquiRegistration.TryGetRegister(type, out var registration)
                ? registration.ProtocolKey.Namespace
                : string.Empty;
        });
}
