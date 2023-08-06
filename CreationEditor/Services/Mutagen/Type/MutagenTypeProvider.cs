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

    /// <summary>
    /// Tries to retrieve a mutagen type via string
    /// </summary>
    /// <param name="gameName">name of a game like "Skyrim"</param>
    /// <param name="typeName">name of a type like "IArmorGetter"</param>
    /// <param name="type">type that was be retrieved from the provided string</param>
    /// <returns>true if the type could be found</returns>
    public bool GetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type) {
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
        return record.Registration.ClassType.FullName!.Split('.').Last();
    }

    public string GetTypeName(IFormLinkIdentifier formLinkIdentifier) {
        // Selecting the type name from the full name
        var fullName = formLinkIdentifier.Type.FullName;
        var startIndex = fullName!.LastIndexOf('.') + 2;

        // Cutting of the "Getter" part of the type name
        return fullName[startIndex..^6];
    }

    public IEnumerable<System.Type> GetRecordTypes(GameRelease gameRelease) {
        var gameCategory = gameRelease.ToCategory();
        var gameNamespace = $"{BaseNamespace}{gameCategory}.{gameCategory}Mod";
        var registration = LoquiRegistration.GetRegisterByFullName(gameNamespace);
        if (registration is null) yield break;

        foreach (var type in registration.ClassType
            .GetProperties()
            .Select(x => x.PropertyType.InheritsFrom(typeof(IGroupCommonGetter)) ? x.PropertyType.GetGenericArguments()[0] : null)
            .NotNull()) {
            yield return type;
        }
    }

    public System.Type GetRecordGetterType(System.Type type) {
        var lastIndexOfDot = type.FullName!.LastIndexOf('.') + 1;
        var getterTypeName = type.FullName[..lastIndexOfDot] + "I" + type.FullName[lastIndexOfDot..] + "Getter";

        var registration = LoquiRegistration.GetRegisterByFullName(getterTypeName);
        return registration!.GetterType;
    }
}
