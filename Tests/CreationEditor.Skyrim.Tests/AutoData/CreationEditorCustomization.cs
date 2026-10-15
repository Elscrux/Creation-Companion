using AutoFixture;
using Mutagen.Bethesda;
using Noggog.Testing.AutoFixture;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class CreationEditorCustomization(
    bool configureMembers,
    GameRelease release,
    TargetFileSystem targetFileSystem) : ICustomization {
    public void Customize(IFixture fixture) {
        fixture.Customize(new EditorEnvironmentCustomization(configureMembers, release, targetFileSystem));
    }
}