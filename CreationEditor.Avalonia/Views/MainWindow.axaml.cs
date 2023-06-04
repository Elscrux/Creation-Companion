using Avalonia;
using CreationEditor.Avalonia.ViewModels;
using FluentAvalonia.UI.Windowing;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views;

public partial class MainWindow : AppWindow, IViewFor<MainVM> {
    public static readonly StyledProperty<MainVM?> ViewModelProperty
        = AvaloniaProperty.Register<MainWindow, MainVM?>(nameof(ViewModel));

    public MainVM? ViewModel {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel {
        get => ViewModel;
        set => ViewModel = (MainVM?) value;
    }

    public MainWindow() {
        InitializeComponent();

        this.GetObservable(DataContextProperty).Subscribe(OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(OnViewModelChanged);
    }

    protected override void OnLoaded() {
        base.OnLoaded();

        // Show mod selection on startup
        ViewModel?.ShowModSelection();
    }

    private void OnDataContextChanged(object? value) => ViewModel = value as MainVM;

    private void OnViewModelChanged(object? value) {
        if (value == null) {
            ClearValue(DataContextProperty);
        } else if (DataContext != value) {
            DataContext = value;
        }
    }
}
