using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public interface IRecordProvider : IDisposable {
    /// <summary>
    /// Cache of all records
    /// </summary>
    SourceCache<IReferencedRecord, FormKey> RecordCache { get; }

    IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    IRecordBrowserSettings RecordBrowserSettings { get; }

    IEnumerable<Type> RecordTypes { get; }

    /// <summary>
    /// Emits true when records are being loaded and false when loading has finished
    /// </summary>
    IObservable<bool> IsBusy { get; }
}

public interface IRecordProvider<TReferenced> : IRecordProvider
    where TReferenced : IReferencedRecord {
    static readonly Func<IRecordBrowserSettings, IObservable<Func<TReferenced, bool>>> DefaultFilter = recordBrowserSettingsVM =>
        recordBrowserSettingsVM.SettingsChanged
            .ThrottleMedium()
            .StartWith(Unit.Default)
            .Select(_ => new Func<TReferenced, bool>(record => recordBrowserSettingsVM.Filter(record.Record)))
            .Replay(1)
            .RefCount();
}
