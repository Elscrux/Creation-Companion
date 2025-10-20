using Avalonia.Controls;
using DialogueExporter.ViewModels;

namespace DialogueExporter.Views;

public partial class DialogueExporterView : UserControl {
    public DialogueExporterView() {
        InitializeComponent();
    }

    public DialogueExporterView(DialogueExporterVM vm) : this() {
        DataContext = vm;
    }
}
