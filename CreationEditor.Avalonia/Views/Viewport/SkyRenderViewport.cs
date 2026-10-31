using Avalonia.OpenGL.Controls;
using Avalonia.OpenGL;
using OpenTK.Graphics.OpenGL4;
using SkyRender;

namespace CreationEditor.Avalonia.Views.Viewport;

public sealed class SkyRenderViewport : OpenGlControlBase {
    private GlRenderer? _renderer;
    private readonly OrbitCamera _camera = new();

    protected override void OnOpenGlInit(GlInterface gl) {
        GL.LoadBindings(new AvaloniaBindingsContext(gl));
        _renderer = new GlRenderer();
        _renderer.Initialize(ModuleResourceLocator.ShaderDirectory);
    }
    
    protected override void OnOpenGlRender(GlInterface gl, int fb) {
        if (_renderer is null) return;
        if (_renderer is null) return;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
        _renderer.Render(
            _camera,
            Math.Max(1, (int) Bounds.Width),
            Math.Max(1, (int) Bounds.Height));
        RequestNextFrameRendering();
    }

    protected override void OnOpenGlDeinit(GlInterface gl) {
        if (_renderer is null) return;

        _renderer.ClearMeshes();
        _renderer.Dispose();
        _renderer = null;
    }

    protected override void OnOpenGlLost() {
        _renderer = null;
    }

    private sealed class AvaloniaBindingsContext(GlInterface gl) : OpenTK.IBindingsContext {
        public IntPtr GetProcAddress(string procName) => gl.GetProcAddress(procName);
    }
}
