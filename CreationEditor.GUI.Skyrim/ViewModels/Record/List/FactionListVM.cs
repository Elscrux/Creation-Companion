using System.Windows;
using CreationEditor.Environment;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.GUI.Views.Record;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
using Syncfusion.UI.Xaml.Grid;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class FactionListVM : RecordListVM<Faction, IFactionGetter> {
    public FactionListVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordEditorController recordEditorControllerController,
        IRecordController recordController)
        : base(referenceQuery, recordBrowserSettings, recordEditorControllerController, recordController) {
        View = new FactionList(this);
    }
}

public class FactionList : RecordList {
    public FactionList(RecordListVM recordListVM) : base(recordListVM) {
        InitializeComponent();
        
        RecordGrid.Columns.Add(new GridTemplateColumn {
            HeaderText = "Name",
            MappingName = "Record.Name.String",
            AllowFiltering = true,
            AllowSorting = true
        });
    }
}