using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenTypeProvider {
    public bool GetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type);

    public string GetGameName(IMajorRecordGetter record);

    public string GetTypeName(IMajorRecordGetter record);

    public string GetTypeName(System.Type type);
}
