﻿using CreationEditor.Services.Environment;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Where;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Avalonia.ViewModels.Query;

public sealed class QueryVM : ViewModel {
    private readonly IEditorEnvironment _editorEnvironment;

    public IQueryRunner QueryRunner { get; }
    public IQueryConditionEntryFactory ConditionEntryFactory { get; }
    public IObservable<ILinkCache> LinkCacheChanged => _editorEnvironment.LinkCacheChanged;

    public QueryVM(
        IEditorEnvironment editorEnvironment,
        IQueryRunner queryRunner,
        IQueryConditionEntryFactory conditionEntryFactory) {
        _editorEnvironment = editorEnvironment;
        QueryRunner = queryRunner;
        ConditionEntryFactory = conditionEntryFactory;
    }
}