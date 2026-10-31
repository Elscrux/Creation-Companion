using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;

namespace CreationEditor.Avalonia.Services.Viewport.SkyRender;

public sealed class SkyRenderViewportFactory : IViewportFactory {
    public bool IsMultiInstanceCapable => false;
    public IObservable<Unit> ViewportInitialized => Observable.Never<Unit>();

    public Task<Func<Control>> CreateViewport() {
        return Task.FromResult<Func<Control>>(() => {
            return new SkyRenderViewport();
        });
    }
}
