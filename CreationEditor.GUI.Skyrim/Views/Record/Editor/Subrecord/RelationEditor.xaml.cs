using System;
using System.Collections.Generic;
using System.Windows;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.GUI.Skyrim.Views.Record; 

public partial class RelationEditor : ISubRecordEditorVM<Relation> {
    public static readonly DependencyProperty LinkCacheProperty = DependencyProperty.Register(nameof(LinkCache), typeof(ILinkCache), typeof(RelationEditor), new PropertyMetadata(default(ILinkCache)));
    public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(nameof(Record), typeof(Relation), typeof(RelationEditor), new PropertyMetadata(default(Relation)));

    public static IEnumerable<Type> RelationTypes { get; } = new[] { typeof(IFactionGetter), typeof(IRaceGetter) };
    public static IEnumerable<CombatReaction> CombatReactions { get; } = Enum.GetValues<CombatReaction>();
    
    public ILinkCache LinkCache {
        get => (ILinkCache) GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }
    
    public Relation Record {
        get => (Relation) GetValue(RecordProperty);
        set => SetValue(RecordProperty, value);
    }

    public RelationEditor() {
        InitializeComponent();
    }
}
