using Avalonia;
using Avalonia.Interactivity;
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

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        this.GetObservable(DataContextProperty).Subscribe(OnDataContextUpdated);
        this.GetObservable(ViewModelProperty).Subscribe(OnViewModelUpdated);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        // Show mod selection on startup
        ViewModel?.ShowModSelection(false);
    }

    private void OnDataContextUpdated(object? value) => ViewModel = value as MainVM;

    private void OnViewModelUpdated(object? value) {
        if (value is null) {
            ClearValue(DataContextProperty);
        } else if (DataContext != value) {
            DataContext = value;
        }
    }
}
