using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Dialog;
namespace CreationEditor.Avalonia.Views.Dialog;

public partial class SaveDialog : ReactiveUserControl<ISaveDialogVM> {
    public static readonly StyledProperty<bool> CancelEnabledFromStartProperty
        = AvaloniaProperty.Register<SaveDialog, bool>(nameof(CancelEnabledFromStart));

    public bool CancelEnabledFromStart {
        get => GetValue(CancelEnabledFromStartProperty);
        set => SetValue(CancelEnabledFromStartProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<SaveDialog, string>(nameof(Title));

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<Control> DialogProperty
        = AvaloniaProperty.Register<SaveDialog, Control>(nameof(Dialog));

    public Control Dialog {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    public SaveDialog() {
        InitializeComponent();
    }

    public SaveDialog(ISaveDialogVM vm) : this() {
        DataContext = vm;
    }

    public void Show() => IsVisible = true;
    public void Hide() {
        IsVisible = false;
        CancelButton.IsEnabled = true;
    }

    public async Task SaveAndHide() {
        if (ViewModel is not null && await ViewModel.Save()) {
            Hide();
        }
    }
}
