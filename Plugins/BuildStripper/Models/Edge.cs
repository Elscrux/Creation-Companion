using QuickGraph;
namespace BuildStripper.Models;

public record Edge<TVertex>(TVertex Source, TVertex Target) : IEdge<TVertex>;
