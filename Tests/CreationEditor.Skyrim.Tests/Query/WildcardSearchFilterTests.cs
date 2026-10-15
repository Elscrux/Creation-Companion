using CreationEditor.Services.Filter;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class WildcardSearchFilterTests {
    private readonly WildcardSearchFilter _filter = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptySearchTerm_MatchesEverything(string? searchTerm) {
        Assert.True(_filter.Filter("Anything", searchTerm!));
    }

    [Theory]
    [InlineData("Skyrim.esm", "skyrim")]
    [InlineData("Skyrim.esm", "SKYRIM")]
    [InlineData("Skyrim.esm", "rim")]
    public void SingleTerm_MatchesCaseInsensitive(string content, string searchTerm) {
        Assert.True(_filter.Filter(content, searchTerm));
    }

    [Theory]
    [InlineData("Skyrim.esm", "missing")]
    [InlineData("Skyrim.esm", "SkyrimEsm")]
    public void SingleTerm_NoMatch_ReturnsFalse(string content, string searchTerm) {
        Assert.False(_filter.Filter(content, searchTerm));
    }

    [Theory]
    [InlineData(@"Textures\Cliffs\cliffside.dds", "textures*cliff*dds")]
    [InlineData(@"Textures\Cliffs\cliffside.dds", "textures*side")]
    [InlineData("aaa", "a*a")]
    [InlineData("abab", "ab*ab")]
    public void MultipleTerms_InOrder_Match(string content, string searchTerm) {
        Assert.True(_filter.Filter(content, searchTerm));
    }

    [Theory]
    [InlineData(@"Textures\Cliffs\cliffside.dds", "cliff*textures")]
    [InlineData("abc", "b*a")]
    public void MultipleTerms_WrongOrder_NoMatch(string content, string searchTerm) {
        Assert.False(_filter.Filter(content, searchTerm));
    }

    [Theory]
    [InlineData("a", "a*a")]
    [InlineData("ab", "ab*ab")]
    [InlineData("abc", "abc*abc")]
    [InlineData("ba", "a**b")]
    public void TermsCannotReuseTheSameMatch(string content, string searchTerm) {
        // Each term must match a distinct part of the content, in order
        Assert.False(_filter.Filter(content, searchTerm));
    }

    [Fact]
    public void TrailingWildcard_Matches() {
        Assert.True(_filter.Filter("Skyrim.esm", "Skyrim*"));
    }

    [Fact]
    public void LeadingWildcard_Matches() {
        Assert.True(_filter.Filter("Skyrim.esm", "*esm"));
    }
}
