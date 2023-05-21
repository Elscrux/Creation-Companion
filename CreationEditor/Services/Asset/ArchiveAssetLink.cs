namespace CreationEditor.Services.Asset;

public record ArchiveAssetLink(string Path, string? ArchivePath) : IArchivePathContainer;
