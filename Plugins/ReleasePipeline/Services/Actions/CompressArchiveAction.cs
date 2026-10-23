using CreationEditor;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Archive;
using Mutagen.Bethesda.Plugins;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services.Actions;

public sealed class ArchiveActionConfig : IConfigDefinition {
    public static TextConfigOption Suffix { get; } = new(
        "Archive Suffix",
        "Suffix to append to the archive name like '_xbox' for 'MyMod_xbox.bsa'. Leave empty to use the default name.",
        isOptional: true
    );

    public static BoolConfigOption CreateDummyPlugins { get; } = new(
        "Create Dummy Plugins",
        "If enabled, the action will create dummy plugin files for any archives that don't have a corresponding mod file. This is useful for ensuring that the game loads all archives.",
        true
    );

    public static IntegrationConfigOption ArchiveCreatorOption { get; } = new(
        "Archive Creator",
        typeof(IArchiveCreator)
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        Suffix,
        CreateDummyPlugins,
        ArchiveCreatorOption,
    ];
}

/// <summary>
/// Pipeline action that compresses loose files into an archive using a configured archive creator integration.
/// The action references a registered integration by id and invokes its archive-creation method.
/// </summary>
public sealed class CompressArchiveAction(IIntegrationRegistry registry, IEditorEnvironment editorEnvironment) : IPipelineAction {
    public Guid Id { get; } = new("a14dc045-5233-48a0-9903-e19bc80d6115");
    public string DisplayName => "Create Archive";
    public string Description => "Compresses files from the data source into a archive and puts it in the release folder.";
    public bool RequiresInPlaceModification => false;
    public IConfigDefinition ConfigDefinition { get; } = new ArchiveActionConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public async Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var archiveCreatorOption = ConfigValues.Get(ArchiveActionConfig.ArchiveCreatorOption);
        var integration = registry.TryGetIntegration<IArchiveCreator>(archiveCreatorOption.IntegrationId);
        if (integration is null) {
            return PipelineActionResult.Fail("No archive creator integration is configured for this action.");
        }

        var suffix = ConfigValues.TryGet(ArchiveActionConfig.Suffix);
        var createDummyPlugins = ConfigValues.Get(ArchiveActionConfig.CreateDummyPlugins);
        var source = new FileSystemDataSource(context.FileSystem, context.WorkingDirectory, true);
        var targetArchivePath = context.FileSystem.Path.Combine(context.ReleaseFolder, $"{context.Mod.ModKey.Name}{suffix}.bsa");

        var result = await integration.CreateArchive(
            source,
            targetArchivePath,
            archiveCreatorOption.ConfigValues,
            token);

        CreateTexturesArchives(context, suffix);

        if (createDummyPlugins) {
            var dummyPlugins = CreateDummyPlugins(context, suffix).ToArray();
            if (dummyPlugins.Length > 0) {
                result = result with {
                    Message = $"{result.Message}\n\nCreated dummy plugins: {string.Join(", ", dummyPlugins)}",
                };
            }
        }

        return result.Succeeded
            ? PipelineActionResult.Pass(result.Message)
            : PipelineActionResult.Fail(result.Message);
    }

    /// <summary>
    /// The result of the archive output can look something like this:
    /// MyModSUFFIX.bsa
    /// MyModSUFFIX2.bsa
    /// MyModSUFFIX3.bsa
    /// 
    /// What we actually want is this:
    /// MyModSUFFIX.bsa
    /// MyModSUFFIX - Textures.bsa
    /// MyModSUFFIX2.bsa
    /// </summary>
    /// <param name="context"></param>
    /// <param name="suffix"></param>
    public static void CreateTexturesArchives(PipelineContext context, string? suffix = null) {
        var bsaFiles = context.FileSystem.Directory.EnumerateFiles(context.ReleaseFolder, "*.bsa").ToArray();
        for (var i = 1; i <= bsaFiles.Length; i++) {
            var bsaFile = bsaFiles[i - 1];

            var index = (i - 1) / 2 + 1;
            var indexString = index <= 1 ? string.Empty : index.ToString();
            if (i % 2 == 0) {
                var newBSAFile = context.FileSystem.Path.Combine(context.ReleaseFolder, $"{context.Mod.ModKey.Name}{suffix}{indexString} - Textures.bsa");
                context.FileSystem.File.Move(bsaFile, newBSAFile);
            } else {
                var newBSAFile = context.FileSystem.Path.Combine(context.ReleaseFolder, $"{context.Mod.ModKey.Name}{suffix}{indexString}.bsa");
                context.FileSystem.File.Move(bsaFile, newBSAFile);
            }
        }
    }

    /// <summary>
    /// Creates dummy plugins for any archives that don't have a corresponding mod file.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public IEnumerable<string> CreateDummyPlugins(PipelineContext context, string? suffix = null) {
        var modExtensions = Enum.GetValues<ModType>().Select(modType => modType.ToFileExtension()).ToArray();
        foreach (var archivePath in context.FileSystem.Directory.EnumerateFiles(context.ReleaseFolder, "*.bsa")) {
            // If the archive name ends with "- Textures" it doesn't need a corresponding mod file, so we skip it
            var normalizedPath = archivePath;
            var archiveNameWithoutExtension = context.FileSystem.Path.GetFileNameWithoutExtension(archivePath);
            if (archiveNameWithoutExtension.EndsWith("- Textures", StringComparison.OrdinalIgnoreCase)) continue;

            if (suffix is not null && archiveNameWithoutExtension.Contains(suffix, StringComparison.OrdinalIgnoreCase)) {
                normalizedPath = normalizedPath.Replace(suffix, string.Empty, StringComparison.OrdinalIgnoreCase);
                archiveNameWithoutExtension = archiveNameWithoutExtension.Replace(suffix, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            if (modExtensions.Any(e => {
                var modFileName = context.FileSystem.Path.ChangeExtension(normalizedPath, e);
                var modFilePath = context.FileSystem.Path.Combine(context.ReleaseFolder, modFileName);
                return context.FileSystem.File.Exists(modFilePath);
            })) {
                // This archive already has a corresponding mod file, skip
                continue;
            }

            // This archive has no corresponding mod file, so we will create a dummy mod file for it to load it
            var dummyModFileName = archiveNameWithoutExtension + ModType.Master.ToFileExtension();
            var dummyModKey = ModKey.FromFileName(dummyModFileName);
            var dummyMod = editorEnvironment.AddNewMutableMod(dummyModKey);
            var dummyModFilePath = context.FileSystem.Path.Combine(context.ReleaseFolder, dummyModFileName);
            dummyMod.WriteToBinary(dummyModFilePath);

            yield return dummyModFileName;
        }

    }
}
