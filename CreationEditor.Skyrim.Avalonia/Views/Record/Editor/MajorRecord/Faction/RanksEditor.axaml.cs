using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Attached;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction; 

public partial class RanksEditor : ReactiveUserControl<RankEditorVM> {
    public RanksEditor() {
        InitializeComponent();

        RankGrid.SetValue(DragDropExtended.CanDropProperty, o => ViewModel != null && ViewModel.CanDrop(o));
    }
}
