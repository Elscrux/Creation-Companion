using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CreationEditor.WPF.Skyrim.ViewModels.Record;
namespace CreationEditor.WPF.Skyrim.Views.Record;

public partial class FactionEditor : ReactiveUserControl<FactionEditorVM> {
    public FactionEditor() {
        InitializeComponent();
    }
    public FactionEditor(FactionEditorVM vm) {
        InitializeComponent();

        DataContext = vm;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
