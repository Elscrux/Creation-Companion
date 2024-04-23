using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.Type;

public sealed class MutagenTypeProvider : IMutagenTypeProvider {
    private const string BaseNamespace = "Mutagen.Bethesda.";

    private readonly Dictionary<string, System.Type> _typeCache = new();

    public System.Type GetType(string gameName, string typeName) {
        var name = $"{gameName}.{typeName}";
        if (_typeCache.TryGetValue(name, out var cachedType)) {
            return cachedType;
        }

        var registration = LoquiRegistration.GetRegisterByFullName(BaseNamespace + name);

        if (registration is null) {
            throw new ArgumentException("Unknown Mutagen type", BaseNamespace + name);
        }

        _typeCache.UpdateOrAdd(name, _ => registration.GetterType);
        return registration.GetterType;
    }

    public bool TryGetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type) {
        var name = $"{gameName}.{typeName}";
        if (_typeCache.TryGetValue(name, out var cachedType)) {
            type = cachedType;
            return true;
        }

        var registration = LoquiRegistration.GetRegisterByFullName(BaseNamespace + name);

        if (registration is null) {
            type = null;
            return false;
        }

        type = registration.GetterType;
        _typeCache.UpdateOrAdd(name, _ => registration.GetterType);
        return true;
    }

    public string GetGameName(IModGetter mod) {
        return mod.GetType().Namespace![BaseNamespace.Length..];
    }

    public string GetGameName(IMajorRecordGetter record) {
        return record.Registration.ProtocolKey.Namespace[BaseNamespace.Length..];
    }

    public string GetTypeName(IMajorRecordGetter record) {
        return record.Registration.ClassType.FullName!.Split('.')[^1];
    }

    public string GetTypeName(IFormLinkIdentifier formLinkIdentifier) {
        // Selecting the type name from the full name
        var fullName = formLinkIdentifier.Type.FullName;
        var startIndex = fullName!.LastIndexOf('.') + 2;

        // Cutting of the "Getter" part of the type name
        return fullName[startIndex..^6];
    }

    public IEnumerable<System.Type> GetRecordClassTypes(GameRelease gameRelease) {
        return GetRegistrations(gameRelease).Select(x => x.ClassType);
    }

    public IEnumerable<System.Type> GetRecordGetterTypes(GameRelease gameRelease) {
        return GetRegistrations(gameRelease).Select(x => x.GetterType);
    }

    public IEnumerable<System.Type> GetRecordSetterTypes(GameRelease gameRelease) {
        return GetRegistrations(gameRelease).Select(x => x.SetterType);
    }

    public IEnumerable<ILoquiRegistration> GetRegistrations(GameRelease gameRelease) {
        var gameCategory = gameRelease.ToCategory();
        var gameNamespace = BaseNamespace + gameCategory;

        return LoquiRegistration.StaticRegister.Registrations
            .Where(x =>
                string.Equals(x.ClassType.Namespace, gameNamespace, StringComparison.Ordinal)
             && x.GetterType.IsAssignableTo(typeof(IMajorRecordGetter)));
    }

    public System.Type GetRecordGetterType(System.Type type) {
        var lastIndexOfDot = type.FullName!.LastIndexOf('.') + 1;
        var getterTypeName = type.FullName[..lastIndexOfDot] + "I" + type.FullName[lastIndexOfDot..] + "Getter";

        var registration = LoquiRegistration.GetRegisterByFullName(getterTypeName);
        return registration!.GetterType;
    }
}
