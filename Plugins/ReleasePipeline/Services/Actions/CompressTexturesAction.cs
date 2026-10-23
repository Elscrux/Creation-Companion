using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Textures;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using Serilog;
namespace ReleasePipeline.Services.Actions;

public sealed class CompressTexturesConfig : IConfigDefinition {
    public static IntegrationConfigOption TextureProcessorOption { get; } = new(
        "Texture Processor",
        typeof(ITextureProcessor),
        "Integration used to convert, compress and resize the textures"
    );

    public static IntegrationConfigOption TextureInfoOption { get; } = new(
        "Texture Info Provider",
        typeof(ITextureInfoProvider),
        "Integration used to read texture metadata (format, size, alpha) for rule evaluation"
    );

    public static ListConfigOption<TextureRule> RulesOption { get; } = new(
        "Texture Processing Rules",
        "Ordered rules; the first matching rule is applied to a texture"
    );

    public static TextConfigOption FileExtensionsOption { get; } = new(
        "File Extensions",
        "Texture file extensions to process, separated by commas. Example: .dds,.png,.tga",
        ".dds,.png,.tga,.jpg,.jpeg"
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        TextureProcessorOption,
        TextureInfoOption,
        RulesOption,
        FileExtensionsOption,
    ];
}

/// <summary>
/// Pipeline action that copies textures from the data source into the release folder and
/// processes them there (format conversion, compression, resizing) according to an ordered
/// rule list. The first matching rule is applied; unmatched textures are copied unchanged.
/// </summary>
public sealed class CompressTexturesAction(
    ILogger logger,
    IFileSystem fileSystem,
    IIntegrationRegistry registry) : IPipelineAction {
    public Guid Id { get; } = new("7f4b9e2c-3d18-4a56-b9c0-8e2f5a1d6b74");
    public string DisplayName => "Compress Textures";
    public string Description => "Copies textures to the release folder and converts, compresses or resizes them according to rules.";
    public bool RequiresInPlaceModification => true;
    public IConfigDefinition ConfigDefinition { get; } = new CompressTexturesConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public async Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var processorOption = ConfigValues.Get(CompressTexturesConfig.TextureProcessorOption);
        var processor = registry.TryGetIntegration<ITextureProcessor>(processorOption.IntegrationId);
        if (processor is null) {
            return PipelineActionResult.Fail("No texture processor integration is configured for this action.");
        }

        var infoOption = ConfigValues.Get(CompressTexturesConfig.TextureInfoOption);
        var infoProvider = registry.TryGetIntegration<ITextureInfoProvider>(infoOption.IntegrationId);
        if (infoProvider is null) {
            return PipelineActionResult.Fail("No texture info provider integration is configured for this action.");
        }

        var rules = ConfigValues.Get(CompressTexturesConfig.RulesOption);
        var extensions = ConfigValues.Get(CompressTexturesConfig.FileExtensionsOption)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var files = extensions
            .SelectMany(extension => fileSystem.Directory.GetFiles(context.WorkingDirectory, $"*{extension}", SearchOption.AllDirectories))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var converted = 0;
        var unchanged = 0;
        var skipped = 0;
        var failed = new List<string>();

        await Parallel.ForEachAsync(files,
            token,
            async (file, t) => {
                t.ThrowIfCancellationRequested();

                var relativePath = fileSystem.Path.GetRelativePath(context.WorkingDirectory, file);

                var infoResult = await infoProvider.GetTextureInfo(file, t);
                if (!infoResult.Succeeded || infoResult.Infos is null || !infoResult.Infos.TryGetValue(file, out var info)) {
                    failed.Add($"{relativePath}: {infoResult.Message}");
                    return;
                }

                var rule = rules.FirstOrDefault(r => r.ConfigDefinition.Matches(r.ConfigValues, info, relativePath));
                if (rule is null) {
                    unchanged++;
                    logger.Here().Verbose("No rule matched texture {Texture}, left unchanged", relativePath);
                    return;
                }

                var targetFormat = rule.ConfigValues.TryGetStruct<TextureFormat>(TextureRule.TargetFormatOption);
                var targetSize = rule.ConfigDefinition.GetTargetSize(rule.ConfigValues, info);
                if (targetFormat is null && targetSize is null) {
                    skipped++;
                    logger.Here().Verbose("Rule matched texture {Texture} but requires no changes", relativePath);
                    return;
                }

                var result = await processor.ProcessTexture(
                    file,
                    fileSystem.Path.GetDirectoryName(file)!,
                    targetFormat,
                    targetSize?.Width,
                    targetSize?.Height,
                    processorOption.ConfigValues,
                    t);

                if (!result.Succeeded) {
                    failed.Add($"{relativePath}: {result.Message}");
                    return;
                }

                // Remove the original copy when the tool wrote the output under a different name (e.g. png -> dds)
                if (result.OutputFilePath is not null
                 && !fileSystem.Path.GetFullPath(result.OutputFilePath).Equals(fileSystem.Path.GetFullPath(file), StringComparison.OrdinalIgnoreCase)) {
                    fileSystem.File.Delete(file);
                }

                converted++;
            });

        var summary = $"{converted} processed, {skipped} matched but unchanged, {unchanged} left unchanged, {failed.Count} failed.";
        if (failed.Count > 0) {
            var details = string.Join("\n", failed);
            return failed.Count == files.Length
                ? PipelineActionResult.Fail($"All textures failed to process. {summary}\n{details}")
                : PipelineActionResult.Warn($"Some textures failed to process. {summary}\n{details}");
        }

        return PipelineActionResult.Pass($"Textures processed successfully. {summary}");
    }
}
