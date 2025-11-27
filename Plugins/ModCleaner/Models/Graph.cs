using System.Diagnostics.CodeAnalysis;
using Noggog;
using QuickGraph;
using QuickGraph.Algorithms;
namespace ModCleaner.Models;

public sealed class Graph<TVertex, TEdge>() : IVertexAndEdgeListGraph<TVertex, TEdge>
    where TVertex : notnull
    where TEdge : Edge<TVertex> {
    private readonly HashSet<TVertex> _vertices = [];
    public IEnumerable<TVertex> Vertices => _vertices;
    private readonly Dictionary<TVertex, HashSet<TEdge>> _incomingEdges = [];
    public IReadOnlyDictionary<TVertex, HashSet<TEdge>> IncomingEdges => _incomingEdges;
    private readonly Dictionary<TVertex, HashSet<TEdge>> _outgoingEdges = [];
    public IReadOnlyDictionary<TVertex, HashSet<TEdge>> OutgoingEdges => _outgoingEdges;

    public bool IsVerticesEmpty => _vertices.Count == 0;
    public int VertexCount => _vertices.Count;
    public bool ContainsEdge(TEdge edge) => OutgoingEdges.TryGetValue(edge.Source, out var edges) && edges.Contains(edge);
    public bool IsEdgesEmpty => EdgeCount == 0;
    public int EdgeCount => Edges.Count();
    public IEnumerable<TEdge> Edges => OutgoingEdges.Values.SelectMany(e => e);

    private readonly Dictionary<TVertex, TryFunc<TVertex, IEnumerable<TEdge>>> _tryGetOutEdgesCache = [];

    public Graph(Graph<TVertex, TEdge> graph) : this() {
        _vertices = new HashSet<TVertex>(graph._vertices);
        _incomingEdges = new Dictionary<TVertex, HashSet<TEdge>>();
        _outgoingEdges = new Dictionary<TVertex, HashSet<TEdge>>();
    }

    public void AddVertex(TVertex vertex) {
        _vertices.Add(vertex);
    }

    public void RemoveVertex(TVertex vertex) {
        _vertices.Remove(vertex);
        _incomingEdges.Remove(vertex);
        _outgoingEdges.Remove(vertex);

        foreach (var edges in _incomingEdges.Values) {
            edges.RemoveWhere(e => Equals(e.Source, vertex));
        }

        foreach (var edges in _outgoingEdges.Values) {
            edges.RemoveWhere(e => Equals(e.Target, vertex));
        }
    }

    public void AddEdge(TEdge edge) {
        _vertices.Add(edge.Source);
        _vertices.Add(edge.Target);
        _outgoingEdges.GetOrAdd(edge.Source, () => []).Add(edge);
        _incomingEdges.GetOrAdd(edge.Target, () => []).Add(edge);
    }

    public List<TVertex>? ShortestPath(TVertex source, TVertex target) {
        if (!_tryGetOutEdgesCache.TryGetValue(source, out var shortestPathsDijkstra)) {
            shortestPathsDijkstra = this.ShortestPathsDijkstra(_ => 1, source);
            _tryGetOutEdgesCache.Add(source, shortestPathsDijkstra);
        }

        var success = shortestPathsDijkstra(target, out var path);
        if (!success || path is null) return null;

        return path.Select(e => e.Target).Prepend(source).ToList();
    }

    public bool IsDirected => true;
    public bool AllowParallelEdges => true;

    public bool ContainsVertex(TVertex vertex) => _vertices.Contains(vertex);
    public bool IsOutEdgesEmpty(TVertex v) => OutgoingEdges.ContainsKey(v);
    public int OutDegree(TVertex v) => OutgoingEdges.TryGetValue(v, out var edges) ? edges.Count : 0;
    public IEnumerable<TEdge> OutEdges(TVertex v) => OutgoingEdges.TryGetValue(v, out var edges) ? edges : [];
    public bool TryGetOutEdges(TVertex v, [UnscopedRef] out IEnumerable<TEdge> edges) {
        if (OutgoingEdges.TryGetValue(v, out var edgesSet)) {
            edges = edgesSet;
            return true;
        }

        edges = [];
        return false;
    }
    public TEdge OutEdge(TVertex v, int index) => OutgoingEdges[v].ElementAt(index);
    public bool ContainsEdge(TVertex source, TVertex target) =>
        OutgoingEdges.TryGetValue(source, out var edges) && edges.Any(e => EqualityComparer<TVertex>.Default.Equals(e.Target, target));
    public bool TryGetEdges(TVertex source, TVertex target, [UnscopedRef] out IEnumerable<TEdge> edges) {
        if (OutgoingEdges.TryGetValue(source, out var edgesSet)) {
            var matchingEdges = edgesSet.Where(e => EqualityComparer<TVertex>.Default.Equals(e.Target, target)).ToList();
            if (matchingEdges.Count != 0) {
                edges = matchingEdges;
                return true;
            }
        }

        edges = [];
        return false;
    }
    public bool TryGetEdge(TVertex source, TVertex target, [UnscopedRef] out TEdge edge) {
        if (OutgoingEdges.TryGetValue(source, out var edgesSet)) {
            var matchingEdge = edgesSet.FirstOrDefault(e => EqualityComparer<TVertex>.Default.Equals(e.Target, target));
            if (matchingEdge != null) {
                edge = matchingEdge;
                return true;
            }
        }

        edge = null!;
        return false;
    }
}
