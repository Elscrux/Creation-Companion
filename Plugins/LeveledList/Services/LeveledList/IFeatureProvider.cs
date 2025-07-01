using LeveledList.Model.Feature;
namespace LeveledList.Services.LeveledList;

public interface IFeatureProvider {
    IEnumerable<FeatureWildcardIdentifier> GetApplicableFeatureWildcards(Type t);
    FeatureWildcard GetFeatureWildcard(FeatureWildcardIdentifier featureWildcardIdentifier);
}
