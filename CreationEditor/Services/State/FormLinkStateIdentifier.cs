using System.Globalization;
using System.IO.Abstractions;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.State;

/// <summary>
/// Saved in folder structure first grouped by mod key, then by type.
/// <code>
/// Skyrim
///     Skyrim.esm
///         Armor
///             ABCDEF.json - for form link ABCDEF:Skyrim.esm of type armor
///         Weapon
///             012345.json - for form link 012345:Skyrim.esm of type weapon
///     Dragonborn.esm
///     ...
/// Fallout4
///     ...
/// </code>
/// </summary>
public sealed class FormLinkStateIdentifier(
    IFileSystem fileSystem,
    IMutagenTypeProvider mutagenTypeProvider) : IStateIdentifier<IFormLinkGetter> {
    public IFormLinkGetter Parse(ReadOnlySpan<char> identifier) {
        var separatorIndex = identifier.IndexOf(fileSystem.Path.DirectorySeparatorChar);
        if (separatorIndex < 0) {
            separatorIndex = identifier.IndexOf(fileSystem.Path.AltDirectorySeparatorChar);
            if (separatorIndex < 0) {
                throw new InvalidOperationException("Invalid identifier format: no directory separator found in " + identifier.ToString());
            }
        }

        var gameName = identifier[..separatorIndex];
        identifier = identifier[(separatorIndex + 1)..];

        separatorIndex = identifier.IndexOf(fileSystem.Path.DirectorySeparatorChar);
        if (separatorIndex < 0) {
            separatorIndex = identifier.IndexOf(fileSystem.Path.AltDirectorySeparatorChar);
            if (separatorIndex < 0) {
                throw new InvalidOperationException("Invalid identifier format: no directory separator found in " + identifier.ToString());
            }
        }

        var modKey = ModKey.FromNameAndExtension(identifier[..separatorIndex]);
        identifier = identifier[(separatorIndex + 1)..];

        separatorIndex = identifier.IndexOf(fileSystem.Path.DirectorySeparatorChar);
        if (separatorIndex < 0) {
            separatorIndex = identifier.IndexOf(fileSystem.Path.AltDirectorySeparatorChar);
            if (separatorIndex < 0) {
                throw new InvalidOperationException("Invalid identifier format: no directory separator found in " + identifier.ToString());
            }
        }

        var type = mutagenTypeProvider.GetType(gameName, identifier[..separatorIndex]);

        var formId = uint.Parse(identifier[(separatorIndex + 1)..], NumberStyles.HexNumber);
        var formKey = new FormKey(modKey, formId);

        return new FormLinkInformation(formKey, type);
    }

    public string AsFileName(IFormLinkGetter t) {
        var formKey = t.FormKey;
        var recordType = t.Type;
        return fileSystem.Path.Combine(
            mutagenTypeProvider.GetGameName(recordType),
            formKey.ModKey.FileName,
            recordType.Name,
            formKey.IDString());
    }
}
