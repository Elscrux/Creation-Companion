// using CreationEditor.Services.Mutagen.Record;
// using CreationEditor.Skyrim.Record;
// using CreationEditor.Skyrim.Tests.AutoData;
// using Mutagen.Bethesda.Skyrim;
// namespace CreationEditor.Skyrim.Tests.Test;
//
// public class RecordHistoryTests {
//     [Theory, CreationEditorAutoData]
//     public void Test(
//         IRecordHistory recordHistory,
//         IRecordController recordController) {
//         // Create record with no references
//         var package = recordController.CreateRecord<Package, IPackageGetter>();
//
//         // Change record
//         recordController.RegisterUpdate(package, () => package.EditorID);
//
//         recordHistory.Undo();
//     }
// }
