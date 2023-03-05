using System.Reactive.Linq;
using Autofac;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Extension;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Loqui;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed class RecordTypeListing : ViewModel, IRecordFilterContainer {
    public ILoquiRegistration Registration { get; }

    public string DisplayName { get; }
    public IObservableCollection<RecordFilterListing> RecordFilters { get; }

    public RecordTypeListing(
        IComponentContext componentContext,
        ILoquiRegistration registration,
        string? displayName = null) {
        Registration = registration;
        DisplayName = displayName ?? string.Concat(registration.Name.Select((c, i) => i != 0 && char.IsUpper(c) ? " " + c : c.ToString()));

        var editorEnvironment = componentContext.Resolve<IEditorEnvironment>();

        RecordFilters = editorEnvironment.LinkCacheChanged
            .ObserveOnTaskpool()
            .Select(_ => componentContext.Resolve<IRecordFilterBuilder>()
                .AddRecordType(registration.GetterType)
                .Build()
                .Select(x => {
                    x.Parent = this;
                    return x;
                })
                .AsObservableChangeSet())
            .Switch()
            .AddKey(listing => listing.DisplayName)
            .SortBy(x => x)
            .ToObservableCollection(this);
    }
}
