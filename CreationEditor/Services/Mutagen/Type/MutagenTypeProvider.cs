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
        var span = record.Registration.ClassType.FullName.AsSpan();
        var lastIndexOfDot = span.LastIndexOf('.');
        return span[(lastIndexOfDot + 1)..].ToString();
    }

    public string GetTypeName(IFormLinkIdentifier formLinkIdentifier) {
        // Selecting the type name from the full name
        var fullName = formLinkIdentifier.Type.FullName.AsSpan();
        var startIndex = fullName.LastIndexOf('.') + 2;

        // Cutting of the "Getter" part of the type name
        return fullName[startIndex..^6].ToString();
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
        if (type.Name.EndsWith("Getter")) return type;

        var nameSpan = type.FullName.AsSpan();
        var lastIndexOfDot = nameSpan.LastIndexOf('.') + 1;
        var namespacePart = nameSpan[..lastIndexOfDot];
        var typePart = nameSpan[lastIndexOfDot..];
        var getterTypeName = typePart.StartsWith('I')
            ? namespacePart.ToString() + typePart.ToString() + "Getter"
            : namespacePart.ToString() + "I" + typePart.ToString() + "Getter";

        var registration = LoquiRegistration.GetRegisterByFullName(getterTypeName)
         ?? throw new ArgumentException("Cannot find getter type for " + type.FullName, nameof(type));
        return registration.GetterType;
    }
}
