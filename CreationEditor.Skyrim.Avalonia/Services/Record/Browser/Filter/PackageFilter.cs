﻿using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class PackageFilter : RecordFilter<IPackageGetter> {
    private readonly IEditorEnvironment _editorEnvironment;

    public PackageFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<IPackageGetter>()
            .Where(package => package.Type == Package.Types.PackageTemplate)
            .Where(template => template.EditorID != null)
            .Select(template => new RecordFilterListing(template.EditorID!, record => record is IPackageGetter package && package.PackageTemplate.FormKey == template.FormKey));
    }
}