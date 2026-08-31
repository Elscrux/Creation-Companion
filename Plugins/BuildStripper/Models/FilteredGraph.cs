using System.Diagnostics.CodeAnalysis;
using Noggog;
using QuickGraph;
namespace BuildStripper.Models;

public sealed class FilteredGraph<TVertex, TEdge>(IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph) : IVertexAndEdgeListGraph<TVertex, TEdge>
    where TVertex : notnull
    where TEdge : Edge<TVertex> {

    public bool IsDirected => baseGraph.IsDirected;
    public bool AllowParallelEdges => baseGraph.AllowParallelEdges;
    public bool ContainsVertex(TVertex vertex) => baseGraph.ContainsVertex(vertex);
    public bool IsOutEdgesEmpty(TVertex v) => baseGraph.IsOutEdgesEmpty(v);
    public int OutDegree(TVertex v) => baseGraph.OutDegree(v);
    public IEnumerable<TEdge> OutEdges(TVertex v) => baseGraph.OutEdges(v);
    public bool TryGetOutEdges(TVertex v, [UnscopedRef] out IEnumerable<TEdge> edges) => baseGraph.TryGetOutEdges(v, out edges);
    public TEdge OutEdge(TVertex v, int index) => baseGraph.OutEdge(v, index);
    public bool ContainsEdge(TVertex source, TVertex target) => baseGraph.ContainsEdge(source, target);
    public bool TryGetEdges(TVertex source, TVertex target, [UnscopedRef] out IEnumerable<TEdge> edges) => baseGraph.TryGetEdges(source, target, out edges);
    public bool TryGetEdge(TVertex source, TVertex target, [UnscopedRef] out TEdge edge) => baseGraph.TryGetEdge(source, target, out edge);
    public bool IsVerticesEmpty => baseGraph.IsVerticesEmpty;
    public int VertexCount => baseGraph.VertexCount;
    public IEnumerable<TVertex> Vertices => baseGraph.Vertices;
    public bool ContainsEdge(TEdge edge) => baseGraph.ContainsEdge(edge);
    public bool IsEdgesEmpty => baseGraph.IsEdgesEmpty;
    public int EdgeCount => baseGraph.EdgeCount;
    public IEnumerable<TEdge> Edges => baseGraph.Edges;

    private readonly HashSet<TVertex> _includedVertices = [];
    public IReadOnlySet<TVertex> IncludedVertices => _includedVertices;
    private readonly HashSet<TVertex> _excludedVertices = [];
    public IReadOnlySet<TVertex> ExcludedVertices => _excludedVertices;

    private Graph<TVertex, TEdge>? _filteredGraph;
    private Graph<TVertex, Edge<TVertex>>? _dependencyGraph;

    private readonly Dictionary<TVertex, HashSet<TVertex>> _includedVertexReasons = new();

    public void IncludeVertex(TVertex vertex, TVertex reasonToIncludeVertex) {
        if (vertex is FormLinkIdentifier { FormLink.IsNull: true }) return;
        if (reasonToIncludeVertex is FormLinkIdentifier { FormLink.IsNull: true }) return;

        _includedVertices.Add(vertex);
        _excludedVertices.Remove(vertex);
        _includedVertexReasons.GetOrAdd(vertex).Add(reasonToIncludeVertex);

        // Update graph if it's currently live
        if (_filteredGraph is not null && _dependencyGraph is not null) {
            IncludeVertexChain(_filteredGraph, _dependencyGraph, vertex, reasonToIncludeVertex);
        }
    }

    public void ExcludeVertex(TVertex vertex) {
        _excludedVertices.Add(vertex);
        _includedVertices.Remove(vertex);
        _includedVertexReasons.Remove(vertex);

        // Remove any vertices that were only included because of this vertex
        var verticesToRemove = _includedVertexReasons
            .Where(kvp => kvp.Value.Remove(vertex))
            .Where(kvp => kvp.Value.Count == 0)
            .Select(kvp => kvp.Key)
            .ToArray();

        _includedVertices.Remove(verticesToRemove);
        _includedVertexReasons.Remove(verticesToRemove);

        // Invalidate graph
        _filteredGraph = null;
        _dependencyGraph = null;
    }

    private (Graph<TVertex, TEdge> filteredGraph, Graph<TVertex, Edge<TVertex>> dependencyGraph) GetGraphs() {
        if (_filteredGraph is not null && _dependencyGraph is not null) return (_filteredGraph, _dependencyGraph);

        var newGraph = new Graph<TVertex, TEdge>();
        var dependencyGraph = new Graph<TVertex, Edge<TVertex>>();

        foreach (var includedVertex in _includedVertices.ToArray()) {
            IncludeVertexChain(newGraph, dependencyGraph, includedVertex, includedVertex);
        }

        _filteredGraph = newGraph;
        _dependencyGraph = dependencyGraph;

        return (_filteredGraph, _dependencyGraph);
    }

    public Graph<TVertex, TEdge> Build() {
        if (_filteredGraph is not null) return _filteredGraph;

        return GetGraphs().filteredGraph;
    }

    public Graph<TVertex, Edge<TVertex>> BuildDependencyGraph() {
        if (_dependencyGraph is not null) return _dependencyGraph;

        return GetGraphs().dependencyGraph;
    }

    private void IncludeVertexChain(Graph<TVertex, TEdge> graph, Graph<TVertex, Edge<TVertex>> depGraph, TVertex vertex, TVertex reason) {
        if (_excludedVertices.Contains(reason)) return;
        if (vertex is FormLinkIdentifier { FormLink.IsNull: true }) return;
        if (reason is FormLinkIdentifier { FormLink.IsNull: true }) return;

        _includedVertices.Add(vertex);
        _excludedVertices.Remove(vertex);
        _includedVertexReasons.GetOrAdd(vertex).Add(reason);

        depGraph.AddEdge(new Edge<TVertex>(reason, vertex));
        if (graph.ContainsVertex(vertex)) return;

        graph.AddVertex(vertex);
        foreach (var reasonVertex in _includedVertexReasons.GetOrAdd(vertex)) {
            depGraph.AddEdge(new Edge<TVertex>(reasonVertex, vertex));
        }

        foreach (var outEdge in baseGraph.OutEdges(vertex)) {
            var linkedVertex = outEdge.Target;
            if (_excludedVertices.Contains(linkedVertex)) continue;
            if (linkedVertex is FormLinkIdentifier { FormLink.IsNull: true }) continue;

            IncludeVertexChain(graph, depGraph, linkedVertex, reason);
            graph.AddEdge(outEdge);
        }
    }
}
