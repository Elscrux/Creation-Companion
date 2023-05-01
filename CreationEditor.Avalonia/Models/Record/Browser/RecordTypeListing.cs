using System.Reactive.Linq;
using System.Reactive.Subjects;
using Autofac;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Loqui;
using Noggog;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed class RecordTypeListing : ViewModel, IRecordFilterContainer {
    public ILoquiRegistration Registration { get; }

    public string DisplayName { get; }
    public IObservableCollection<RecordFilterListing> RecordFilters { get; }

    private readonly BehaviorSubject<bool> _activated = new(false);

    public RecordTypeListing(
        ILifetimeScope lifetimeScope,
        ILoquiRegistration registration,
        string? displayName = null) {
        Registration = registration;
        DisplayName = displayName ?? string.Concat(registration.Name.Select((c, i) => i != 0 && char.IsUpper(c) ? " " + c : c.ToString()));

        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        var editorEnvironment = newScope.Resolve<IEditorEnvironment>();

        RecordFilters = editorEnvironment.LoadOrderChanged.CombineLatest(_activated, (_, b) => b)
            .ObserveOnTaskpool()
            .Where(active => active)
            .Select(_ => newScope.Resolve<IRecordFilterBuilder>()
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

    public void Activate() {
        _activated.OnNext(true);
    }
}
