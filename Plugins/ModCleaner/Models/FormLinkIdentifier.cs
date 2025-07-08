using Mutagen.Bethesda.Plugins;
namespace ModCleaner.Models;

public record FormLinkIdentifier(IFormLinkIdentifier FormLink) : ILinkIdentifier;
