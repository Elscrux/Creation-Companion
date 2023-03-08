using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public interface IRecordProvider {
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; }
    public IReferencedRecord? SelectedRecord { get; set; }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public IObservable<bool> IsBusy { get; set; }

    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
}
public interface IRecordProvider<TReferenced> : IRecordProvider
    where TReferenced : IReferencedRecord {
    public new TReferenced? SelectedRecord { get; set; }

    public static readonly Func<IRecordBrowserSettingsVM, IObservable<Func<TReferenced, bool>>> DefaultFilter = recordBrowserSettingsVM =>
        recordBrowserSettingsVM.SettingsChanged
            .StartWith(Unit.Default)
            .Select(_ => new Func<TReferenced, bool>(record => recordBrowserSettingsVM.Filter(record.Record)));
}
