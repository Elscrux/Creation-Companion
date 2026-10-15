using CreationEditor.Services.Cache.Validation;
namespace CreationEditor.Skyrim.Tests.References;

public sealed class CacheValidationResultTests {
    [Fact]
    public void UpdateCache_InvokesActionOnFirstCall() {
        var calls = 0;
        var result = CacheValidationResult<int>.PartlyInvalid([1], () => calls++);

        result.UpdateCache();

        Assert.Equal(1, calls);
    }

    [Fact]
    public void UpdateCache_InvokesActionOnlyOnce() {
        var calls = 0;
        var result = CacheValidationResult<int>.PartlyInvalid([1], () => calls++);

        result.UpdateCache();
        result.UpdateCache();
        result.UpdateCache();

        Assert.Equal(1, calls);
    }

    [Fact]
    public void ValidResult_UpdateCache_DoesNothing() {
        var calls = 0;
        var result = CacheValidationResult<int>.Valid();

        result.UpdateCache();

        Assert.Equal(0, calls);
    }
}