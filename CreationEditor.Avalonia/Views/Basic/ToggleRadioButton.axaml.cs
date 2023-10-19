using Avalonia.Controls;
namespace CreationEditor.Avalonia.Views.Basic;

public class ToggleRadioButton : RadioButton {
    protected override void OnClick() {
        if (IsChecked == true) {
            IsChecked = false;
        } else {
            base.OnClick();
        }
    }
}
