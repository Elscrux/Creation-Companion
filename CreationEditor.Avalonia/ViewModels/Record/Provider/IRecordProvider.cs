using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public interface IRecordProvider : IDisposable {
    /// <summary>
    /// Cache of all records
    /// </summary>
    SourceCache<IReferencedRecord, FormKey> RecordCache { get; }

    IReferencedRecord? SelectedRecord { get; set; }

    IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    /// <summary>
    /// Emits true when records are being loaded and false when loading has finished
    /// </summary>
    IObservable<bool> IsBusy { get; }

    IList<IMenuItem> ContextMenuItems { get; }
    ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
}

public interface IRecordProvider<TReferenced> : IRecordProvider
    where TReferenced : IReferencedRecord {
    public new TReferenced? SelectedRecord { get; set; }

    public static readonly Func<IRecordBrowserSettingsVM, IObservable<Func<TReferenced, bool>>> DefaultFilter = recordBrowserSettingsVM =>
        recordBrowserSettingsVM.SettingsChanged
            .StartWith(Unit.Default)
            .Select(_ => new Func<TReferenced, bool>(record => recordBrowserSettingsVM.Filter(record.Record)));
}
