using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class PackageFilter : RecordFilter<IPackageGetter> {
    private readonly ILinkCacheProvider _linkCacheProvider;

    public PackageFilter(
        ILinkCacheProvider linkCacheProvider) {
        _linkCacheProvider = linkCacheProvider;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<IPackageGetter>()
            .Where(package => package.Type == Package.Types.PackageTemplate)
            .Where(template => template.EditorID is not null)
            .Select(template => new RecordFilterListing(
                template.EditorID!,
                record =>
                    record is IPackageGetter package
                 && package.PackageTemplate.FormKey == template.FormKey));
    }
}
