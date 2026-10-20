using CreationEditor.Services.Search;
namespace CreationEditor.Skyrim.Tests.Search;

public sealed class SearchRankerTests {
    private readonly FuzzySearchRanker _ranker = new();

    [Fact]
    public void ExactMatchScoresHighest() {
        var match = _ranker.Match("Save", "Save");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.Exact, match.Kind);
    }

    [Fact]
    public void PrefixMatchIsRecognized() {
        var match = _ranker.Match("Save All", "Save");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.Prefix, match.Kind);
    }

    [Fact]
    public void WordStartMatchIsRecognized() {
        var match = _ranker.Match("Iron Book", "Bo");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.WordStart, match.Kind);
    }

    [Fact]
    public void SubstringMatchIsRecognized() {
        var match = _ranker.Match("NotABookItem", "Book");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.Substring, match.Kind);
    }

    [Fact]
    public void SubsequenceMatchIsRecognized() {
        var match = _ranker.Match("IronBook", "Irk");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.Subsequence, match.Kind);
    }

    [Fact]
    public void SubsequenceMatchMustStartAtWordStart() {
        // "rb" and "nk" start in the middle of "Iron", so there is no real anchor for the query
        Assert.Null(_ranker.Match("IronBook", "rb"));
        Assert.Null(_ranker.Match("IronBook", "nk"));
    }

    [Fact]
    public void UnrelatedQueryReturnsNoMatch() {
        Assert.Null(_ranker.Match("Iron Sword", "xyz"));
        Assert.Null(_ranker.Match("Save All", "qzx"));
    }

    [Fact]
    public void AcronymMatchIsRecognized() {
        var match = _ranker.Match("NonPlayerCharacter", "NPC");

        Assert.NotNull(match);
        Assert.Equal(SearchMatchKind.Acronym, match.Kind);
    }

    [Fact]
    public void NonMatchReturnsNull() {
        Assert.Null(_ranker.Match("Iron Sword", "Book"));
    }

    [Fact]
    public void CaseIsIgnored() {
        Assert.NotNull(_ranker.Match("Save", "save"));
        Assert.NotNull(_ranker.Match("SAVE", "save"));
    }

    [Fact]
    public void EmptyQueryReturnsNull() {
        Assert.Null(_ranker.Match("Save", ""));
        Assert.Null(_ranker.Match("Save", "   "));
    }

    [Fact]
    public void ExactMatchScoresHigherThanPrefix() {
        var exact = _ranker.Match("Save", "Save")!.Score;
        var prefix = _ranker.Match("Save All", "Save")!.Score;

        Assert.True(exact > prefix);
    }

    [Fact]
    public void EarlierMatchScoresHigher() {
        var early = _ranker.Match("IronBook", "Book")!.Score;
        var late = _ranker.Match("SomeIronBook", "Book")!.Score;

        Assert.True(early > late);
    }

    [Fact]
    public void AllTermsMustMatch() {
        Assert.Null(_ranker.Match("Iron Book", "Book Sword"));
        Assert.NotNull(_ranker.Match("Iron Book", "Book Iron"));
    }

    [Fact]
    public void WeightScalesScore() {
        var normal = _ranker.Match("Save", "Save")!.Score;
        var weighted = _ranker.Match("Save", "Save", 0.5)!.Score;

        Assert.Equal(normal / 2, weighted);
    }
}
