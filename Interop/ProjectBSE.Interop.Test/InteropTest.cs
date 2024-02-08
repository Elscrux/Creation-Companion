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
            Rotations = new P3Float()
        }
    };

    public InteropTest() {
        TestInit();
    }

    private static async void TestInit() {
        var task = await Record.ExceptionAsync(() => Task.Run(() => {
            var initTgEditor = InitTGEditor(InitConfig, []);
            Console.WriteLine(initTgEditor);
            // Assert.NotEqual(-1, initTgEditor);
        }));

        Assert.Null(task);

        WaitFinishedInit();

        Assert.Null(task);
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
    }

    [Fact]
    public void TestLoadRefs() {
        ReferenceLoad[] referenceLoads = [TestReference];

        var loadCallbackTriggered = false;
        AddLoadCallback(refs => {
            Assert.Equal(referenceLoads.Length, refs.Length);
            for (var i = 0; i < refs.Length; i++) {
                Assert.Equal(refs[i].FormKey, referenceLoads[i].FormKey);
            }
            loadCallbackTriggered = true;
        });

        WaitFinishedInit();

        LoadReferences(referenceLoads);

        Assert.True(loadCallbackTriggered);
    }
}
