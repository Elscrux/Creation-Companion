using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.Subrecord;

public partial class ConditionValue : ActivatableUserControl {
    private static readonly IList<ICustomConditionValueDataTemplate> ConditionValueTemplates
        = typeof(ICustomConditionValueDataTemplate)
            .GetAllSubClasses<ICustomConditionValueDataTemplate>()
            .ToArray();

    public static readonly StyledProperty<EditableCondition> ConditionProperty
        = AvaloniaProperty.Register<ConditionValue, EditableCondition>(nameof(Condition));

    public EditableCondition Condition {
        get => GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<ConditionValue, ILinkCache?>(nameof(LinkCache));
    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<IObservable<Unit>?> ConditionDataChangedProperty
        = AvaloniaProperty.Register<ConditionValue, IObservable<Unit>?>(nameof(ConditionDataChanged));

    public IObservable<Unit>? ConditionDataChanged {
        get => GetValue(ConditionDataChangedProperty);
        set => SetValue(ConditionDataChangedProperty, value);
    }

    private IDisposable? _valueSubscription;

    public ConditionValue() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        this.WhenAnyValue(
                x => x.ConditionDataChanged,
                x => x.LinkCache,
                x => x.Condition,
                x => x.Condition.Data,
                (conditionDataChanged, linkCache, condition, data)
                    => (ConditionDataChanged: conditionDataChanged, LinkCache: linkCache, Condition: condition, Data: data))
            .Subscribe(x => {
                var template = ConditionValueTemplates.FirstOrDefault(c => c.Match(x.Condition.Function));
                if (template is null || x.LinkCache is null) {
                    Content = new NumericUpDown {
                        DataContext = x.Condition,
                        [!NumericUpDown.ValueProperty] = new Binding(nameof(EditableCondition.FloatValue)),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        ShowButtonSpinner = false,
                        FormatString = "N4",
                    };
                } else {
                    _valueSubscription?.Dispose();
                    _valueSubscription = template.Build(x.LinkCache, x.Condition, x.Data, x.ConditionDataChanged)
                        .ObserveOnGui()
                        .BindTo(this, conditionValue => conditionValue.Content)
                        .DisposeWith(ActivatedDisposable);
                }
            })
            .DisposeWith(ActivatedDisposable);
    }
}
