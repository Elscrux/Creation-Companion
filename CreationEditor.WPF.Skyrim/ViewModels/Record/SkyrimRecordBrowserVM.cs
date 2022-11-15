using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Skyrim.ViewModels.Record;

public class SkyrimRecordBrowserVM : ReactiveObject, IRecordBrowserVM {
    private readonly IRecordListFactory _recordListFactory;
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }

    [Reactive] public IRecordListVM? RecordList { get; set; }
    

    public SkyrimRecordBrowserVM(
        IRecordListFactory recordListFactory,
        IRecordBrowserSettings recordBrowserSettings) {
        _recordListFactory = recordListFactory;
        RecordBrowserSettings = recordBrowserSettings;

        SelectRecordType = ReactiveCommand.Create((RecordTypeListing recordTypeListing) => {
            var recordType = recordTypeListing.Registration.GetterType;
            if (RecordList != null && RecordList.Type == recordType) return;

            RecordList = _recordListFactory.FromType(recordType, RecordBrowserSettings);
        });
        
        RecordTypeGroups = new ObservableCollection<RecordTypeGroup> {
            new("Actors",
                new ObservableCollection<RecordTypeListing> {
                    new(INpcGetter.StaticRegistration),
                    new(IActionRecordGetter.StaticRegistration),
                    new(IBodyPartDataGetter.StaticRegistration),
                    new(ILeveledNpcGetter.StaticRegistration),
                    new(IPerkGetter.StaticRegistration),
                    new(ITalkingActivatorGetter.StaticRegistration),
                }),
            new("Audio",
                new ObservableCollection<RecordTypeListing> {
                    new(IAcousticSpaceGetter.StaticRegistration),
                    new(IMusicTrackGetter.StaticRegistration),
                    new(IMusicTypeGetter.StaticRegistration),
                    new(IReverbParametersGetter.StaticRegistration),
                    new(ISoundCategoryGetter.StaticRegistration),
                    new(ISoundDescriptorGetter.StaticRegistration),
                    new(ISoundMarkerGetter.StaticRegistration),
                    new(ISoundOutputModelGetter.StaticRegistration),
                }),
            new("Character",
                new ObservableCollection<RecordTypeListing> {
                    new(IAssociationTypeGetter.StaticRegistration),
                    new(IClassGetter.StaticRegistration),
                    new(IEquipTypeGetter.StaticRegistration),
                    new(IFactionGetter.StaticRegistration),
                    new(IHeadPartGetter.StaticRegistration),
                    new(IMovementTypeGetter.StaticRegistration),
                    new(IPackageGetter.StaticRegistration),
                    new(IQuestGetter.StaticRegistration),
                    new(IRaceGetter.StaticRegistration),
                    new(IRelationshipGetter.StaticRegistration),
                    new(IStoryManagerEventNodeGetter.StaticRegistration),
                    new(IVoiceTypeGetter.StaticRegistration),
                }),
            new("Items",
                new ObservableCollection<RecordTypeListing> {
                    new(IAmmunitionGetter.StaticRegistration),
                    new(IArmorGetter.StaticRegistration),
                    new(IArmorAddonGetter.StaticRegistration),
                    new(IBookGetter.StaticRegistration),
                    new(IConstructibleObjectGetter.StaticRegistration),
                    new(IIngredientGetter.StaticRegistration),
                    new(IKeyGetter.StaticRegistration),
                    new(ILeveledItemGetter.StaticRegistration),
                    new(IMiscItemGetter.StaticRegistration),
                    new(IOutfitGetter.StaticRegistration),
                    new(ISoulGemGetter.StaticRegistration),
                    new(IWeaponGetter.StaticRegistration),
                }),
            new("Magic",
                new ObservableCollection<RecordTypeListing> {
                    new(IDualCastDataGetter.StaticRegistration),
                    new(IObjectEffectGetter.StaticRegistration),
                    new(ILeveledSpellGetter.StaticRegistration),
                    new(IMagicEffectGetter.StaticRegistration),
                    new(IIngestibleGetter.StaticRegistration),
                    new(IScrollGetter.StaticRegistration),
                    new(ISpellGetter.StaticRegistration),
                    new(IWordOfPowerGetter.StaticRegistration),
                }),
            new("Miscellaneous",
                new ObservableCollection<RecordTypeListing> {
                    new(IAnimatedObjectGetter.StaticRegistration),
                    new(IArtObjectGetter.StaticRegistration),
                    new(ICollisionLayerGetter.StaticRegistration),
                    new(IFormListGetter.StaticRegistration),
                    new(IGlobalGetter.StaticRegistration),
                    new(IIdleMarkerGetter.StaticRegistration),
                    new(IKeywordGetter.StaticRegistration),
                    new(ILandscapeTextureGetter.StaticRegistration),
                    new(ILoadScreenGetter.StaticRegistration),
                    new(IMaterialObjectGetter.StaticRegistration),
                    new(IMessageGetter.StaticRegistration),
                    new(ITextureSetGetter.StaticRegistration),
                }),
            new("Special Effects",
                new ObservableCollection<RecordTypeListing> {
                    new(IAddonNodeGetter.StaticRegistration),
                    new(ICameraShotGetter.StaticRegistration),
                    new(IDebrisGetter.StaticRegistration),
                    new(IEffectShaderGetter.StaticRegistration),
                    new(IExplosionGetter.StaticRegistration),
                    new(IFootstepGetter.StaticRegistration),
                    new(IFootstepSetGetter.StaticRegistration),
                    new(IHazardGetter.StaticRegistration),
                    new(IImageSpaceGetter.StaticRegistration),
                    new(IImageSpaceAdapterGetter.StaticRegistration),
                    new(IImpactDataSetGetter.StaticRegistration),
                    new(ILensFlareGetter.StaticRegistration),
                    new(IMaterialTypeGetter.StaticRegistration),
                    new(IProjectileGetter.StaticRegistration),
                    new(IVolumetricLightingGetter.StaticRegistration),
                }),
            new("World Data",
                new ObservableCollection<RecordTypeListing> {
                    new(IClimateGetter.StaticRegistration),
                    new(IEncounterZoneGetter.StaticRegistration),
                    new(ILightingTemplateGetter.StaticRegistration),
                    new(ILocationGetter.StaticRegistration),
                    new(ILocationReferenceTypeGetter.StaticRegistration),
                    new(IShaderParticleGeometryGetter.StaticRegistration),
                    new(IVisualEffectGetter.StaticRegistration),
                    new(IWaterGetter.StaticRegistration),
                    new(IWeatherGetter.StaticRegistration),
                }),
            new("World Objects",
                new ObservableCollection<RecordTypeListing> {
                    new(IActivatorGetter.StaticRegistration),
                    new(IContainerGetter.StaticRegistration),
                    new(IDoorGetter.StaticRegistration),
                    new(IFloraGetter.StaticRegistration),
                    new(IFurnitureGetter.StaticRegistration),
                    new(IGrassGetter.StaticRegistration),
                    new(ILightGetter.StaticRegistration),
                    new(IMoveableStaticGetter.StaticRegistration),
                    new(IStaticGetter.StaticRegistration),
                    new(ITreeGetter.StaticRegistration),
                }),
        };
    }
}
