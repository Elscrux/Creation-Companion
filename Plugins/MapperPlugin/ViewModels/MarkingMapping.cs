using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Media;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Core;
using MapperPlugin.Model;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace MapperPlugin.ViewModels;

public sealed partial class MarkingMapping : ReactiveObject, IMementoProvider<MarkingMappingMemento>, IMementoReceiver<MarkingMappingMemento> {
    private const uint MaxAutomaticUpdateSize = 10;
    private readonly Subject<Unit> _forceUpdates = new();

    [Reactive] public partial bool NeedsForceUpdate { get; set; }
    [Reactive] public partial bool Enable { get; set; }
    [Reactive] public partial bool UseQuery { get; set; }
    [Reactive] public partial bool UseRandomColorsInQuery { get; set; }
    public QueryVM QueryVM { get; set; }
    [Reactive] public partial IFormLinkGetter Record { get; set; }
    [Reactive] public partial Color Color { get; set; }
    [Reactive] public partial float Size { get; set; }

    public IObservable<Unit> LogicalUpdates { get; }
    public IObservable<Unit> VisualUpdates { get; }

    public MarkingMapping(QueryVM queryVM) {
        Enable = true;
        Size = 1;
        Record = FormLinkInformation.Null;
        QueryVM = queryVM;

        LogicalUpdates = this.WhenAnyValue(x => x.UseQuery)
            .Select(useQuery => useQuery
                ? QueryVM.QueryRunner.SettingsChanged
                    .ThrottleVeryLong()
                    .Where(_ => QueryVM.QueryRunner.QueryFrom.SelectedItem is not null)
                    .Select(_ => QueryVM.QueryRunner
                        .RunQuery()
                        .OfType<IFormLinkIdentifier>()
                        .CountGreaterThan(MaxAutomaticUpdateSize))
                    .ObserveOnGui()
                    .Do(needsForceUpdate => NeedsForceUpdate = needsForceUpdate)
                    .ObserveOnTaskpool()
                    .WhereFalse()
                    .Unit()
                    .Merge(_forceUpdates)
                : this.WhenAnyValue(marking => marking.Record)
                    .Where(record => !record.IsNull)
                    .Unit())
            .Switch();

        VisualUpdates = this.WhenAnyValue(
                x => x.Enable,
                x => x.Color,
                x => x.UseRandomColorsInQuery,
                x => x.Size)
            .Unit();
    }

    public void ForceUpdate() {
        NeedsForceUpdate = false;
        _forceUpdates.OnNext(Unit.Default);
    }

    public IEnumerable<IFormLinkIdentifier> CurrentRecords => UseQuery
        ? QueryVM.QueryRunner.RunQuery().OfType<IFormLinkIdentifier>()
        : [Record];

    public MarkingMappingMemento CreateMemento() {
        return new MarkingMappingMemento(
            Enable,
            UseQuery,
            UseRandomColorsInQuery,
            new FormLinkInformation(Record.FormKey, Record.Type),
            QueryVM.QueryRunner.CreateMemento(),
            Color,
            Size);
    }

    public void RestoreMemento(MarkingMappingMemento memento) {
        Enable = memento.Enable;
        UseQuery = memento.UseQuery;
        UseRandomColorsInQuery = memento.UseRandomColorsInQuery;
        Record = memento.Record;
        QueryVM.QueryRunner.RestoreMemento(memento.QueryRunner);
        Color = memento.Color;
        Size = memento.Size;
    }
}
