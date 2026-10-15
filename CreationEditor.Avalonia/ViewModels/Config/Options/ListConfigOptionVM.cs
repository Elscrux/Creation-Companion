using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using DynamicData.Binding;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

/// <summary>
/// Wraps a single list item that implements <see cref="IConfiguredValues" />: edits the item's
/// config values through a <see cref="ConfigVM" /> built from the options of the item's config
/// definition, writing changes back on every edit.
/// </summary>
public sealed partial class ListItemVM : ViewModel {
    public object Item { get; }
    public ConfigVM ConfigVM { get; }

    /// <summary>Short single-line summary of the item's first set value, shown in the editor header</summary>
    [Reactive] public partial string Summary { get; set; }

    public ListItemVM(object item, IConfigVMFactory configVMFactory) {
        Item = item;
        Summary = item.GetType().Name;

        var configured = (IConfiguredValues)item;

        ConfigVM = configVMFactory.Create(configured.ConfigDefinition, configured.ConfigValues);
        ConfigVM.ConfigChanged
            .Subscribe(_ => WriteToItem())
            .DisposeWith(this);

        WriteToItem();
    }

    private void WriteToItem() {
        var configured = (IConfiguredValues)Item;
        configured.ConfigValues = ConfigVM.GetConfigValues();

        UpdateSummary(configured.ConfigValues);
    }

    private void UpdateSummary(ConfigValues configValues) {
        var summary = ((IConfiguredValues)Item).ConfigDefinition.Options
            .Select(option => configValues.TryGetValue(option.Name, out var value) ? value?.ToString() : null)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        Summary = summary ?? Item.GetType().Name;
    }
}

/// <summary>
/// Config option VM for an ordered list of items (<see cref="IListConfigOption" />). Each item is
/// a <see cref="IConfiguredValues" /> pairing a config definition with its configured values and
/// is edited through the definition's options.
/// </summary>
public sealed partial class ListConfigOptionVM : ViewModel, IConfigOptionVM {
    private readonly IConfigVMFactory _configVMFactory;
    private readonly Subject<object?> _valueChanged = new();
    private readonly Dictionary<ListItemVM, IDisposable> _itemSubscriptions = [];

    public IConfigOption Option => ListOption;
    public IListConfigOption ListOption { get; }

    [Reactive] public partial bool IsActive { get; set; }

    public ObservableCollectionExtended<ListItemVM> Items { get; } = [];

    public IObservable<object?> ValueChanged => _valueChanged;

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => Observable.Return<ConfigOptionValidationError?>(null);

    public object? Value {
        get => ListOption.CreateItemList(Items.Select(item => item.Item));
        set {
            ClearItems();
            if (value is not IEnumerable<object> items) return;

            foreach (var item in items) {
                AddItem(item);
            }
        }
    }

    public ListConfigOptionVM(IListConfigOption option, IConfigVMFactory configVMFactory) {
        ListOption = option;
        _configVMFactory = configVMFactory;
        IsActive = !option.IsOptional;

        Observable
            .FromEventPattern<NotifyCollectionChangedEventArgs>(Items, nameof(Items.CollectionChanged))
            .Subscribe(_ => _valueChanged.OnNext(Value))
            .DisposeWith(this);
    }

    [ReactiveCommand]
    public void AddItem() => AddItem(System.Activator.CreateInstance(ListOption.ItemType)!);

    [ReactiveCommand]
    public void RemoveItem(ListItemVM item) {
        if (_itemSubscriptions.Remove(item, out var subscription)) {
            subscription.Dispose();
        }
        item.Dispose();
        Items.Remove(item);
    }

    private void ClearItems() {
        foreach (var item in Items.ToArray()) {
            RemoveItem(item);
        }
    }

    private void AddItem(object item) {
        var itemVM = new ListItemVM(item, _configVMFactory);
        _itemSubscriptions[itemVM] = Observable
            .FromEventPattern<PropertyChangedEventArgs>(itemVM, nameof(itemVM.PropertyChanged))
            .Subscribe(_ => _valueChanged.OnNext(Value));
        Items.Add(itemVM);
    }
}
