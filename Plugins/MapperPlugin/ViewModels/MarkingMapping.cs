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
using ReactiveUI.Fody.Helpers;
namespace MapperPlugin.ViewModels;

public sealed class MarkingMapping : ReactiveObject, IMementoProvider<MarkingMappingMemento> {
    private const uint MaxAutomaticUpdateSize = 10;
    private readonly Subject<Unit> _forceUpdates = new();

    public MarkingMapping(QueryVM queryVM) {
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

    [Reactive] public bool NeedsForceUpdate { get; set; }
    [Reactive] public bool Enable { get; set; } = true;
    [Reactive] public bool UseQuery { get; set; }
    [Reactive] public bool UseRandomColorsInQuery { get; set; }
    public QueryVM QueryVM { get; set; }
    [Reactive] public IFormLinkGetter Record { get; set; } = FormLinkInformation.Null;
    [Reactive] public Color Color { get; set; }
    [Reactive] public float Size { get; set; } = 1;

    public IObservable<Unit> LogicalUpdates { get; }
    public IObservable<Unit> VisualUpdates { get; }

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
