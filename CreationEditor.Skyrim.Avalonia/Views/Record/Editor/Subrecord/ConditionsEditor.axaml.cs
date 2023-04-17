using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using CreationEditor.Skyrim.Avalonia.Services.Record.Editor;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.Subrecord;

public partial class ConditionsEditor : LoadedUserControl {
    public static readonly StyledProperty<ObservableCollection<EditableCondition>> ConditionsProperty
        = AvaloniaProperty.Register<ConditionsEditor, ObservableCollection<EditableCondition>>(nameof(Conditions));

    public static readonly StyledProperty<IMajorRecordGetter?> ContextProperty
        = AvaloniaProperty.Register<ConditionsEditor, IMajorRecordGetter?>(nameof(Context));

    public static readonly StyledProperty<IQuestGetter?> QuestContextProperty
        = AvaloniaProperty.Register<ConditionsEditor, IQuestGetter?>(nameof(QuestContext));

    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<ConditionsEditor, ILinkCache?>(nameof(LinkCache));

    public static readonly StyledProperty<ReadOnlyObservableCollection<EditableCondition.ExtendedRunOnType>> RunOnTypesProperty
        = AvaloniaProperty.Register<ConditionsEditor, ReadOnlyObservableCollection<EditableCondition.ExtendedRunOnType>>(nameof(RunOnTypes));

    public static readonly StyledProperty<ReadOnlyObservableCollection<Condition.Function>> FunctionsProperty
        = AvaloniaProperty.Register<ConditionsEditor, ReadOnlyObservableCollection<Condition.Function>>(nameof(Functions));

    public static readonly StyledProperty<IConditionCopyPasteController?> CopyPasteControllerProperty
        = AvaloniaProperty.Register<ConditionsEditor, IConditionCopyPasteController?>(nameof(CopyPasteController));

    public static readonly StyledProperty<ReactiveCommand<IList, Unit>> CopyProperty
        = AvaloniaProperty.Register<ConditionsEditor, ReactiveCommand<IList, Unit>>(nameof(Copy));

    public static readonly StyledProperty<ReactiveCommand<IList, Unit>> CopyAllProperty
        = AvaloniaProperty.Register<ConditionsEditor, ReactiveCommand<IList, Unit>>(nameof(CopyAll));

    public static readonly StyledProperty<ReactiveCommand<int, Unit>> PasteProperty
        = AvaloniaProperty.Register<ConditionsEditor, ReactiveCommand<int, Unit>>(nameof(Paste));

    /// <summary>
    /// List of conditions which can be modified in the editor.
    /// </summary>
    public ObservableCollection<EditableCondition> Conditions {
        get => GetValue(ConditionsProperty);
        set => SetValue(ConditionsProperty, value);
    }

