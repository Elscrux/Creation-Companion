using System.Runtime.InteropServices;
using Noggog;
using Xunit;
using static CreationEditor.Avalonia.Services.Viewport.BSE.Interop;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

//Must be run in debug mode to include dll
public sealed class InteropTest {
    private static readonly InitConfig InitConfig = new() {
        Version = 2,
        AssetDirectory = "test",
        SizeOfWindowHandles = 0,
        WindowHandles = Array.Empty<nint>(),
    };
    private static readonly ReferenceLoad TestReference = new() {
        FormKey = "123456:Skyrim.esm",
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

    private static void TestInit() {
        var task = Xunit.Record.ExceptionAsync(() => Task.Run(() => {
            initTGEditor(
                new InitConfig {
                    Version = 1,
                    AssetDirectory = "test",
                    SizeOfWindowHandles = 0,
                    WindowHandles = Array.Empty<nint>(),
                },
                Array.Empty<string>(),
                0);
        }));

        waitFinishedInit();

        Assert.Null(task.Exception);
    }

    [Fact]
    public void TestSize() {
        var sizeInformation = getSizeInfo();

        Assert.Equal(sizeInformation.SizeInformationStruct, Marshal.SizeOf<SizeInformation>());
        Assert.Equal(sizeInformation.InitConfigStruct, Marshal.SizeOf<InitConfig>());
        Assert.Equal(sizeInformation.ReferenceTransformStruct, Marshal.SizeOf<ReferenceTransform>());
        Assert.Equal(sizeInformation.ReferenceLoadStruct, Marshal.SizeOf<ReferenceLoad>());
        Assert.Equal(sizeInformation.ReferenceUpdateStruct, Marshal.SizeOf<ReferenceUpdate>());
        Assert.Equal(sizeInformation.TextureSetStruct, Marshal.SizeOf<TextureSet>());
        Assert.Equal(sizeInformation.AlphaDataStruct, Marshal.SizeOf<AlphaData>());
        Assert.Equal(sizeInformation.AlphaLayerStruct, Marshal.SizeOf<AlphaLayer>());
        Assert.Equal(sizeInformation.QuadrantStruct, Marshal.SizeOf<Quadrant>());
        Assert.Equal(sizeInformation.CornerSetsStruct, Marshal.SizeOf<CornerSets>());
        Assert.Equal(sizeInformation.TerrainInfoStruct, Marshal.SizeOf<TerrainInfo>());
    }

    [Fact]
    public void TestLoadRefs() {
        var referenceLoads = new[] { TestReference };
        var count = Convert.ToUInt32(referenceLoads.Length);

        Assert.True(addLoadCallback((callbackCount, callbackLoads) => {
            Assert.Equal(count, callbackCount);
            for (var i = 0; i < callbackLoads.Length; i++) {
                Assert.Equal(callbackLoads[i].FormKey, referenceLoads[i].FormKey);
            }
        }));

        waitFinishedInit();

        loadReferences(count, referenceLoads);
    }
}
