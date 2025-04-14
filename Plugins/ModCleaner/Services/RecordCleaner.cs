using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace ModCleaner.Services;

public sealed class RecordCleaner(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    IRecordReferenceController recordReferenceController) {

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var masters = mod.GetTransitiveMasters(editorEnvironment.GameEnvironment).ToArray();
        foreach (var record in mod.EnumerateMajorRecords()) {
            // Add all transitive dependencies of the record
            var queue = new Queue<IFormLinkIdentifier>([record.ToFormLinkInformation()]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                graph.AddVertex(new FormLinkIdentifier(current));

                foreach (var currentReference in recordReferenceController.GetReferences(current.FormKey)) {
                    // This just checks if the reference was defined in the mod or one of its dependencies. Update if needed.
                    var modKey = currentReference.FormKey.ModKey;
                    if (modKey != mod.ModKey
                     && !masters.Contains(modKey)
                     && !dependencies.Contains(modKey)) continue;

                    var currentReferenceLink = new FormLinkIdentifier(currentReference);
                    if (!graph.Vertices.Contains(currentReferenceLink)) {
                        queue.Enqueue(currentReference);
                    }

                    graph.AddEdge(new Edge<ILinkIdentifier>(currentReferenceLink, new FormLinkIdentifier(current)));
                }
            }
        }
    }
}