using System.ComponentModel;
using System.Runtime.CompilerServices;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Record;

// TODO: analyzer integration
// TODO: edit  history

/// <summary>
/// This is the model of a Book class.
/// It presents the logic of a book record inside the editor. It can be changed from any source, for example the UI.
/// </summary>
public class PackageX : INotifyPropertyChanged {
    private readonly Package _package;
    // private readonly IDictionary<sbyte, APackageData> _data;

    public string? EditorID {
        get;
        set => SetField(ref field, value);
        // notify property changed is relevant for the UI
        // history will probably just be called when the record is saved
        // can possibly add automatic saving in editors to make sure history is frequently updated (setting toggleable, might not want to save, and actually abort changes?)
    }

    public string? Name {
        get;
        set => SetField(ref field, value);
    }

    public PackageX(Package package) {
        _package = package;
        IDictionary<sbyte, APackageData> aPackageDatas = package.Data;
        foreach (var (key, value) in aPackageDatas) {
            switch (value) {
                case PackageDataBool packageDataBool: {
                    packageDataBool.Data = true;
                    break;
                }
                case PackageDataFloat packageDataFloat: break;
                case PackageDataInt packageDataInt: break;
                case PackageDataLocation packageDataLocation: break;
                case PackageDataObjectList packageDataObjectList: break;
                case PackageDataTarget packageDataTarget: break;
                case PackageDataTopic packageDataTopic: break;
                default: throw new ArgumentOutOfRangeException(nameof(value));

            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public interface IRecordHistory {
    IReadOnlyList<TMajorRecord> GetHistory<TMajorRecord>(FormKey formKey)
        where TMajorRecord : IMajorRecord;
    bool Restore<TMajorRecord>(TMajorRecord record)
        where TMajorRecord : IMajorRecord;

    /// <summary>
    /// Undo the last change on a record.
    /// </summary>
    /// <returns>True if the undo was successful, false otherwise.</returns>
    bool Undo();

    bool Redo();
}

class RecordHistory : IRecordHistory {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private readonly IRecordController _recordController;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly List<IRecordState> _history = [];
    private readonly List<IRecordState> _redoHistory = [];
    private readonly GameConstants _gameConstants;

    public RecordHistory(
        IEditorEnvironment editorEnvironment,
        IMutagenTypeProvider mutagenTypeProvider,
        IRecordController recordController,
        IGameReleaseContext gameReleaseContext) {
        _editorEnvironment = editorEnvironment;
        _mutagenTypeProvider = mutagenTypeProvider;
        _recordController = recordController;
        _gameReleaseContext = gameReleaseContext;
        _gameConstants = GameConstants.Get(_gameReleaseContext.Release);

        recordController.RecordCreated
            .Subscribe(r => {
                var stream = new MemoryStream();
                r.Record.WriteToBinary(new MutagenWriter(stream, _gameConstants));
                _history.Add(new RecordCreation(r.Record.ToStandardizedIdentifier(), stream.GetBuffer(), r.Mod, ChangeType.Create));
            });

        recordController.RecordChangedDiff
            .Subscribe(r => {
                return updated => {
                    var oldStream = new MemoryStream();
                    r.Record.WriteToBinary(new MutagenWriter(oldStream, _gameConstants));

                    var newStream = new MemoryStream();
                    updated.Record.WriteToBinary(new MutagenWriter(newStream, _gameConstants));

                    _history.Add(new RecordUpdate(r.Record.ToStandardizedIdentifier(), oldStream.GetBuffer(), newStream.GetBuffer(), r.Mod, ChangeType.Update));
                };
            });

        recordController.RecordDeleted
            .Subscribe(r => {
                var stream = new MemoryStream();
                _history.Add(new RecordCreation(r.Record.ToStandardizedIdentifier(), stream.GetBuffer(), r.Mod, ChangeType.Delete));
            });
    }

    public IReadOnlyList<TMajorRecord> GetHistory<TMajorRecord>(FormKey formKey)
        where TMajorRecord : IMajorRecord {
        throw new NotImplementedException();
    }

    public bool Restore<TMajorRecord>(TMajorRecord record)
        where TMajorRecord : IMajorRecord {
        throw new NotImplementedException();
    }

    public bool Undo() {
        if (_history.Count == 0) return false;

        var last = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        switch (last) {
            case RecordCreation creation:
                break;
            case RecordUpdate update:
                break;
            case RecordDeletion deletion:
                break;
        }

        return false;
    }

    public bool Redo() {
        if (_redoHistory.Count == 0) return false;

        var last = _redoHistory[^1];
        _redoHistory.RemoveAt(_redoHistory.Count - 1);

        switch (last) {
            case RecordCreation(var identifier, var data, var mod, _):
                // Assume form key is not taken by another record in the meantime
                var package = ParseRecord(mod, identifier, data);
                _recordController.CreateRecord(package, mod);
                return true;
            case RecordUpdate(var identifier, var data, var @new, var mod, _):
                var oldRecord = ParseRecord(mod, identifier, data);
                var newRecord = ParseRecord(mod, identifier, @new);
                _recordController.RegisterUpdate(
                    oldRecord,
                    mod,
                    () => oldRecord.DeepCopyIn(newRecord));
                return true;
            case RecordDeletion(var identifier, var data, var mod, _):
                var deleted = ParseRecord(mod, identifier, data);
                _recordController.DeleteRecord(deleted, mod);
                return true;
        }

        return false;
    }

    private Package ParseRecord(IMod mod, IFormLinkIdentifier identifier, byte[] recordData) {
        var recordGetterType = _mutagenTypeProvider.GetRecordGetterType(identifier.Type);
        var masterPackage = SeparatedMasterPackage.Factory(
            _gameReleaseContext.Release,
            mod.ModKey,
            mod.MasterStyle,
            new MasterReferenceCollection(mod.ModKey, mod.MasterReferences),
            new LoadOrder<IModMasterStyledGetter>());
        var parsingMeta = new ParsingMeta(_gameConstants, mod.ModKey, masterPackage);
        return Package.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(recordData, parsingMeta)));
    }
}

internal enum ChangeType {
    Create,
    Update,
    Delete
}

internal interface IRecordState {
    IMod Mod { get; }
    ChangeType ChangeType { get; }
}

internal record RecordCreation(IFormLinkIdentifier Identifier, byte[] Created, IMod Mod, ChangeType ChangeType) : IRecordState;
internal record RecordUpdate(IFormLinkIdentifier Identifier, byte[] Old, byte[] New, IMod Mod, ChangeType ChangeType) : IRecordState;
internal record RecordDeletion(IFormLinkIdentifier Identifier, byte[] Deleted, IMod Mod, ChangeType ChangeType) : IRecordState;
