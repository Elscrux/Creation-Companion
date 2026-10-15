using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
namespace CreationEditor.Skyrim.Tests.Config;

public sealed class ConfigValuesExtensionsTests {
    private readonly TextConfigOption _textOption = new("Text", defaultValue: "default");
    private readonly TextConfigOption _optionalTextOption = new("OptionalText", isOptional: true);
    private readonly LongConfigOption _longOption = new("Long", defaultValue: 42);

    [Fact]
    public void Get_SetValue_ReturnsValue() {
        var configValues = new Dictionary<string, object> { [_textOption.Name] = "custom" };

        Assert.Equal("custom", configValues.Get(_textOption));
    }

    [Fact]
    public void Get_UnsetNonOptional_ReturnsDefault() {
        var configValues = new Dictionary<string, object>();

        Assert.Equal("default", configValues.Get(_textOption));
    }

    [Fact]
    public void Get_UnsetOptional_Throws() {
        var configValues = new Dictionary<string, object>();

        Assert.Throws<ArgumentException>(() => configValues.Get(_optionalTextOption));
    }

    [Fact]
    public void Get_Typed_ReturnsConvertedValue() {
        var configValues = new Dictionary<string, object> { [_textOption.Name] = "123" };

        Assert.Equal(123, configValues.Get<int>(_textOption));
    }

    [Fact]
    public void Get_TypedEnum_FromBoxedLong() {
        var configValues = new Dictionary<string, object> { [_longOption.Name] = 1L };

        Assert.Equal(DateTimeKind.Utc, configValues.Get<DateTimeKind>(_longOption));
    }

    [Fact]
    public void TryGet_UnsetOptional_ReturnsNull() {
        var configValues = new Dictionary<string, object>();

        Assert.Null(configValues.TryGet(_optionalTextOption));
    }

    [Fact]
    public void TryGet_UnsetNonOptional_ReturnsDefault() {
        var configValues = new Dictionary<string, object>();

        Assert.Equal("default", configValues.TryGet(_textOption));
    }

    [Fact]
    public void TryGetStruct_UnsetNonOptional_ReturnsDefault() {
        var configValues = new Dictionary<string, object>();

        Assert.Equal(42L, configValues.TryGetStruct<long>(_longOption));
    }

    [Fact]
    public void TryGetStruct_UnsetOptional_ReturnsNull() {
        var optionalLongOption = new LongConfigOption("OptionalLong", isOptional: true);
        var configValues = new Dictionary<string, object>();

        Assert.Null(configValues.TryGetStruct<long>(optionalLongOption));
    }

    [Fact]
    public void TryGetStruct_Set_ReturnsValue() {
        var configValues = new Dictionary<string, object> { [_longOption.Name] = 7L };

        Assert.Equal(7L, configValues.TryGetStruct<long>(_longOption));
    }

    [Fact]
    public void Has_ReflectsSetState() {
        var configValues = new Dictionary<string, object>();

        Assert.False(configValues.Has(_textOption));

        configValues.Set(_textOption, "value");

        Assert.True(configValues.Has(_textOption));
    }

    [Fact]
    public void Set_StoresValue() {
        var configValues = new Dictionary<string, object>();

        configValues.Set(_longOption, 5L);

        Assert.Equal(5L, configValues.Get(_longOption));
    }
}
