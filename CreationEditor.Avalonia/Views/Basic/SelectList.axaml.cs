using System.Collections;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
namespace CreationEditor.Avalonia.Views.Basic; 

public class SelectList : TemplatedControl {
    public static readonly StyledProperty<IEnumerable> ItemsProperty
        = AvaloniaProperty.Register<SelectList, IEnumerable>(nameof(Items));

    public IEnumerable Items {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> DataTemplateProperty
        = AvaloniaProperty.Register<SelectList, IDataTemplate>(nameof(DataTemplate));

    [Content]
    public IDataTemplate DataTemplate {
        get => GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }
}

