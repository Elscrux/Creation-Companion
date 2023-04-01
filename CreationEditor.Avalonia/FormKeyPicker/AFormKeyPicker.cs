using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.Views;
using DynamicData;
using DynamicData.Binding;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.FormKeyPicker;

public record RecordNamePair(IMajorRecordIdentifier Record, string? Name);

[TemplatePart(Name = "PART_EditorIDBox", Type = typeof(TextBox))]
[TemplatePart(Name = "PART_FormKeyBox", Type = typeof(TextBox))]
public class AFormKeyPicker : DisposableTemplatedControl {
    private enum UpdatingEnum { None, FormKey, EditorID, FormStr }
    private UpdatingEnum _updating;

    public enum FormKeyPickerSearchMode {
        None,
        EditorID,
        FormKey,
    }

    public readonly IDisposableDropoff LoadedDisposable = new DisposableBucket();

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }
    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty = AvaloniaProperty.Register<AFormKeyPicker, ILinkCache?>(nameof(LinkCache));

    public IEnumerable? ScopedTypes {
        get => GetValue(ScopedTypesProperty);
        set => SetValue(ScopedTypesProperty, value);
    }
    public static readonly StyledProperty<IEnumerable?> ScopedTypesProperty = AvaloniaProperty.Register<AFormKeyPicker, IEnumerable?>(nameof(ScopedTypes));

    /// <summary>
    /// todo too many scoped records break the observable filter flow
    /// </summary>
    public IList<IMajorRecordGetter>? ScopedRecords {
        get => GetValue(ScopedRecordsProperty);
        set => SetValue(ScopedRecordsProperty, value);
    }
    public static readonly StyledProperty<IList<IMajorRecordGetter>?> ScopedRecordsProperty = AvaloniaProperty.Register<AFormKeyPicker, IList<IMajorRecordGetter>?>(nameof(ScopedRecords));

    public ReadOnlyObservableCollection<TypeItem> SelectableTypes {
        get => GetValue(SelectableTypesProperty);
        set => SetValue(SelectableTypesProperty, value);
    }
    public static readonly StyledProperty<ReadOnlyObservableCollection<TypeItem>> SelectableTypesProperty = AvaloniaProperty.Register<AFormKeyPicker, ReadOnlyObservableCollection<TypeItem>>(nameof(SelectableTypes));

    public IObservable<bool> AnyTypeSelected {
        get => GetValue(AnyTypeSelectedProperty);
        set => SetValue(AnyTypeSelectedProperty, value);
    }
    public static readonly StyledProperty<IObservable<bool>> AnyTypeSelectedProperty = AvaloniaProperty.Register<AFormKeyPicker, IObservable<bool>>(nameof(AnyTypeSelected));

    public ReadOnlyObservableCollection<ModItem> SelectableMods {
        get => GetValue(SelectableModsProperty);
        set => SetValue(SelectableModsProperty, value);
    }
    public static readonly StyledProperty<ReadOnlyObservableCollection<ModItem>> SelectableModsProperty = AvaloniaProperty.Register<AFormKeyPicker, ReadOnlyObservableCollection<ModItem>>(nameof(SelectableMods));

    public IObservable<bool> AnyModSelected {
        get => GetValue(AnyModSelectedProperty);
        set => SetValue(AnyModSelectedProperty, value);
    }
    public static readonly StyledProperty<IObservable<bool>> AnyModSelectedProperty = AvaloniaProperty.Register<AFormKeyPicker, IObservable<bool>>(nameof(AnyModSelected));

    public ICollection<FormKey>? BlacklistFormKeys {
        get => GetValue(BlacklistFormKeysProperty);
        set => SetValue(BlacklistFormKeysProperty, value);
    }
    public static readonly StyledProperty<ICollection<FormKey>?> BlacklistFormKeysProperty = AvaloniaProperty.Register<AFormKeyPicker, ICollection<FormKey>?>(nameof(BlacklistFormKeys));

    public Func<FormKey, string?, bool>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }
    public static readonly StyledProperty<Func<FormKey, string?, bool>?> FilterProperty = AvaloniaProperty.Register<AFormKeyPicker, Func<FormKey, string?, bool>?>(nameof(Filter));

    public bool Found {
        get => GetValue(FoundProperty);
        set => SetValue(FoundProperty, value);
    }
    public static readonly StyledProperty<bool> FoundProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(Found), defaultBindingMode: BindingMode.TwoWay);

    public bool Processing {
        get => GetValue(ProcessingProperty);
        set => SetValue(ProcessingProperty, value);
    }
    public static readonly StyledProperty<bool> ProcessingProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(Processing));

    public FormKey FormKey {
        get => GetValue(FormKeyProperty);
        set => SetValue(FormKeyProperty, value);
    }
    public static readonly StyledProperty<FormKey> FormKeyProperty = AvaloniaProperty.Register<AFormKeyPicker, FormKey>(nameof(FormKey), defaultBindingMode: BindingMode.TwoWay);

    public string FormKeyStr {
        get => GetValue(FormKeyStrProperty);
        set => SetValue(FormKeyStrProperty, value);
    }
    public static readonly StyledProperty<string> FormKeyStrProperty = AvaloniaProperty.Register<AFormKeyPicker, string>(nameof(FormKeyStr), string.Empty, defaultBindingMode: BindingMode.TwoWay);

    public string EditorID {
        get => GetValue(EditorIDProperty);
        set => SetValue(EditorIDProperty, value);
    }
    public static readonly StyledProperty<string> EditorIDProperty = AvaloniaProperty.Register<AFormKeyPicker, string>(nameof(EditorID), string.Empty, defaultBindingMode: BindingMode.TwoWay);

    public bool MissingMeansError {
        get => GetValue(MissingMeansErrorProperty);
        set => SetValue(MissingMeansErrorProperty, value);
    }
    public static readonly StyledProperty<bool> MissingMeansErrorProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(MissingMeansError), true);

    public bool MissingMeansNull {
        get => GetValue(MissingMeansNullProperty);
        set => SetValue(MissingMeansNullProperty, value);
    }
    public static readonly StyledProperty<bool> MissingMeansNullProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(MissingMeansNull));

    private StatusIndicatorState _status;
    public StatusIndicatorState Status {
        get => _status;
        protected set => SetAndRaise(StatusProperty, ref _status, value);
    }
    public static readonly DirectProperty<AFormKeyPicker, StatusIndicatorState> StatusProperty = AvaloniaProperty.RegisterDirect<AFormKeyPicker, StatusIndicatorState>(nameof(StatusProperty), picker => picker.Status);

    private string _statusString = string.Empty;
    public string StatusString {
        get => _statusString;
        protected set => SetAndRaise(StatusStringProperty, ref _statusString, value);
    }
    public static readonly DirectProperty<AFormKeyPicker, string> StatusStringProperty = AvaloniaProperty.RegisterDirect<AFormKeyPicker, string>(nameof(StatusString), picker => picker.StatusString);

    public ICommand PickerClickCommand {
        get => GetValue(PickerClickCommandProperty);
        set => SetValue(PickerClickCommandProperty, value);
    }
    public static readonly StyledProperty<ICommand> PickerClickCommandProperty = AvaloniaProperty.Register<AFormKeyPicker, ICommand>(nameof(PickerClickCommand), defaultBindingMode: BindingMode.TwoWay);

    private readonly Subject<FormKey> _formKeyChanged = new();
    public IObservable<FormKey> FormKeyChanged {
        get => GetValue(FormKeyChangedProperty);
        set => SetValue(FormKeyChangedProperty, value);
    }
    public static readonly StyledProperty<IObservable<FormKey>> FormKeyChangedProperty = AvaloniaProperty.Register<AFormKeyPicker, IObservable<FormKey>>(nameof(FormKeyChanged), defaultBindingMode: BindingMode.OneWayToSource);

    public bool InSearchMode {
        get => GetValue(InSearchModeProperty);
        set => SetValue(InSearchModeProperty, value);
    }
    public static readonly StyledProperty<bool> InSearchModeProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(InSearchMode));

    private FormKeyPickerSearchMode _searchMode = FormKeyPickerSearchMode.None;
    public FormKeyPickerSearchMode SearchMode {
        get => _searchMode;
        protected set {
            SetAndRaise(SearchModeProperty, ref _searchMode, value);
            InSearchMode = _searchMode switch {
                FormKeyPickerSearchMode.None => false,
                _ => true
            };
        }
    }
    public static readonly DirectProperty<AFormKeyPicker, FormKeyPickerSearchMode> SearchModeProperty = AvaloniaProperty.RegisterDirect<AFormKeyPicker, FormKeyPickerSearchMode>(nameof(SearchMode), picker => picker.SearchMode);

    #region Theming
    public Brush ProcessingSpinnerForeground {
        get => GetValue(ProcessingSpinnerForegroundProperty);
        set => SetValue(ProcessingSpinnerForegroundProperty, value);
    }
    public static readonly StyledProperty<Brush> ProcessingSpinnerForegroundProperty = AvaloniaProperty.Register<AFormKeyPicker, Brush>(nameof(ProcessingSpinnerForeground), new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)));

    public Color ProcessingSpinnerGlow {
        get => GetValue(ProcessingSpinnerGlowProperty);
        set => SetValue(ProcessingSpinnerGlowProperty, value);
    }
    public static readonly StyledProperty<Color> ProcessingSpinnerGlowProperty = AvaloniaProperty.Register<AFormKeyPicker, Color>(nameof(ProcessingSpinnerGlow), Color.FromArgb(255, 0, 255, 255));

    public ISolidColorBrush ErrorBrush {
        get => GetValue(ErrorBrushProperty);
        set => SetValue(ErrorBrushProperty, value);
    }
    public static readonly StyledProperty<ISolidColorBrush> ErrorBrushProperty = AvaloniaProperty.Register<AFormKeyPicker, ISolidColorBrush>(nameof(ErrorBrush), Brushes.IndianRed);

    public ISolidColorBrush SuccessBrush {
        get => GetValue(SuccessBrushProperty);
        set => SetValue(SuccessBrushProperty, value);
    }
    public static readonly StyledProperty<ISolidColorBrush> SuccessBrushProperty = AvaloniaProperty.Register<AFormKeyPicker, ISolidColorBrush>(nameof(SuccessBrush), Brushes.ForestGreen);

    public ISolidColorBrush PassiveBrush {
        get => GetValue(PassiveBrushProperty);
        set => SetValue(PassiveBrushProperty, value);
    }
    public static readonly StyledProperty<ISolidColorBrush> PassiveBrushProperty = AvaloniaProperty.Register<AFormKeyPicker, ISolidColorBrush>(nameof(PassiveBrush), Brushes.Orange);

    public bool AllowsSearchMode {
        get => GetValue(AllowsSearchModeProperty);
        set => SetValue(AllowsSearchModeProperty, value);
    }
    public static readonly StyledProperty<bool> AllowsSearchModeProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(AllowsSearchMode), true, defaultBindingMode: BindingMode.TwoWay);

    private IEnumerable? _applicableEditorIDs;
    public IEnumerable? ApplicableEditorIDs {
        get => _applicableEditorIDs;
        protected set => SetAndRaise(ApplicableEditorIDsProperty, ref _applicableEditorIDs, value);
    }
    public static readonly DirectProperty<AFormKeyPicker, IEnumerable?> ApplicableEditorIDsProperty = AvaloniaProperty.RegisterDirect<AFormKeyPicker, IEnumerable?>(nameof(ApplicableEditorIDs), picker => picker.ApplicableEditorIDs);

    public Func<IMajorRecordIdentifier, ILinkCache?, string?> NameSelector {
        get => GetValue(NameSelectorProperty);
        set => SetValue(NameSelectorProperty, value);
    }
    public static readonly StyledProperty<Func<IMajorRecordIdentifier, ILinkCache?, string?>> NameSelectorProperty = AvaloniaProperty.Register<AFormKeyPicker, Func<IMajorRecordIdentifier, ILinkCache?, string?>>(nameof(NameSelector), (record, _) => record.EditorID);

    public bool ViewingAllowedTypes {
        get => GetValue(ViewingAllowedTypesProperty);
        set => SetValue(ViewingAllowedTypesProperty, value);
    }
    public static readonly StyledProperty<bool> ViewingAllowedTypesProperty = AvaloniaProperty.Register<AFormKeyPicker, bool>(nameof(ViewingAllowedTypes));

    public ICommand ToggleViewAllowedTypesCommand {
        get => GetValue(ViewAllowedTypesCommandProperty);
        set => SetValue(ViewAllowedTypesCommandProperty, value);
    }
    public static readonly StyledProperty<ICommand> ViewAllowedTypesCommandProperty = AvaloniaProperty.Register<AFormKeyPicker, ICommand>(nameof(ToggleViewAllowedTypesCommand));

    public ICommand Clear {
        get => GetValue(ClearProperty);
        set => SetValue(ClearProperty, value);
    }
    public static readonly StyledProperty<ICommand> ClearProperty = AvaloniaProperty.Register<AFormKeyPicker, ICommand>(nameof(Clear));
    #endregion

    private sealed record State(StatusIndicatorState Status, string Text, FormKey FormKey, string EditorID);

    public AFormKeyPicker() {
        ToggleViewAllowedTypesCommand = ReactiveCommand.Create(() => ViewingAllowedTypes = !ViewingAllowedTypes);

        Clear = ReactiveCommand.Create(() => {
            FormKey = FormKey.Null;
            Status = StatusIndicatorState.Passive;
            Found = false;
        });

        FormKeyChanged = _formKeyChanged;
    }

    private const string LocatedRecord = "Located record";
    private const string LinkCacheMissing = "No LinkCache is provided for lookup";
    private const string FormKeyNull = "FormKey is null. No lookup required";
    private const string FormKeyBlacklisted = "FormKey is blacklisted";
    private const string RecordNotResolved = "Could not resolve record";
    private const string RecordFiltered = "Record filtered out";

    protected override void OnLoaded() {
        base.OnLoaded();

        SelectableTypes = this.WhenAnyValue(x => x.ScopedTypes)
            .Select(x => GetMajorTypes(x).AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(x => new TypeItem(x), UnloadDisposable);

        var selectedTypesChanged = this.WhenAnyValue(x => x.SelectableTypes)
            .CombineLatest(SelectableTypes.SelectionChanged().StartWith(Unit.Default), (types, _) => types);

        AnyTypeSelected = selectedTypesChanged
            .Select(x => x.Any(typeItem => typeItem.IsSelected));

        SelectableMods = this.WhenAnyValue(x => x.LinkCache)
            .NotNull()
            .Select(linkCache => linkCache.ListedOrder.AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(x => new ModItem(x.ModKey) { IsSelected = true }, UnloadDisposable);

        var selectedModsChanged = this.WhenAnyValue(x => x.SelectableMods)
            .CombineLatest(SelectableMods.SelectionChanged(), (mods, _) => mods);

        AnyModSelected = selectedModsChanged
            .Select(x => x.Any(modItem => modItem.IsSelected));

        var scopedRecordsCollection = this.WhenAnyValue(x => x.ScopedRecords)
            .Select(idents => (idents ?? Array.Empty<IMajorRecordGetter>()).AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(UnloadDisposable);

        var scopedRecordsChanged = this.WhenAnyValue(x => x.ScopedRecords)
            .CombineLatest(scopedRecordsCollection.ObserveCollectionChanges().Unit().StartWith(Unit.Default), (idents, _) => idents);

        this.WhenAnyValue(x => x.FormKey)
            .DistinctUntilChanged()
            .Do(formKey => _formKeyChanged.OnNext(formKey))
            .CombineLatest(
                this.WhenAnyValue(x => x.LinkCache),
                selectedTypesChanged,
                scopedRecordsChanged,
                (form, linkCache, types, scopedRecords)
                    => (FormKey: form,
                        LinkCache: linkCache,
                        Types: types,
                        ScopedRecords: scopedRecords))
            .Where(_ => _updating is UpdatingEnum.None or UpdatingEnum.FormKey)
            .Do(_ => {
                _updating = UpdatingEnum.FormKey;
                if (!Processing) {
                    Processing = true;
                }
            })
            .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                try {
                    if (x.LinkCache == null) {
                        return new State(StatusIndicatorState.Passive, LinkCacheMissing, FormKey.Null, string.Empty);
                    }
                    if (x.FormKey.IsNull) {
                        return new State(StatusIndicatorState.Passive, FormKeyNull, FormKey.Null, string.Empty);
                    }

                    IMajorRecordIdentifier? record = null;
                    if (x.ScopedRecords != null) {
                        record = x.ScopedRecords.FirstOrDefault(ident => ident.FormKey == x.FormKey);
                    } else if (x.LinkCache.TryResolve(x.FormKey, EnabledTypes(x.Types), out var resolvedRecord)) {
                        record = resolvedRecord;
                    }

                    if (record == null) {
                        return new State(StatusIndicatorState.Failure, RecordNotResolved, x.FormKey, string.Empty);
                    }

                    return new State(StatusIndicatorState.Success, LocatedRecord, x.FormKey, record.EditorID ?? string.Empty);
                } catch (Exception ex) {
                    return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                }
            })
            .StartWith(new State(StatusIndicatorState.Passive, FormKeyNull, FormKey.Null, string.Empty))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(rec => {
                if (Processing) Processing = false;
                if (StatusString != rec.Text) StatusString = rec.Text;
                if (Status != rec.Status) Status = rec.Status;

                if (rec.Status == StatusIndicatorState.Success) {
                    if (EditorID != rec.EditorID) {
                        EditorID = rec.EditorID;
                    }

                    var formKeyStr = rec.FormKey.ToString();
                    if (formKeyStr != FormKeyStr) {
                        FormKeyStr = formKeyStr;
                    }

                    if (!Found) {
                        Found = true;
                    }
                } else {
                    if (EditorID != string.Empty) {
                        EditorID = string.Empty;
                    }

                    var formKeyStr = rec.FormKey.IsNull ? string.Empty : rec.FormKey.ToString();
                    if (FormKeyStr != formKeyStr) {
                        FormKeyStr = formKeyStr;
                    }

                    if (Found) {
                        Found = false;
                    }
                }
            })
            .Do(_ => _updating = UpdatingEnum.None)
            .Subscribe()
            .DisposeWith(UnloadDisposable);

        this.WhenAnyValue(x => x.EditorID)
            .Skip(1)
            .DistinctUntilChanged()
            .CombineLatest(
                this.WhenAnyValue(
                    x => x.LinkCache,
                    x => x.BlacklistFormKeys,
                    x => x.Filter),
                selectedTypesChanged,
                scopedRecordsChanged,
                (editorId, sources, types, scopedRecords)
                    => (EditorID: editorId,
                        LinkCache: sources.Item1,
                        BlacklistFormKeys: sources.Item2,
                        Filter: sources.Item3,
                        Types: types,
                        ScopedRecords: scopedRecords))
            .Where(_ => _updating is UpdatingEnum.None or UpdatingEnum.EditorID)
            .Do(_ => {
                _updating = UpdatingEnum.EditorID;
                if (!Processing) {
                    Processing = true;
                }
            })
            .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                try {
                    if (x.LinkCache == null) {
                        return new State(StatusIndicatorState.Passive, LinkCacheMissing, FormKey.Null, string.Empty);
                    }
                    if (string.IsNullOrWhiteSpace(x.EditorID)) {
                        return new State(StatusIndicatorState.Passive, "EditorID is empty. No lookup required", FormKey.Null, string.Empty);
                    }

                    var formKey = FormKey.Null;
                    if (x.ScopedRecords != null) {
                        formKey = x.ScopedRecords.FirstOrDefault(ident => ident.EditorID == x.EditorID)?.FormKey ?? FormKey.Null;
                    } else if (x.LinkCache.TryResolveIdentifier(x.EditorID, EnabledTypes(x.Types), out var resolvedFormKey)) {
                        formKey = resolvedFormKey;
                    }

                    if (formKey.IsNull) {
                        return new State(StatusIndicatorState.Failure, RecordNotResolved, formKey, x.EditorID);
                    }

                    if (x.Filter != null && !x.Filter(formKey, x.EditorID)) {
                        return new State(StatusIndicatorState.Passive, RecordFiltered, formKey, x.EditorID);
                    }
                    if (x.BlacklistFormKeys != null && x.BlacklistFormKeys.Contains(formKey)) {
                        return new State(StatusIndicatorState.Passive, FormKeyBlacklisted, formKey, x.EditorID);
                    }

                    return new State(StatusIndicatorState.Success, LocatedRecord, formKey, x.EditorID);
                } catch (Exception ex) {
                    return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                }
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(rec => {
                if (Processing) {
                    Processing = false;
                }

                if (StatusString != rec.Text) {
                    StatusString = rec.Text;
                }

                if (Status != rec.Status) {
                    Status = rec.Status;
                }

                if (rec.Status == StatusIndicatorState.Success) {
                    if (FormKey != rec.FormKey) {
                        FormKey = rec.FormKey;
                    }

                    if (!Found) {
                        Found = true;
                    }
                } else {
                    if (!FormKey.IsNull) {
                        FormKey = FormKey.Null;
                    }

                    if (FormKeyStr != string.Empty) {
                        FormKeyStr = string.Empty;
                    }

                    if (Found) {
                        Found = false;
                    }
                }
            })
            .Do(_ => _updating = UpdatingEnum.None)
            .Subscribe()
            .DisposeWith(UnloadDisposable);

        this.WhenAnyValue(x => x.FormKeyStr)
            .Skip(1)
            .Select(x => x.Trim())
            .DistinctUntilChanged()
            .CombineLatest(
                this.WhenAnyValue(
                    x => x.LinkCache,
                    x => x.BlacklistFormKeys,
                    x => x.Filter,
                    x => x.MissingMeansError,
                    x => x.MissingMeansNull),
                selectedTypesChanged,
                scopedRecordsChanged,
                (str, sources, types, scopedRecords)
                    => (Raw: str,
                        LinkCache: sources.Item1,
                        BlacklistFormKeys: sources.Item2,
                        Filter: sources.Item3,
                        Types: types,
                        ScopedRecords: scopedRecords,
                        MissingMeansError: sources.Item4,
                        MissingMeansNull: sources.Item5))
            .Where(_ => _updating is UpdatingEnum.None or UpdatingEnum.FormStr)
            .Do(_ => {
                _updating = UpdatingEnum.FormStr;
                if (!Processing) {
                    Processing = true;
                }
            })
            .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                try {
                    if (string.IsNullOrWhiteSpace(x.Raw)) {
                        return new State(StatusIndicatorState.Passive, "Input is empty.  No lookup required", FormKey.Null, string.Empty);
                    }

                    if (FormKey.TryFactory(x.Raw, out var formKey)) {
                        if (x.BlacklistFormKeys != null && x.BlacklistFormKeys.Contains(formKey)) {
                            return new State(StatusIndicatorState.Passive, FormKeyBlacklisted, formKey, string.Empty);
                        }

                        if (x.LinkCache == null) {
                            return new State(StatusIndicatorState.Success, "Valid FormKey", formKey, string.Empty);
                        }

                        IMajorRecordIdentifier? record = null;
                        if (x.ScopedRecords != null) {
                            record = x.ScopedRecords.FirstOrDefault(ident => ident.FormKey == formKey);
                        } else if (x.LinkCache.TryResolve(formKey, EnabledTypes(x.Types), out var resolvedRecord)) {
                            record = resolvedRecord;
                        }

                        if (record == null) {
                            return new State(
                                x.MissingMeansError ? StatusIndicatorState.Failure : StatusIndicatorState.Success,
                                RecordNotResolved,
                                x.MissingMeansNull ? FormKey.Null : formKey,
                                string.Empty);
                        }

                        var editorID = record.EditorID ?? string.Empty;
                        if (x.Filter != null && !x.Filter(formKey, editorID)) {
                            return new State(StatusIndicatorState.Passive, RecordFiltered, formKey, editorID);
                        }

                        return new State(StatusIndicatorState.Success, LocatedRecord, formKey, editorID);
                    }

                    if (x.LinkCache == null) {
                        return new State(StatusIndicatorState.Passive, LinkCacheMissing, FormKey.Null, string.Empty);
                    }

                    if (FormID.TryFactory(x.Raw, out var formID)) {
                        if (x.LinkCache.ListedOrder.Count >= formID.ModIndex.ID) {
                            var targetMod = x.LinkCache.ListedOrder[formID.ModIndex.ID];
                            formKey = new FormKey(targetMod.ModKey, formID.ID);
                            return x.LinkCache.TryResolveIdentifier(formKey, EnabledTypes(x.Types), out var editorId)
                                ? new State(StatusIndicatorState.Success, LocatedRecord, formKey, editorId ?? string.Empty)
                                : new State(StatusIndicatorState.Failure, RecordNotResolved, FormKey.Null, string.Empty);
                        }
                    }

                    return new State(StatusIndicatorState.Failure, RecordNotResolved, FormKey.Null, string.Empty);
                } catch (Exception ex) {
                    return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                }
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(rec => {
                if (Processing) Processing = false;
                if (StatusString != rec.Text) StatusString = rec.Text;
                if (Status != rec.Status) Status = rec.Status;

                if (rec.Status == StatusIndicatorState.Success) {
                    if (FormKey != rec.FormKey) {
                        FormKey = rec.FormKey;
                    }

                    var formKeyStr = rec.FormKey.ToString();
                    if (FormKeyStr != formKeyStr) {
                        FormKeyStr = formKeyStr;
                    }

                    if (EditorID != rec.EditorID) {
                        EditorID = rec.EditorID;
                    }

                    if (!Found) {
                        Found = true;
                    }
                } else {
                    if (FormKey != rec.FormKey) {
                        FormKey = rec.FormKey;
                    }

                    if (EditorID != string.Empty) {
                        EditorID = string.Empty;
                    }

                    if (Found) {
                        Found = false;
                    }
                }
            })
            .Do(_ => _updating = UpdatingEnum.None)
            .Subscribe()
            .DisposeWith(UnloadDisposable);

        ApplicableEditorIDs = this.WhenAnyValue(x => x.LinkCache)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .CombineLatest(
                selectedModsChanged,
                selectedTypesChanged,
                scopedRecordsChanged,
                (linkCache, selectedMods, enabledTypes, scopedRecords)
                    => (LinkCache: linkCache,
                        SelectedMods: selectedMods,
                        Types: enabledTypes,
                        ScopedRecords: scopedRecords))
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                var enabledTypes = EnabledTypes(x.Types).ToArray();
                if (!enabledTypes.Any()) return Observable.Empty<IMajorRecordIdentifier>();

                if (x.ScopedRecords != null) return x.ScopedRecords.Where(r => r.Type.InheritsFromAny(enabledTypes)).ToObservable();

                return Observable.Create<IMajorRecordIdentifier>((obs, cancel) => {
                    try {
                        if (x.LinkCache == null) return Task.CompletedTask;

                        foreach (var item in x.LinkCache.AllIdentifiers(enabledTypes, cancel)) {
                            if (cancel.IsCancellationRequested) return Task.CompletedTask;

                            if (x.SelectedMods.Where(modItem => modItem.IsSelected).All(modItem => modItem.ModKey != item.FormKey.ModKey)) continue;

                            obs.OnNext(item);
                        }
                    } catch (Exception ex) {
                        obs.OnError(ex);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                });
            })
            .FlowSwitch(this.WhenAnyValue(x => x.InSearchMode), Observable.Empty<IMajorRecordIdentifier>())
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ObserveOnGui()
            .Filter(Observable.CombineLatest(
                    this.WhenAnyValue(x => x.SearchMode)
                        .DistinctUntilChanged(),
                    this.WhenAnyValue(x => x.LinkCache),
                    (searchMode, cache) => (SearchMode: searchMode, Cache: cache))
                .Select(x => {
                    switch (x.SearchMode) {
                        case FormKeyPickerSearchMode.None:
                            return Observable.Return<Func<IMajorRecordIdentifier, bool>>(_ => false);
                        case FormKeyPickerSearchMode.EditorID:
                            return Observable.CombineLatest(
                                    this.WhenAnyValue(x => x.LinkCache),
                                    this.WhenAnyValue(x => x.EditorID),
                                    this.WhenAnyValue(x => x.BlacklistFormKeys),
                                    this.WhenAnyValue(x => x.Filter),
                                    this.WhenAnyValue(x => x.NameSelector),
                                    (linkCache, editorId, blacklistFormKeys, filter, nameSelector)
                                        => (LinkCache: linkCache,
                                            EditorID: editorId,
                                            BlacklistFormKeys: blacklistFormKeys,
                                            Filter: filter,
                                            NameSelector: nameSelector))
                                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                .ObserveOn(RxApp.TaskpoolScheduler)
                                .Select(data => {
                                    return new Func<IMajorRecordIdentifier, bool>(ident => {
                                        if (data.Filter != null && !data.Filter(ident.FormKey, ident.EditorID)) return false;
                                        if (data.BlacklistFormKeys != null && data.BlacklistFormKeys.Contains(ident.FormKey)) return false;
                                        if (data.EditorID.IsNullOrWhitespace()) return true;

                                        var editorID = data.NameSelector == null ? ident.EditorID : data.NameSelector(ident, data.LinkCache);
                                        return !editorID.IsNullOrWhitespace() && editorID.ContainsInsensitive(data.EditorID);
                                    });
                                });
                        case FormKeyPickerSearchMode.FormKey:
                            var modKeyToId = x.Cache?.ListedOrder
                                    .Select((mod, index) => (mod, index))
                                    .Take(ModIndex.MaxIndex)
                                    .ToDictionary(x => x.mod.ModKey, x => (byte) x.index)
                             ?? default;

                            return this.WhenAnyValue(x => x.FormKeyStr)
                                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                .ObserveOn(RxApp.TaskpoolScheduler)
                                .Select(rawStr => (RawStr: rawStr, FormKey: FormKey.TryFactory(rawStr), FormID: FormID.TryFactory(rawStr, false)))
                                .Select<(string RawStr, FormKey? FormKey, FormID? ID), Func<IMajorRecordIdentifier, bool>>(term => ident => {
                                    var fk = ident.FormKey;
                                    if (fk == term.FormKey) return true;
                                    if (term.ID == null) return false;

                                    if (term.RawStr.Length <= 6) return fk.ID == term.ID.Value.Raw;
                                    if (modKeyToId == null || !modKeyToId.TryGetValue(fk.ModKey, out var index)) return false;

                                    var formID = new FormID(new ModIndex(index), fk.ID);
                                    return formID.Raw == term.ID.Value.Raw;
                                });
                        default:
                            throw new NotImplementedException();
                    }
                })
                .Switch())
            .Sort(RecordComparers.EditorIDComparer)
            .ToObservableCollection(x => new RecordNamePair(x, NameSelector(x, LinkCache)), LoadedDisposable);


        this.WhenAnyValue(x => x.AllowsSearchMode)
            .Where(x => !x)
            .Subscribe(_ => InSearchMode = false)
            .DisposeWith(UnloadDisposable);

        this.WhenAnyValue(x => x.InSearchMode)
            .Where(x => !x)
            .ObserveOnGui()
            .Subscribe(_ => ViewingAllowedTypes = false)
            .DisposeWith(UnloadDisposable);
    }

    private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> InterfaceCache = new();

    protected IEnumerable<Type> GetMajorTypes(IEnumerable? types) {
        var majorRecordType = typeof(IMajorRecordGetter);

        if (types is not IEnumerable<Type> scopedTypes || !scopedTypes.Any()) {
            return majorRecordType.AsEnumerable();
        }

        var list = scopedTypes
            .Select(type => {
                if (LoquiRegistration.TryGetRegister(type, out _)) return type.AsEnumerable();
                if (InterfaceCache.TryGetValue(type, out var implementingTypes)) return implementingTypes;

                var list = IMajorRecordGetter.StaticRegistration.GetterType
                    .GetSubclassesOf()
                    .Where(x => x.InheritsFrom(type))
                    .Select(t => LoquiRegistration.TryGetRegister(t, out var registration) ? registration.GetterType : null)
                    .NotNull()
                    .Distinct()
                    .ToList();

                InterfaceCache.TryAdd(type, list);
                return list;
            })
            .SelectMany(x => x)
            .ToList();

        return list
            .OrderBy(x => x.Name);
    }

    protected IEnumerable<Type> EnabledTypes(IEnumerable<TypeItem> types) {
        return types.Where(x => x.IsSelected).Select(x => x.Type);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var editorIdBox = e.NameScope.Find<TextBox>("PART_EditorIDBox");
        AttachTextBox(editorIdBox, FormKeyPickerSearchMode.EditorID);
        var formKeyBox = e.NameScope.Find<TextBox>("PART_FormKeyBox");
        AttachTextBox(formKeyBox, FormKeyPickerSearchMode.FormKey);
    }

    private void AttachTextBox(TextBox? textBox, FormKeyPickerSearchMode searchMode) {
        if (textBox == null) return;

        textBox.WhenAnyValue(x => x.Text)
            .Skip(1)
            .FlowSwitch(textBox.WhenAnyValue(x => x.IsFocused))
            .Subscribe(_ => SearchMode = searchMode)
            .DisposeWith(TemplateDisposable);

        var pressed = new Subject<Unit>();
        textBox.AddHandler(PointerPressedEvent, (_, _) => pressed.OnNext(Unit.Default), handledEventsToo: true);

        textBox.WhenAnyValue(x => x.IsFocused)
            .DistinctUntilChanged()
            .Where(focused => focused)
            .CombineLatest(pressed, (b, _) => b)
            .WithLatestFrom(
                this.WhenAnyValue(x => x.Found),
                (_, found) => found)
            .Where(found => !found)
            .Subscribe(_ => SearchMode = searchMode)
            .DisposeWith(TemplateDisposable);
    }
}
