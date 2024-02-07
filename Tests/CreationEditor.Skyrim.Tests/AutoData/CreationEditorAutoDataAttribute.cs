using AutoFixture;
using AutoFixture.Xunit2;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Testing.AutoData;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class CreationEditorAutoDataAttribute(
    bool configureMembers = false,
    GameRelease release = GameRelease.SkyrimSE,
    bool useMockFileSystem = true)
    : AutoDataAttribute(() => new Fixture().Customize(new CreationEditorCustomization(configureMembers, release, useMockFileSystem)));

public sealed class CreationEditorCustomization(
    bool configureMembers,
    GameRelease release,
    bool useMockFileSystem) : ICustomization {
    public void Customize(IFixture fixture) {
        fixture.Customize(new MutagenDefaultCustomization(configureMembers, release, useMockFileSystem));
        fixture.Customize(new EditorEnvironmentCustomization());
    }
}