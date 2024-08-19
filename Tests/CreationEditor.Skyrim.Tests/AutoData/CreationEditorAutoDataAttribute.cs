using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda;
using Noggog.Testing.AutoFixture;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class CreationEditorAutoDataAttribute(
    bool configureMembers = false,
    GameRelease release = GameRelease.SkyrimSE,
    TargetFileSystem targetFileSystem = TargetFileSystem.Fake)
    : AutoDataAttribute(() => new Fixture().Customize(new CreationEditorCustomization(configureMembers, release, targetFileSystem)));

public sealed class CreationEditorCustomization(
    bool configureMembers,
    GameRelease release,
    TargetFileSystem targetFileSystem) : ICustomization {
    public void Customize(IFixture fixture) {
        fixture.Customize(new EditorEnvironmentCustomization(configureMembers, release, targetFileSystem));
    }
}
