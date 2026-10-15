using CreationEditor.Services.Mutagen.Mod.Save;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Mutagen.Save;

public sealed class SavePipelineTests {
    private sealed class TrackingStep(string name, List<string> executionLog) : ISaveStep {
        public void Execute(ILinkCache linkCache, IMod mod) {
            executionLog.Add(name);
        }
    }

    [Fact]
    public void Execute_RunsStepsInOrder() {
        var log = new List<string>();
        var pipeline = new SavePipeline();
        pipeline.AddStep(new TrackingStep("step1", log));
        pipeline.AddStep(new TrackingStep("step2", log));

        pipeline.Execute(null!, new SkyrimMod(new ModKey("Test", ModType.Plugin), SkyrimRelease.SkyrimSE));

        Assert.Equal(["step1", "step2"], log);
    }

    [Fact]
    public void AddStep_DuplicateStep_IsNotAddedTwice() {
        var log = new List<string>();
        var pipeline = new SavePipeline();
        var step = new TrackingStep("step", log);

        pipeline.AddStep(step);
        pipeline.AddStep(step);

        pipeline.Execute(null!, new SkyrimMod(new ModKey("Test", ModType.Plugin), SkyrimRelease.SkyrimSE));

        Assert.Equal(["step"], log);
    }

    [Fact]
    public void RemoveStep_RemovesStep() {
        var log = new List<string>();
        var pipeline = new SavePipeline();
        var step = new TrackingStep("step", log);
        pipeline.AddStep(step);

        pipeline.RemoveStep(step);

        pipeline.Execute(null!, new SkyrimMod(new ModKey("Test", ModType.Plugin), SkyrimRelease.SkyrimSE));

        Assert.DoesNotContain("step", log);
    }

    [Fact]
    public void DefaultStep_RemovesDeletedModCreatedRecords() {
        var mod = new SkyrimMod(new ModKey("Test", ModType.Plugin), SkyrimRelease.SkyrimSE);
        var armor = new Armor(mod) { EditorID = "NewArmor", IsDeleted = true };
        mod.Armors.Add(armor);

        new SavePipeline().Execute(null!, mod);

        Assert.Empty(mod.Armors);
    }
}
