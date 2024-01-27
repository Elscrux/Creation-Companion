using Avalonia.Media;
namespace CreationEditor.Avalonia.Services.Avalonia;

public interface IApplicationIconProvider {
    IImage Icon { get; }
    IImage SpinningIcon { get; }
}
