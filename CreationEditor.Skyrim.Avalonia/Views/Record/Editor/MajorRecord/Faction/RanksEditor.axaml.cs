using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Attached.DragDrop;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction;

public partial class RanksEditor : ReactiveUserControl<RankEditorVM> {
    public RanksEditor() {
        InitializeComponent();

        RankGrid.SetValue(DragDropExtended.CanDropProperty, o => ViewModel is not null && RankEditorVM.CanDrop(o));
    }
}
