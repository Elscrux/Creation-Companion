using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed class EditableCondition : ReactiveObject {
    public enum ExtendedRunOnType {
        Subject,
        Target,
        Reference,
        CombatTarget,
        LinkedReference,
        QuestAlias,
        PackageData,
        EventData,
        Player
    }

    private readonly CompositeDisposable _disposable = new();

    [Reactive] public ExtendedRunOnType RunOnType { get; set; }
    [Reactive] public FormLink<IPlacedGetter> ReferenceLink { get; set; } = new();

    [Reactive] public Condition.Function Function { get; set; }
    [Reactive] public ConditionData Data { get; set; }
    [Reactive] public IObservable<Unit>? DataChanged { get; set; }

    [Reactive] public CompareOperator CompareOperator { get; set; }

    [Reactive] public FormKey GlobalValue { get; set; }
    [Reactive] public float FloatValue { get; set; }

    [Reactive] public bool UseGlobal { get; set; }

    [Reactive] public bool Or { get; set; }
    [Reactive] public bool SwapSubjectAndTarget { get; set; }

    [Reactive] public int QuestAlias { get; set; }
    [Reactive] public sbyte PackageData { get; set; }
    [Reactive] public Enum? EventData { get; set; }

    public IObservable<bool>? ShowMoreRunOn { get; }
    public IObservable<bool>? ShowReference { get; }
    public IObservable<bool>? ShowQuestAlias { get; }
    public IObservable<bool>? ShowPackageData { get; }
    public IObservable<bool>? ShowEventData { get; }

    public EditableCondition() {
        Data = new GetIsIDConditionData();
        Function = Condition.Function.GetIsID;
        RunOnType = ExtendedRunOnType.Subject;

        var runOnTypeChanged = this.WhenAnyValue(x => x.RunOnType);

        ShowReference = runOnTypeChanged
            .Select(runOnType => runOnType == ExtendedRunOnType.Reference);

        ShowQuestAlias = runOnTypeChanged
            .Select(runOnType => runOnType == ExtendedRunOnType.QuestAlias);

        ShowPackageData = runOnTypeChanged
            .Select(runOnType => runOnType == ExtendedRunOnType.PackageData);

        ShowEventData = runOnTypeChanged
            .Select(runOnType => runOnType == ExtendedRunOnType.EventData);

        ShowMoreRunOn = Observable.CombineLatest(
                ShowReference,
                ShowQuestAlias,
                ShowPackageData,
                ShowEventData)
            .Select(x => x.Any(b => b));

        // Update condition data when the function changes
        this.WhenAnyValue(x => x.Function)
            .DistinctUntilChanged()
            .Subscribe(function => Data = function.ToCondition())
            .DisposeWith(_disposable);

        // Set the reference to the player when RunOnType is player
        this.WhenAnyValue(x => x.RunOnType)
            .DistinctUntilChanged()
            .Subscribe(runOnType => {
                if (runOnType == ExtendedRunOnType.Player) {
                    ReferenceLink.SetTo(Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.PlayerRef.FormKey);
                }
            })
            .DisposeWith(_disposable);
    }

    private static readonly PropertyInfo? FunctionProperty = typeof(IConditionDataGetter).GetProperty(nameof(IConditionDataGetter.Function));

    public EditableCondition(ICondition parent) : this() {
        var value = FunctionProperty?.GetValue(parent.Data);
        if (value is Condition.Function function) Function = function;

        switch (parent) {
            case IConditionFloat conditionFloat:
                UseGlobal = false;
                FloatValue = conditionFloat.ComparisonValue;
                break;
            case IConditionGlobal conditionGlobal:
                UseGlobal = true;
                GlobalValue = conditionGlobal.ComparisonValue.FormKey;
                break;
        }

        Data = parent.Data;
        ReferenceLink = new FormLink<IPlacedGetter>(Data.Reference.FormKey);

        if (Data.Reference.FormKey == Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.PlayerRef.FormKey
         && Data.RunOnType == Condition.RunOnType.Reference) {
            RunOnType = ExtendedRunOnType.Player;
        } else {
            RunOnType = (ExtendedRunOnType) Data.RunOnType;
        }

        CompareOperator = parent.CompareOperator;

        Or = (parent.Flags & Condition.Flag.OR) != 0;
        SwapSubjectAndTarget = (parent.Flags & Condition.Flag.SwapSubjectAndTarget) != 0;

        switch (Data.RunOnType) {
            case Condition.RunOnType.QuestAlias:
                QuestAlias = Data.Unknown3;
                break;
            case Condition.RunOnType.PackageData:
                PackageData = (sbyte) Data.Unknown3;
                break;
            case Condition.RunOnType.EventData:
                EventData = (GetEventDataConditionData.EventMember) Data.Unknown3;
                break;
        }
    }

    public Condition ToCondition() {
        Condition condition = UseGlobal
            ? new ConditionGlobal { ComparisonValue = new FormLink<IGlobalGetter>(GlobalValue) }
            : new ConditionFloat { ComparisonValue = FloatValue };

        condition.Data = Data;
        condition.Data.RunOnType = RunOnType == ExtendedRunOnType.Player
            ? Condition.RunOnType.Reference
            : (Condition.RunOnType) RunOnType;
        condition.Data.Reference = new FormLink<ISkyrimMajorRecordGetter>(ReferenceLink.FormKey);

        if (Or) condition.Flags |= Condition.Flag.OR;
        if (SwapSubjectAndTarget) condition.Flags |= Condition.Flag.SwapSubjectAndTarget;

        condition.CompareOperator = CompareOperator;

        condition.Data.Unknown3 = RunOnType switch {
            ExtendedRunOnType.QuestAlias => QuestAlias,
            ExtendedRunOnType.PackageData => PackageData,
            ExtendedRunOnType.EventData => Convert.ToInt32(EventData),
            _ => -1
        };

        return condition;
    }

    private static readonly List<string> BannedProperties = new() {
        "StaticRegistration",
        "RunOnType",
        "Reference",
        "Unknown3",
        "UseAliases",
        "UsePackageData",
    };

    public static IEnumerable<PropertyInfo> GetParameterProperties(ConditionData data) {
        return data
            .GetType()
            .GetProperties()
            .Where(propertyInfo => {
                if (propertyInfo.Name.Contains("Unused")) return false;

                return !BannedProperties.Contains(propertyInfo.Name);
            });
    }
}
