using LeveledList.Model;
namespace LeveledList.Services;

public interface IFeatureProvider {
    IEnumerable<FeatureWildcardIdentifier> GetApplicableFeatureWildcards(Type t);
    FeatureWildcard GetFeatureWildcard(FeatureWildcardIdentifier featureWildcardIdentifier);
}
