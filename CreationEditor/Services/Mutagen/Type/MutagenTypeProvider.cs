using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.Type;

public sealed class MutagenTypeProvider : IMutagenTypeProvider {
    private const string BaseNamespace = "Mutagen.Bethesda.";
    private const string GetterSuffix = "Getter";
    private const char InterfacePrefix = 'I';

    private readonly ConcurrentDictionary<string, System.Type> _typeCache = new();

    public System.Type GetType(ReadOnlySpan<char> gameName, ReadOnlySpan<char> typeName) {
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

    public System.Type GetType(string gameName, string typeName) {
        return GetType(gameName.AsSpan(), typeName.AsSpan());
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

    public string GetGameName(System.Type recordType) {
        return recordType.Namespace![BaseNamespace.Length..];
    }

    public string GetTypeName(IMajorRecordGetter record) {
        return record.Registration.ClassType.Name;
    }

    public string GetTypeName(IFormLinkIdentifier formLinkIdentifier) {
        // Selecting the type name from the name
        var name = formLinkIdentifier.Type.Name.AsSpan();

        // Cutting starting interface prefix if it exists
        if (name.StartsWith(InterfacePrefix)) {
            name = name[1..];
        }

        // Cutting ending getter suffix if it exists
        if (name.EndsWith(GetterSuffix)) {
            name = name[..^6];
        }

        return name.ToString();
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
        if (type.Name.EndsWith(GetterSuffix)) return type;

        var nameSpan = type.FullName.AsSpan();
        var lastIndexOfDot = nameSpan.LastIndexOf('.') + 1;
        var namespacePart = nameSpan[..lastIndexOfDot];
        var typePart = nameSpan[lastIndexOfDot..];
        var getterTypeName = typePart.StartsWith(InterfacePrefix)
            ? namespacePart.ToString() + typePart.ToString() + GetterSuffix
            : namespacePart.ToString() + InterfacePrefix + typePart.ToString() + GetterSuffix;

        var registration = LoquiRegistration.GetRegisterByFullName(getterTypeName)
         ?? throw new ArgumentException("Cannot find getter type for " + type.FullName, nameof(type));
        return registration.GetterType;
    }
}
