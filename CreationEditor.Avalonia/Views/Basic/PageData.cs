using System.Reactive;
using Avalonia;
using Avalonia.Controls.Templates;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Basic;

public sealed class PageData : AvaloniaObject {
    public static readonly StyledProperty<IDataTemplate> DataTemplateProperty
        = AvaloniaProperty.Register<PageData, IDataTemplate>(nameof(DataTemplate));

    public IDataTemplate DataTemplate {
        get => GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>?> OnNextProperty
        = AvaloniaProperty.Register<PageData, ReactiveCommand<Unit, Unit>?>(nameof(OnNext));

    public ReactiveCommand<Unit, Unit>? OnNext {
        get => GetValue(OnNextProperty);
        set => SetValue(OnNextProperty, value);
    }

    public static readonly StyledProperty<string?> NextButtonTextProperty
        = AvaloniaProperty.Register<PageData, string?>(nameof(NextButtonText), "Next");

    public string? NextButtonText {
        get => GetValue(NextButtonTextProperty);
        set => SetValue(NextButtonTextProperty, value);
    }
}
