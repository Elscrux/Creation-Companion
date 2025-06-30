using Mutagen.Bethesda.Plugins;
using Noggog;
using static ProjectBSE.Interop.Interop;
namespace ProjectBSE.Interop.Test;

//Must be run in debug mode to include dll
public sealed class InteropTest {
    private static readonly InitConfig InitConfig = new() {
        AssetDirectory = "test",
    };
    private static readonly ReferenceLoad TestReference = new() {
        FormKey = FormKey.Factory("123456:Base.esm"),
        Path = "test.nif",
        Transform = new ReferenceTransform {
            Translation = new P3Float(1, 2, 3),
            Scale = new P3Float(2, 3, 3),
            Rotations = new P3Float(),
        },
    };

    public InteropTest() {
        Task.Run(() => InitTGEditor(InitConfig, []));
        WaitFinishedInit();
    }

    [Fact]
    public void TestKeybindings() {
        UpdateKeybindings(Enum.GetValues<IOFunction>()
            .ToDictionary(x => x,
                _ => new IOFunctionBinding {
                    Type = IOFunctionBindingType.Keyboard,
                    Key = -1,
                })
        );
        // var strings = EnumerateKeyBindingNames();
        // Assert.NotNull(strings);
        // Assert.NotEmpty(strings);
        var keyBindings = GetKeyBindings();
        Assert.NotNull(keyBindings);
        Assert.NotEmpty(keyBindings);
    }

    [Fact]
    public void TestSize() {
        var sizeInformation = GetSizeInfo();

        Assert.Equal(sizeInformation.SizeInformationStruct, SizeInformationSize);
        Assert.Equal(sizeInformation.InitConfigStruct, InitConfigSize);
        Assert.Equal(sizeInformation.ReferenceTransformStruct, ReferenceTransformSize);
        Assert.Equal(sizeInformation.ReferenceLoadStruct, ReferenceLoadSize);
        Assert.Equal(sizeInformation.ReferenceUpdateStruct, ReferenceUpdateSize);
        Assert.Equal(sizeInformation.TextureSetStruct, TextureSetSize);
        Assert.Equal(sizeInformation.AlphaDataStruct, AlphaDataSize);
        Assert.Equal(sizeInformation.AlphaLayerStruct, AlphaLayerSize);
        Assert.Equal(sizeInformation.QuadrantStruct, QuadrantSize);
        Assert.Equal(sizeInformation.CornerSetsStruct, CornerSetsSize);
        Assert.Equal(sizeInformation.TerrainInfoStruct, TerrainInfoSize);
        Assert.Equal(sizeInformation.FeatureSetStruct, FeatureSetSize);
        Assert.Equal(sizeInformation.KeyBindingsStruct, KeyBindingsSize);
    }

    [Fact]
    public void TestLoadRefs() {
        ReferenceLoad[] referenceLoads = [TestReference];

        var callbackTriggered = false;
        AddLoadCallback(refs => {
            Assert.Equal(referenceLoads.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i].FormKey, referenceLoads[i].FormKey);
            }
            callbackTriggered = true;
        });

        WaitFinishedInit();

        LoadReferences(referenceLoads);

        Assert.True(callbackTriggered);
    }

    [Fact]
    public void TestDeleteRefs() {
        FormKey[] formKeys = [FormKey.Factory("123456:Skyrim.esm")];

        var callbackTriggered = false;
        AddDeleteCallback(refs => {
            Assert.Equal(formKeys.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i], formKeys[i]);
            }
            callbackTriggered = true;
        });

        WaitFinishedInit();

        DeleteReferences(formKeys);

        Assert.True(callbackTriggered);
    }

    // Update, Hide, and Show currently don't work because they are not implemented on the C++ side,
    // and it stops calling anything after the C++ handler function returns false.

    [Fact]
    public void TestUpdateRefs() {
        ReferenceUpdate[] updates = [
            new() {
                FormKey = FormKey.Factory("123456:Skyrim.esm"),
                Transform = new ReferenceTransform(),
                Update = UpdateType.Transform
            },
            new() {
                FormKey = FormKey.Factory("123456:Skyrim.esm"),
                Path = "test.nif",
                Update = UpdateType.Path
            },
        ];

        var callbackTriggered = false;
        AddUpdateCallback(refs => {
            Assert.Equal(updates.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i].FormKey, updates[i].FormKey);
                Assert.Equal(refs[i].Path, updates[i].Path);
                Assert.Equal(refs[i].Transform, updates[i].Transform);
                Assert.Equal(refs[i].Update, updates[i].Update);
            }
            callbackTriggered = true;
        });

        WaitFinishedInit();

        UpdateReferences(updates);

        Assert.True(callbackTriggered);
    }

    [Fact]
    public void TestHideRefs() {
        FormKey[] formKeys = [FormKey.Factory("123456:Skyrim.esm")];

        var callbackTriggered = false;
        AddHideCallback(refs => {
            Assert.Equal(formKeys.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i], formKeys[i]);
            }
            callbackTriggered = true;
        });

        WaitFinishedInit();

        HideReferences(formKeys);

        Assert.True(callbackTriggered);
    }

    [Fact]
    public void TestShowRefs() {
        FormKey[] formKeys = [FormKey.Factory("123456:Skyrim.esm")];
    
        var callbackTriggered = false;
        AddShowCallback(refs => {
            Assert.Equal(formKeys.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i], formKeys[i]);
            }
            callbackTriggered = true;
        });
    
        WaitFinishedInit();
    
        ShowReferences(formKeys);
    
        Assert.True(callbackTriggered);
    }
}
