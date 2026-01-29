using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using DynamicData.Binding;
using ModCleaner.Models.FeatureFlag;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace ModCleaner.ViewModels;

public sealed partial class ReactiveFormKey : ReactiveObject {
    [Reactive] public partial FormKey FormKey { get; set; }
}

public sealed partial class ReactiveWorldspaceRegions : ReactiveObject {
    [Reactive] public partial FormKey Worldspace { get; set; }
    public IObservableCollection<ReactiveFormKey> Regions { get; init; } = new ObservableCollectionExtended<ReactiveFormKey>();
}

public sealed partial class FeatureFlagEditorVM : ViewModel {
    [Reactive] public partial string Name { get; set; }

    public ModKey ModKey { get; }
    public SingleModPickerVM ModPickerVM { get; }
    public IObservableCollection<IQuestGetter> EssentialQuests { get; }
    public IObservableCollection<IQuestGetter> UnusedQuests { get; }
    public IObservableCollection<ILoadScreenGetter> EssentialLoadScreens { get; }
    public IObservableCollection<ILoadScreenGetter> UnusedLoadScreens { get; }
    public IObservableCollection<IIdleAnimationGetter> EssentialIdleAnimations { get; }
    public IObservableCollection<IIdleAnimationGetter> UnusedIdleAnimations { get; }
    public IObservableCollection<ReactiveWorldspaceRegions> AllowedRegions { get; }
    public ILinkCacheProvider LinkCacheProvider { get; }

    public FeatureFlagEditorVM(
        FeatureFlag featureFlag,
        SingleModPickerVM modPickerVM,
        ILinkCacheProvider linkCacheProvider) {
        ModPickerVM = modPickerVM;
        LinkCacheProvider = linkCacheProvider;
        Name = featureFlag.Name;
        ModKey = featureFlag.ModKey;
        ModPickerVM.SelectedMod = ModPickerVM.Mods.FirstOrDefault(m => m.ModKey == featureFlag.ModKey);

        var mod = LinkCacheProvider.LinkCache.ResolveMod(ModPickerVM.SelectedMod?.ModKey);

        EssentialQuests = GetEssentialRecords<IQuestGetter>();
        UnusedQuests = GetUnusedRecords(EssentialQuests, mod);

        EssentialLoadScreens = GetEssentialRecords<ILoadScreenGetter>();
        UnusedLoadScreens = GetUnusedRecords(EssentialLoadScreens, mod);

        EssentialIdleAnimations = GetEssentialRecords<IIdleAnimationGetter>();
        UnusedIdleAnimations = GetUnusedRecords(EssentialIdleAnimations, mod);

        AllowedRegions = new ObservableCollectionExtended<ReactiveWorldspaceRegions>(
            featureFlag.AllowedRegions.Select(wr => new ReactiveWorldspaceRegions {
                Worldspace = wr.Worldspace.FormKey,
                Regions = new ObservableCollectionExtended<ReactiveFormKey>(
                    wr.Regions.Select(r => new ReactiveFormKey { FormKey = r.FormKey })),
            })
        );

        this.WhenAnyValue(x => x.ModPickerVM.SelectedMod)
            .Subscribe(selectedMod => {
                if (selectedMod is { ModKey: var modKey }) {
                    var newMod = LinkCacheProvider.LinkCache.ResolveMod(modKey);
                    UnusedQuests.ReplaceWith(GetUnusedRecords(EssentialQuests, newMod));
                    UnusedLoadScreens.ReplaceWith(GetUnusedRecords(EssentialLoadScreens, newMod));
                    UnusedIdleAnimations.ReplaceWith(GetUnusedRecords(EssentialIdleAnimations, newMod));
                } else {
                    UnusedQuests.ReplaceWith([]);
                    UnusedLoadScreens.ReplaceWith([]);
                    UnusedIdleAnimations.ReplaceWith([]);
                }
            })
            .DisposeWith(this);

        ObservableCollectionExtended<T> GetEssentialRecords<T>()
            where T : class, IMajorRecordQueryableGetter => new(
            featureFlag.EssentialRecords
                .Select(link => LinkCacheProvider.LinkCache.TryResolve<T>(link.FormKey, out var r) ? r : null)
                .WhereNotNull());

        IObservableCollection<T> GetUnusedRecords<T>(IObservableCollection<T> essentialRecords, IModGetter? currentMod)
            where T : class, IMajorRecordGetter {
            if (currentMod is null) return new ObservableCollectionExtended<T>();

            return new ObservableCollectionExtended<T>(currentMod.EnumerateMajorRecords<T>()
                .Where(q => !essentialRecords.Contains(q))
                .OrderBy(q => q.EditorID)
            );
        }
    }

    public FeatureFlag GetFeatureFlag() {
        return new FeatureFlag(
            Name,
            ModPickerVM.SelectedMod?.ModKey ?? ModKey,
            AllowedRegions.Select(wr => new WorldspaceRegions(
                new FormLinkGetter<IWorldspaceGetter>(wr.Worldspace),
                wr.Regions.Select(IFormLinkGetter<IRegionGetter> (r) => new FormLink<IRegionGetter>(r.FormKey)).ToList()
            )).ToList(),
            EssentialQuests
                .Select(FormLinkInformation (r) => new FormLinkInformation(r.FormKey, typeof(ISkyrimMajorRecordGetter)))
                .Concat(EssentialLoadScreens
                    .Select(FormLinkInformation (r) => new FormLinkInformation(r.FormKey, typeof(ISkyrimMajorRecordGetter))))
                .Concat(EssentialIdleAnimations
                    .Select(FormLinkInformation (r) => new FormLinkInformation(r.FormKey, typeof(ISkyrimMajorRecordGetter))))
                .ToList());
    }
}
