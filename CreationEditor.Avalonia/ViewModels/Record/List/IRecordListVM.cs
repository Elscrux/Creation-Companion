using System.Collections;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposable {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }

    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }

    public IObservable<bool> IsBusy { get; set; }
    public Func<StyledElement, IFormLinkIdentifier> GetFormLink { get; }
}
