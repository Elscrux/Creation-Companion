using LeveledList.Model.Feature;
namespace LeveledList.Services.LeveledList;

public interface IFeatureProvider {
    FeatureWildcard GetFeatureWildcard(FeatureWildcardIdentifier featureWildcardIdentifier);
}
