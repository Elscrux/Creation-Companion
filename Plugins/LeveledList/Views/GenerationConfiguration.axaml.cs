using Avalonia.ReactiveUI;
using LeveledList.ViewModels;
namespace LeveledList.Views;

public partial class WorkflowConfiguration : ReactiveUserControl<GenerationConfigurationVM> {
    public WorkflowConfiguration() {
        InitializeComponent();
    }
}
