using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenTypeProvider {
    /// <summary>
    /// Retrieve a mutagen type from a game name and type name
    /// </summary>
    /// <param name="gameName">Name of the game</param>
    /// <param name="typeName">Name of the type</param>
    /// <param name="type">Type of name and game</param>
    /// <returns>True if the type could be retrieved, false otherwise</returns>
    bool GetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type);

    /// <summary>
    /// Returns the game name of a mod
    /// </summary>
    /// <param name="mod">Mod to retrieve the game name for</param>
    /// <returns>Game name of a mod</returns>
    string GetGameName(IModGetter mod);

    /// <summary>
    /// Returns the game name of a record
    /// </summary>
    /// <param name="record">Record to retrieve the game name for</param>
    /// <returns>Game name of a record</returns>
    string GetGameName(IMajorRecordGetter record);

    /// <summary>
    /// Returns the type name of a record
    /// </summary>
    /// <param name="record">Record to retrieve the type name for</param>
    /// <returns>Type name of a record</returns>
    string GetTypeName(IMajorRecordGetter record);

    /// <summary>
    /// Returns the type name of a form link
    /// </summary>
    /// <param name="formLinkIdentifier">Form link to retrieve the type name for</param>
    /// <returns>Type name of a form link</returns>
    string GetTypeName(IFormLinkIdentifier formLinkIdentifier);
}