    /// <summary>
    /// Assigns a context which is used to determine available conditions functions, run on types and more.
    /// </summary>
    public IMajorRecordGetter? Context {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <summary>
    /// Quest context, only needed when the main context is not a quest.
    /// For example this is used for packages with an owning quest.
    /// </summary>
    public IQuestGetter? QuestContext {
        get => GetValue(QuestContextProperty);
        set => SetValue(QuestContextProperty, value);
    }

    /// <summary>
    /// Link Cache to access the load order.
    /// </summary>
    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    /// <summary>
    /// Controller for copy/paste functionality.
    /// </summary>
    public IConditionCopyPasteController? CopyPasteController {
        get => GetValue(CopyPasteControllerProperty);
        set => SetValue(CopyPasteControllerProperty, value);
    }

    public ReadOnlyObservableCollection<EditableCondition.ExtendedRunOnType> RunOnTypes {
        get => GetValue(RunOnTypesProperty);
        set => SetValue(RunOnTypesProperty, value);
    }

    public ReadOnlyObservableCollection<Condition.Function> Functions {
        get => GetValue(FunctionsProperty);
        set => SetValue(FunctionsProperty, value);
    }

    public ReactiveCommand<IList, Unit> Copy {
        get => GetValue(CopyProperty);
        set => SetValue(CopyProperty, value);
    }

    public ReactiveCommand<IList, Unit> CopyAll {
        get => GetValue(CopyAllProperty);
        set => SetValue(CopyAllProperty, value);
    }

    public ReactiveCommand<int, Unit> Paste {
        get => GetValue(PasteProperty);
        set => SetValue(PasteProperty, value);
    }

    public static readonly StyledProperty<FuncDataTemplate<EditableCondition>> FunctionTemplateProperty
        = AvaloniaProperty.Register<ConditionsEditor, FuncDataTemplate<EditableCondition>>(nameof(FunctionTemplate));

    public FuncDataTemplate<EditableCondition> FunctionTemplate {
        get => GetValue(FunctionTemplateProperty);
        set => SetValue(FunctionTemplateProperty, value);
    }

    public static Func<object, bool> CanDrop { get; } = o => o is EditableCondition;

    public ConditionsEditor() {
        InitializeComponent();

        Copy = ReactiveCommand.Create<IList>(
            conditions => CopyPasteController?.Copy(conditions.OfType<EditableCondition>().Select(c => c.ToCondition())),
            this.WhenAnyValue(x => x.ConditionsGrid.SelectedItem).Select(_ => ConditionsGrid.SelectedItems.Count > 0));

        CopyAll = ReactiveCommand.Create<IList>(
            conditions => CopyPasteController?.Copy(conditions.OfType<EditableCondition>().Select(c => c.ToCondition())),
            this.WhenAnyValue(x => x.Conditions.Count).Select(count => count > 0));

        Paste = ReactiveCommand.Create<int>(
            index => CopyPasteController?.Paste((IList<EditableCondition>) ConditionsGrid.Items, index + 1),
            this.WhenAnyValue(x => x.CopyPasteController)
                .NotNull()
                .CombineLatest(this.WhenAnyValue(x => x.CopyPasteController!.CanPaste), (_, x) => x));

        FunctionTemplate = new FuncDataTemplate<EditableCondition>((condition, _) => {
            var autoCompleteBox = new AutoCompleteBox {
                [!AutoCompleteBox.ItemsProperty] = this.GetObservable(FunctionsProperty).ToBinding(),
                FilterMode = AutoCompleteFilterMode.ContainsOrdinal,
                IsTextCompletionEnabled = true,
                MaxDropDownHeight = 750,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                SelectedItem = condition.Function
            };

            autoCompleteBox.WhenAnyValue(box => box.SelectedItem)
                .NotNull()
                .OfType<Condition.Function>()
                .ObserveOnGui()
                .Subscribe(function => condition.Function = function);

            return autoCompleteBox;
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);

        // Update RunOnTypes when the context changes
        RunOnTypes = this.WhenAnyValue(x => x.Context)
            .Select(context => {
                switch (context) {
                    case IPackageGetter:
                        return new[] {
                            EditableCondition.ExtendedRunOnType.Subject,
                            EditableCondition.ExtendedRunOnType.Target,
                            EditableCondition.ExtendedRunOnType.Reference,
                            EditableCondition.ExtendedRunOnType.LinkedReference,
                            EditableCondition.ExtendedRunOnType.PackageData,
                            EditableCondition.ExtendedRunOnType.Player,
                        };
                    case IQuestGetter:
                    case IStoryManagerEventNodeGetter:
                    case IStoryManagerBranchNodeGetter:
                    case IStoryManagerQuestNodeGetter:
                        return new[] {
                            EditableCondition.ExtendedRunOnType.Subject,
                            EditableCondition.ExtendedRunOnType.Target,
                            EditableCondition.ExtendedRunOnType.Reference,
                            EditableCondition.ExtendedRunOnType.LinkedReference,
                            EditableCondition.ExtendedRunOnType.QuestAlias,
                            EditableCondition.ExtendedRunOnType.EventData,
                            EditableCondition.ExtendedRunOnType.Player,
                        };
                    case IDialogResponsesGetter:
                    case IMagicEffectGetter:
                    case IIdleAnimationGetter:
                    case IPerkGetter:
                        return new[] {
                            EditableCondition.ExtendedRunOnType.Subject,
                            EditableCondition.ExtendedRunOnType.Target,
                            EditableCondition.ExtendedRunOnType.Reference,
                            EditableCondition.ExtendedRunOnType.CombatTarget,
                            EditableCondition.ExtendedRunOnType.LinkedReference,
                            EditableCondition.ExtendedRunOnType.Player,
                        };
                    default:
                        return new[] {
                            EditableCondition.ExtendedRunOnType.Subject,
                            EditableCondition.ExtendedRunOnType.Target,
                            EditableCondition.ExtendedRunOnType.Reference,
                            EditableCondition.ExtendedRunOnType.LinkedReference,
                            EditableCondition.ExtendedRunOnType.Player,
                        };
                }
            })
            .ToObservableCollection(UnloadDisposable);

        // Update Function when the context changes
        Functions = this.WhenAnyValue(x => x.Context)
            .Select(context => {
                return context switch {
                    IQuestGetter => ConditionConstants.QuestAndStoryManagerOnlyFunctions.Concat(ConditionConstants.QuestOnlyFunctions),
                    IStoryManagerEventNodeGetter => ConditionConstants.QuestAndStoryManagerOnlyFunctions,
                    IStoryManagerBranchNodeGetter => ConditionConstants.QuestAndStoryManagerOnlyFunctions,
                    IStoryManagerQuestNodeGetter => ConditionConstants.QuestAndStoryManagerOnlyFunctions,
                    IPerkGetter => ConditionConstants.PerkOnlyFunctions,
                    IPackageGetter p when p.OwnerQuest.IsNull => ConditionConstants.PackageOnlyFunctions,
                    IPackageGetter => ConditionConstants.PackageOnlyFunctions.Concat(ConditionConstants.QuestOnlyFunctions),
                    ICameraPathGetter => ConditionConstants.CameraPathOnlyFunctions,
                    _ => Array.Empty<Condition.Function>()
                };
            })
            .Select(additional => ConditionConstants.BaseFunctions.Concat(additional))
            .ToObservableCollection(UnloadDisposable);
    }
}
