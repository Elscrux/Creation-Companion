using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed partial class EditableCondition : ReactiveObject {
    public enum ExtendedRunOnType {
        Subject,
        Target,
        Reference,
        CombatTarget,
        LinkedReference,
        QuestAlias,
        PackageData,
        EventData,
        Player,
    }

    private static readonly PropertyInfo? FunctionProperty = typeof(IConditionDataGetter).GetProperty(nameof(IConditionDataGetter.Function));

    private readonly CompositeDisposable _disposable = new();

    [Reactive] public partial ExtendedRunOnType RunOnType { get; set; }
    [Reactive] public partial FormLink<IPlacedGetter> ReferenceLink { get; set; }

    [Reactive] public partial Condition.Function Function { get; set; }
    [Reactive] public partial ConditionData Data { get; set; }
    [Reactive] public partial IObservable<Unit>? DataChanged { get; set; }

    [Reactive] public partial CompareOperator CompareOperator { get; set; }

    [Reactive] public partial FormKey GlobalValue { get; set; }
    [Reactive] public partial float FloatValue { get; set; }

    [Reactive] public partial bool UseGlobal { get; set; }

    [Reactive] public partial bool Or { get; set; }
    [Reactive] public partial bool SwapSubjectAndTarget { get; set; }

    [Reactive] public partial int QuestAlias { get; set; }
    [Reactive] public partial sbyte PackageData { get; set; }
    [Reactive] public partial Enum? EventData { get; set; }

    public IObservable<bool>? ShowMoreRunOn { get; }
    public IObservable<bool>? ShowReference { get; }
    public IObservable<bool>? ShowQuestAlias { get; }
    public IObservable<bool>? ShowPackageData { get; }
    public IObservable<bool>? ShowEventData { get; }

    public EditableCondition() {
        ReferenceLink = new FormLink<IPlacedGetter>();
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

    public EditableCondition(IConditionGetter parent) : this() {
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

        Data = parent.Data.DeepCopy();
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
            _ => -1,
        };

        return condition;
    }

    private static readonly List<string> BannedProperties = [
        "StaticRegistration",
        "RunOnType",
        "Reference",
        "Unknown3",
        "UseAliases",
        "UsePackageData",
    ];

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
