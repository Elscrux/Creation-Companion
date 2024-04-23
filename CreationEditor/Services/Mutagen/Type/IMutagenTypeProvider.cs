using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenTypeProvider {
    /// <summary>
    /// Retrieve a mutagen type from a game name and type name
    /// </summary>
    /// <param name="gameName">Name of the game</param>
    /// <param name="typeName">Name of the type</param>
    /// <returns>Type of name and game</returns>
    /// <exception cref="ArgumentException">Thrown when the type could not be found</exception>
    System.Type GetType(string gameName, string typeName);

    /// <summary>
    /// Try to retrieve a mutagen type from a game name and type name
    /// </summary>
    /// <param name="gameName">Name of the game</param>
    /// <param name="typeName">Name of the type</param>
    /// <param name="type">Type of name and game</param>
    /// <returns>True if the type could be retrieved, false otherwise</returns>
    bool TryGetType(string gameName, string typeName, [MaybeNullWhen(false)] out System.Type type);

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

    /// <summary>
    /// Returns all record class types of a game release 
    /// </summary>
    /// <param name="gameRelease">Game release to get types for</param>
    /// <returns>Record class types of the game</returns>
    IEnumerable<System.Type> GetRecordClassTypes(GameRelease gameRelease);

    /// <summary>
    /// Returns all record getter types of a game release 
    /// </summary>
    /// <param name="gameRelease">Game release to get types for</param>
    /// <returns>Record getter types of the game</returns>
    IEnumerable<System.Type> GetRecordGetterTypes(GameRelease gameRelease);

    /// <summary>
    /// Returns all record setter types of a game release 
    /// </summary>
    /// <param name="gameRelease">Game release to get types for</param>
    /// <returns>Record setter types of the game</returns>
    IEnumerable<System.Type> GetRecordSetterTypes(GameRelease gameRelease);

    /// <summary>
    /// Returns all record registrations of a game release 
    /// </summary>
    /// <param name="gameRelease">Game release to get types for</param>
    /// <returns>Record registrations of the game</returns>
    IEnumerable<ILoquiRegistration> GetRegistrations(GameRelease gameRelease);

    /// <summary>
    /// Returns the getter type of record class type
    /// </summary>
    /// <param name="type">Record class type</param>
    /// <returns>Getter type of the record</returns>
    System.Type GetRecordGetterType(System.Type type);
}
