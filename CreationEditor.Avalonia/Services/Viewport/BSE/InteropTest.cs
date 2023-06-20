﻿using Noggog;
using Xunit;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

//Must be run in debug mode to include dll
public sealed class InteropTest {
    private readonly Interop.ReferenceLoad _testReference = new() {
        FormKey = "123456:Skyrim.esm",
        Path = "test.nif",
        Transform = new Interop.ReferenceTransform {
            Translation = new P3Float(1, 2, 3),
            Scale = new P3Float(2, 3, 3),
            Rotations = new P3Float()
        }
    };

    [Fact]
    public void TestInit() {
        var task = Xunit.Record.ExceptionAsync(() => Task.Run(() => {
            Interop.initTGEditor(
                new Interop.InitConfig {
                    Version = 1,
                    AssetDirectory = "test",
                    SizeOfWindowHandles = 0,
                    WindowHandles = new IntPtr[] {},
                },
                Array.Empty<string>(),
                0);
        }));

        Interop.waitFinishedInit();

        Assert.Null(task.Exception);
    }

    [Fact]
    public void TestLoadRefs() {
        var referenceLoads = new[] { _testReference };
        var count = Convert.ToUInt32(referenceLoads.Length);

        Assert.True(Interop.addLoadCallback((callbackCount, callbackLoads) => {
            Assert.Equal(count, callbackCount);
            for (var i = 0; i < callbackLoads.Length; i++) {
                Assert.Equal(callbackLoads[i].FormKey, referenceLoads[i].FormKey);
            }
        }));

        Interop.waitFinishedInit();

        Interop.loadReferences(count, referenceLoads);
    }
}
