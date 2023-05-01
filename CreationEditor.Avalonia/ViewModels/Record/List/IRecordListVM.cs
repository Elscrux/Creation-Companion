using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposableDropoff {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }

    public IList<IMenuItem> ContextMenuItems { get; }

    public IObservable<bool> IsBusy { get; }
    public Func<StyledElement, IFormLinkIdentifier> GetFormLink { get; }
}
