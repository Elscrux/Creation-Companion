using System.Collections.ObjectModel;
using System.Linq;
using CreationEditor.GUI.Skyrim.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using Syncfusion.UI.Xaml.Grid;
namespace CreationEditor.GUI.Skyrim.Views.Record;

public partial class FactionEditor {
    public FactionEditor(FactionEditorVM vm) {
        InitializeComponent();

        DataContext = vm;
        RankGrid.RowDragDropController ??= new GridRowDragDropController();
        RankGrid.RowDragDropController.Dropped += (_, args) => {
            if (DataContext is not FactionEditorVM factionEditorVM) return;
            if (args.DropPosition == DropPosition.None) return;
            if (args.Data.GetData("Records") is not ObservableCollection<object> draggingRecords) return;

            // Gets the TargetRecord from the underlying collection using record index of the TargetRecord (e.TargetRecord)
            var targetRecord = factionEditorVM.EditableRecord.Ranks[(int) args.TargetRecord];

            // Use Batch update to avoid data operatons in SfDataGrid during records removing and inserting
            RankGrid.BeginInit();


            // Removes the dragging records from the underlying collection
            foreach (var item in draggingRecords.Cast<Rank>()) {
                factionEditorVM.EditableRecord.Ranks.Remove(item);
            }

            // Find the target record index after removing the records
            var targetIndex = factionEditorVM.EditableRecord.Ranks.IndexOf(targetRecord);
            var insertionIndex = args.DropPosition == DropPosition.DropAbove ? targetIndex : targetIndex + 1;
            insertionIndex = insertionIndex < 0 ? 0 : insertionIndex;

            // Insert dragging records to the target position
            for (var i = draggingRecords.Count - 1; i >= 0; i--) {
                factionEditorVM.EditableRecord.Ranks.Insert(insertionIndex, draggingRecords[i] as Rank);
            }
            RankGrid.EndInit();
        };
    }
}
