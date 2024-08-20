using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
namespace CreationEditor.Avalonia.Behavior;

public sealed class ListBoxAutoScrollToBottom : Behavior<ListBox>, IDisposable {
    public static readonly StyledProperty<bool> ScrollingEnabledProperty =
        AvaloniaProperty.Register<ListBoxAutoScrollToBottom, bool>(nameof(ScrollingEnabled));

    public bool ScrollingEnabled {
        get => GetValue(ScrollingEnabledProperty);
        set => SetValue(ScrollingEnabledProperty, value);
    }

    private IDisposable? _attachedDisposable;

    protected override void OnAttachedToVisualTree() {
        base.OnAttachedToVisualTree();

        _attachedDisposable?.Dispose();
        _attachedDisposable = AssociatedObject?.WhenAnyValue(x => x.ItemCount)
            .ThrottleMedium()
            .Subscribe(_ => {
                if (!ScrollingEnabled) return;

                switch (AssociatedObject.Scroll) {
                    case ScrollViewer scrollViewer:
                        scrollViewer.ScrollToEnd();
                        break;
                }
            });
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();

        _attachedDisposable?.Dispose();
    }

    public void Dispose() => _attachedDisposable?.Dispose();
}
