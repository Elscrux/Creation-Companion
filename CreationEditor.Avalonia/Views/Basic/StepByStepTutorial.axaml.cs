using Avalonia;
namespace CreationEditor.Avalonia.Views.Basic;

public class StepByStepTutorial : IconHeaderedControl {
    public static readonly StyledProperty<IList<string>> StepsProperty
        = AvaloniaProperty.Register<StepByStepTutorial, IList<string>>(nameof(Steps));

    public IList<string> Steps {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }
}
