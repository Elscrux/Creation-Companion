using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Views.Basic;
namespace CreationEditor.Avalonia.Views.Viewport;

public sealed class ViewportHost(IntPtr windowHandle, string descriptor) : WindowHandleHost(windowHandle, descriptor), IViewport;
