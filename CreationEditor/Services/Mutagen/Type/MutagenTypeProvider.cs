using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenTypeProvider {
    public bool GetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type);
    
    public string GetGameName(IMajorRecordGetter record);
    
    public string GetTypeName(IMajorRecordGetter record);
    
    public string GetTypeName(System.Type type);
}

public class MutagenTypeProvider : IMutagenTypeProvider {
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

        if (registration == null) {
            type = null;
            return false;
        }

        type = registration.GetterType;
        _typeCache.Add(name, type);
        return true;
    }

    public string GetGameName(IMajorRecordGetter record) {
        return record.Registration.ProtocolKey.Namespace;
    }

    public string GetTypeName(IMajorRecordGetter record) {
        return record.Registration.ClassType.FullName!.Split('.').Last();
    }

    public string GetTypeName(System.Type type) {
        var fullName = type.FullName;
        var startIndex = fullName!.LastIndexOf('.') + 2;

        return fullName[startIndex..^6];
    }
}
