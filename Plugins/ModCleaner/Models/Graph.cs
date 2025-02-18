using Noggog;
namespace ModCleaner.Models;

public sealed class Graph<TVertex, TEdge>
    where TVertex : notnull
    where TEdge : Edge<TVertex> {
    public HashSet<TVertex> Vertices { get; } = [];

    public Dictionary<TVertex, HashSet<TEdge>> IncomingEdges { get; } = [];
    public Dictionary<TVertex, HashSet<TEdge>> OutgoingEdges { get; } = [];

    public void AddVertex(TVertex vertex) {
        Vertices.Add(vertex);
    }

    public void AddEdge(TEdge edge) {
        Vertices.Add(edge.Source);
        Vertices.Add(edge.Target);
        OutgoingEdges.GetOrAdd(edge.Source, () => []).Add(edge);
        IncomingEdges.GetOrAdd(edge.Target, () => []).Add(edge);
    }
}
