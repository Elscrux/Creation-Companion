using CreationEditor.Services.Config;
namespace CreationEditor.Avalonia.ViewModels.Config;

public interface IConfigOptionVM {
    IConfigOption Option { get; }
    bool IsActive { get; set; }
    IObservable<object?> ValueChanged { get; }
    object? Value { get; set; }
    IObservable<ConfigOptionValidationError?> ValidationErrorChanged { get; }
}
