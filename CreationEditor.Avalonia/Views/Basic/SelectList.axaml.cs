using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using CreationEditor.Avalonia.Command;
namespace CreationEditor.Avalonia.Views.Basic;

[TemplatePart(Name = ItemsControlName, Type = typeof(ItemsControl))]
public class SelectList : TemplatedControl {
    private const string ItemsControlName = "PART_ItemsControl";

    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty
        = AvaloniaProperty.Register<SelectList, IEnumerable>(nameof(ItemsSource));

    public IEnumerable ItemsSource {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> DataTemplateProperty
        = AvaloniaProperty.Register<SelectList, IDataTemplate>(nameof(DataTemplate));

    [Content]
    public IDataTemplate DataTemplate {
        get => GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var itemsControl = e.NameScope.Find<ItemsControl>(ItemsControlName);
        if (itemsControl is null) return;

        itemsControl.ContextFlyout ??= new MenuFlyout {
            ItemsSource = (MenuItem[]) [
                new MenuItem {
                    Command = SelectableCommand.SelectAll,
                    CommandParameter = ItemsSource,
                    Header = "Select All",
                },
                new MenuItem {
                    Command = SelectableCommand.UnselectAll,
                    CommandParameter = ItemsSource,
                    Header = "Unselect All",
                },
                new MenuItem {
                    Command = SelectableCommand.ToggleAll,
                    CommandParameter = ItemsSource,
                    Header = "Toggle All",
                },
            ]
        };
    }
}
