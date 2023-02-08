using System.Collections;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using CreationEditor.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.FormKeyPicker;

[TemplatePart(Name = "PART_EditorIDBox", Type = typeof(TextBox))]
[TemplatePart(Name = "PART_FormKeyBox", Type = typeof(TextBox))]
public class AFormKeyPicker : DisposableTemplatedControl {
    private enum UpdatingEnum { None, FormKey, EditorID, FormStr }
    private UpdatingEnum _updating;

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

    private bool _inSearchMode;
    [Reactive] public bool InSearchMode {
        get => _inSearchMode;
        protected set => SetAndRaise(InSearchModeProperty, ref _inSearchMode, value);
    }
    public static readonly DirectProperty<AFormKeyPicker, bool> InSearchModeProperty = AvaloniaProperty.RegisterDirect<AFormKeyPicker, bool>(nameof(InSearchMode), picker => picker.InSearchMode);

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
    #endregion

    private sealed record State(StatusIndicatorState Status, string Text, FormKey FormKey, string EditorID);

    public AFormKeyPicker() {
        ToggleViewAllowedTypesCommand = ReactiveCommand.Create(() => ViewingAllowedTypes = !ViewingAllowedTypes);
    }

    protected override void OnLoaded() {
        base.OnLoaded();
        this.WhenAnyValue(x => x.FormKey)
            .DistinctUntilChanged()
            .CombineLatest(
                this.WhenAnyValue(
                    x => x.LinkCache,
                    x => x.ScopedTypes),
                (form, sources) => (FormKey: form, LinkCache: sources.Item1, Types: sources.Item2))
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
                        return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                    }
                    if (x.FormKey.IsNull) {
                        return new State(StatusIndicatorState.Passive, "FormKey is null.  No lookup required", FormKey.Null, string.Empty);
                    }
                    var scopedTypes = ScopedTypesInternal(x.Types);
                    return x.LinkCache.TryResolveIdentifier(x.FormKey, scopedTypes, out var editorId)
                        ? new State(StatusIndicatorState.Success, "Located record", x.FormKey, editorId ?? string.Empty)
                        : new State(StatusIndicatorState.Failure, "Could not resolve record", x.FormKey, string.Empty);
                } catch (Exception ex) {
                    return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                }
            })
            .StartWith(new State(StatusIndicatorState.Passive, "FormKey is null.  No lookup required", FormKey.Null, string.Empty))
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
                    x => x.ScopedTypes),
                (editorId, sources) => (EditorID: editorId, LinkCache: sources.Item1, Types: sources.Item2))
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
                    var (editorID, linkCache, types) = x;
                    if (linkCache == null) return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                    if (string.IsNullOrWhiteSpace(editorID)) return new State(StatusIndicatorState.Passive, "EditorID is empty.  No lookup required", FormKey.Null, string.Empty);

                    var scopedTypes = ScopedTypesInternal(types);
                    return linkCache.TryResolveIdentifier(editorID, scopedTypes, out var formKey)
                        ? new State(StatusIndicatorState.Success, "Located record", formKey, editorID)
                        : new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
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
                    x => x.ScopedTypes,
                    x => x.MissingMeansError,
                    x => x.MissingMeansNull),
                (str, sources) => (Raw: str, LinkCache: sources.Item1, Types: sources.Item2, MissingMeansError: sources.Item3, MissingMeansNull: sources.Item4))
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

                    var scopedTypes = ScopedTypesInternal(x.Types);

                    if (FormKey.TryFactory(x.Raw, out var formKey)) {
                        if (x.LinkCache == null) return new State(StatusIndicatorState.Success, "Valid FormKey", formKey, string.Empty);

                        if (x.LinkCache.TryResolveIdentifier(formKey, scopedTypes, out var editorId)) {
                            return new State(StatusIndicatorState.Success, "Located record", formKey, editorId ?? string.Empty);
                        }
                        
                        var formKeyToUse = x.MissingMeansNull ? FormKey.Null : formKey;
                        return new State(
                            x.MissingMeansError ? StatusIndicatorState.Failure : StatusIndicatorState.Success,
                            "Could not resolve record",
                            formKeyToUse,
                            string.Empty);
                    }

                    if (x.LinkCache == null) {
                        return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                    }

                    if (FormID.TryFactory(x.Raw, out var formID, strictLength: true)) {
                        if (x.LinkCache.ListedOrder.Count >= formID.ModIndex.ID) {
                            var targetMod = x.LinkCache.ListedOrder[formID.ModIndex.ID];
                            formKey = new FormKey(targetMod.ModKey, formID.ID);
                            return x.LinkCache.TryResolveIdentifier(formKey, scopedTypes, out var editorId)
                                ? new State(StatusIndicatorState.Success, "Located record", formKey, editorId ?? string.Empty)
                                : new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
                        }
                    }

                    return new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
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

        ApplicableEditorIDs = Observable.CombineLatest(
                this.WhenAnyValue(x => x.LinkCache),
                this.WhenAnyValue(x => x.ScopedTypes),
                (linkCache, scopedTypes) => (LinkCache: linkCache, ScopedTypes: scopedTypes))
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => Observable.Create<IMajorRecordIdentifier>((obs, cancel) => {
                try {
                    var scopedTypes = ScopedTypesInternal(x.ScopedTypes);
                    if (!scopedTypes.Any() || x.LinkCache == null) {
                        return Task.CompletedTask;
                    }
                    foreach (var item in x.LinkCache.AllIdentifiers(scopedTypes, cancel)) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;

                        obs.OnNext(item);
                    }
                } catch (Exception ex) {
                    obs.OnError(ex);
                }
                obs.OnCompleted();
                return Task.CompletedTask;
            }))
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
                            return this.WhenAnyValue(x => x.EditorID)
                                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                .ObserveOn(RxApp.TaskpoolScheduler)
                                .Select<string, Func<IMajorRecordIdentifier, bool>>(term => ident => {
                                    if (term.IsNullOrWhitespace()) return true;

                                    var editorID = ident.EditorID;
                                    return !editorID.IsNullOrWhitespace() && editorID.ContainsInsensitive(term);

                                });
                        case FormKeyPickerSearchMode.FormKey:

                            var modKeyToId = x.Cache?.ListedOrder
                                    .Select((mod, index) => (mod, index))
                                    .Take(ModIndex.MaxIndex)
                                    .ToDictionary(keySelector: x => x.mod.ModKey, elementSelector: x => (byte) x.index)
                             ?? default;

                            return this.WhenAnyValue(x => x.FormKeyStr)
                                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                .ObserveOn(RxApp.TaskpoolScheduler)
                                .Select(rawStr => (RawStr: rawStr, FormKey: FormKey.TryFactory(rawStr), FormID: FormID.TryFactory(rawStr, strictLength: false)))
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
            .ObserveOnGui()
            .ToObservableCollection(LoadedDisposable);


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

    protected IEnumerable<Type> ScopedTypesInternal(IEnumerable? types) {
        if (types is not IEnumerable<Type> scopedTypes || !scopedTypes.Any()) {
            scopedTypes = typeof(IMajorRecordGetter).AsEnumerable();
        }
        return scopedTypes;
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

        textBox.WhenAnyValue(x => x.IsFocused)
            .DistinctUntilChanged()
            .Where(focused => focused)
            .WithLatestFrom(
                this.WhenAnyValue(x => x.Found),
                (_, found) => found)
            .Where(found => !found)
            .Subscribe(_ => SearchMode = searchMode)
            .DisposeWith(TemplateDisposable);

        Observable.Merge(
                this.WhenAnyValue(x => x.IsKeyboardFocusWithin),
                this.WhenAnyValue(x => x.IsVisible))
            .Where(x => !x)
            .Delay(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
            .Subscribe(_ => {
                SearchMode = FormKeyPickerSearchMode.None;
            })
            .DisposeWith(TemplateDisposable);
    }
}
