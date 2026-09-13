using System.IO.Abstractions;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Models;

/// <summary>
/// Context handed to each <see cref="IPipelineAction"/> when a pipeline runs.
/// </summary>
public sealed record PipelineContext(ISkyrimModGetter Mod, string ReleaseFolder, IFileSystem FileSystem);
