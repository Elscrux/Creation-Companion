// using System.ComponentModel;
// using System.Numerics;
// using CreationEditor.Services.Environment;
// using Mutagen.Bethesda.Plugins;
// using Mutagen.Bethesda.Plugins.Cache;
// using Mutagen.Bethesda.Skyrim;
// using Noggog;
// using ReactiveUI;
// namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;
//
// public sealed class EditablePlaced : PlacedObject, INotifyPropertyChanged, IDisposable {
//     private readonly IEditorEnvironment _editorEnvironment;
//     private IDisposableDropoff _disposableDropoff = new DisposableBucket();
//
//     public IObservable<ILinkCache> LinkCacheChanged => _editorEnvironment.LinkCacheChanged;
//
//     public bool IsActor { get; set; }
//
//     public bool IsDoor { get; set; }
//
//     public bool IsPatrol { get; set; }
//
//     public bool IsItem { get; set; }
//
//     public bool IsEnchantedWeapon { get; set; }
//
//     public bool IsLeveledActor { get; set; }
//
//     public bool IsMapMarker { get; set; }
//
//     public bool IsRoomMarker { get; set; }
//
//
// #region Transform
//     [Category("Transform")]
//     [Description("Position of the reference in the world.")]
//     public Vector3 Position { get; set; }
//
//     [Category("Transform")]
//     [Description("Rotation of the reference in the world.")]
//     public Vector3 Rotation { get; set; }
// #endregion
//
// #region Enabled
//     public bool HasEnableParent { get; set; }
//
//     [Category("Enabled State")]
//     [DisplayName("Initially Disabled")]
//     [Description("The reference starts disabled and must be enabled through script or game events before it will appear.")]
//     public bool InitiallyDisabled { get; set; }
//
//     [Category("Enabled State")]
//     [DisplayName("Enable Parent")]
//     [Description("This object's enabled state is synchronized to the enabled state of the enable parent.")]
//     [ScopedTypes(typeof(IPlacedGetter))]
//     public FormKey EnableParent { get; set; }
//
//     [Category("Enable Parent")]
//     [VisibilityPropertyCondition(nameof(HasEnableParent), true)]
//     [DisplayName("Opposite of Parent State")]
//     [Description("This object's enabled state is set to the opposite of the enabled state.")]
//     public bool OppositeOfParentState { get; set; }
// #endregion
//
// #region Room Bounds
//     [Category("Multibound")]
//     [DisplayName("Room Marker")]
//     [ScopedTypes(typeof(IPlacedObjectGetter))]
//     [Description("By default, objects will be rendered in the Room Bound that contains their center point."
//       + " Here, you can specify a Room Bound in which the object should be rendered instead."
//       + " This is useful for especially large objects (such as water planes) and when portalling complex or organic environments.")]
//     public FormKey RoomMarker { get; set; } // todo: filter room marker
//
//     [Category("Multibound")]
//     [VisibilityPropertyCondition(nameof(IsRoomMarker), true)]
//     [DisplayName("Image Space")]
//     [Description("Custom image space for the area of the room marker.")]
//     [ScopedTypes(typeof(IImageSpaceGetter))]
//     public FormKey ImageSpace { get; set; }
//
//     [Category("Multibound")]
//     [VisibilityPropertyCondition(nameof(IsRoomMarker), true)]
//     [DisplayName("Image Space")]
//     [Description("Custom lighting template for the area of the room marker.")]
//     [ScopedTypes(typeof(ILightingTemplateGetter))]
//     public FormKey LightingTemplate { get; set; }
// #endregion
//
// #region Emittance
//     [Category("Emittance")]
//     [DisplayName("Interior Light ")]
//     [Description("The reference starts disabled and must be enabled through script or game events before it will appear.")]
//     [ScopedTypes(typeof(ILightGetter))]
//     public FormKey InteriorLight { get; set; }
//
//     [Category("Emittance")]
//     [DisplayName("Exterior Light")]
//     [Description("This object's enabled state is synchronized to the enabled state of the enable parent.")]
//     [ScopedTypes(typeof(ILightGetter))]
//     public FormKey ExteriorLight { get; set; }
// #endregion
//
// #region Respawning
//     [Category("Respawning")]
//     [Description("If unchecked (when possible) this object will not respawn even if the cell it is in should do so.")]
//     public bool Respawns { get; set; }
// #endregion
//
// #region LOD
//     [Category("LOD")]
//     [DisplayName("Is Full LOD")]
//     [Description("Reference will not fade from a distance. Good to apply to very large objects or lighting where fading out would feel obvious and awkward. Warning: Can impact performance if over-used.")]
//     public bool IsFullLOD { get; set; }
//
//     [Category("LOD")]
//     [DisplayName("High Priority LOD")]
//     [Description("Brings reference to the front of the LOD loading hierarchy. Object will fade out last and fade in first.")]
//     public bool HighPriorityLOD { get; set; }
// #endregion
//
// #region Actor
//     public bool StartsDead { get; set; }
//     public FormKey Horse { get; set; }
//     public FormKey PersistenceLocation { get; set; }
// #endregion
//
// #region AI
//     [Category("AI")]
//     [DisplayName("Ignored by Sandbox")]
//     [Description("Check this box to prevent sandboxing NPCs from attempting to use this reference. Useful for furniture and idle markers.")]
//     public bool IgnoredBySandbox { get; set; }
// #endregion
//
// #region Physics
//     [Category("Physics")]
//     [DisplayName("Don't Havok Settle")]
//     [Description("This object, if havokable, doesn't initially settle itself when the cell is finished loading. Note that if any objects near the object settle, this object will settle regardless of this flag. (For Arrows in targets, etc...)")]
//     public bool DontHavokSettle { get; set; }
// #endregion
//
// #region Door
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [Description("If checked, the door will be inoperable and will show 'Inaccessible' when rolled over. This overrides any lock level setting on the door.")]
//     public bool Inaccessible { get; set; }
//
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [DisplayName("Open by Default")]
//     [Description("If checked, the door or object will begin in the open state.")]
//     public bool OpenByDefault { get; set; }
//
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [DisplayName("Lock Level")]
//     [Description("The lock level of the door.")]
//     public LockLevel LockLevel { get; set; }
//
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [ScopedTypes(typeof(IKeyGetter))]
//     [Description("The key that unlocks the door.")]
//     public FormKey Key { get; set; }
//
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [DisplayName("Leveled Lock")]
//     [Description("If checked, the lock will be a leveled lock. The lock level will be determined by the player's level. Leveled Locks are not normally used in Skyrim.")]
//     public bool LeveledLock { get; set; }
//
//     [Category("Door")]
//     [VisibilityPropertyCondition(nameof(IsDoor), true)]
//     [ScopedTypes(typeof(IPlacedObjectGetter))]
//     [Description("The door or object that this door will teleport to.")]
//     public FormKey LinkedDoor { get; set; }
// #endregion
//
// #region Encounter Zone
//     [Category("Encounter Zone")]
//     [DisplayName("Encounter Zone")]
//     [ScopedTypes(typeof(IEncounterZoneGetter))]
//     [Description("The Encounter Zone is this reference is attached to."
//       + " If an encounter zone is not explicitly specified, the object will inherit the Encounter Zone of its Cell, if any."
//       + " The Encounter Zone is primarily used by Leveled Lists in determining the strength of their creatures or objects.")]
//     public FormKey EncounterZone { get; set; }
// #endregion
//
// #region Navmesh
//     [Category("Navmesh")]
//     public bool IsBoundingBox { get; set; }
//
//     [Category("Navmesh")]
//     public bool IsFilter { get; set; }
// #endregion
//
// #region Patrol
//     [Category("Patrol")]
//     [VisibilityPropertyCondition(nameof(IsPatrol), true)]
//     [DisplayName("Idle Time")]
//     [Description("Amount of time in seconds that the actor will spend at this object before moving on.")]
//     public bool IdleTime { get; set; }
//
//     [Category("Patrol")]
//     [VisibilityPropertyCondition(nameof(IsPatrol), true)]
//     [ScopedTypes(typeof(IDialogTopicGetter))]
//     [Description("Dialog topic said by the actor when they reach this object.")]
//     public FormKey Topic { get; set; }
// #endregion
//
// #region Location Ref Type
//     [Category("Location Ref Type")]
//     [DisplayName("Location Ref Type")]
//     [ScopedTypes(typeof(ILocationReferenceTypeGetter))]
//     [Description("The Location Reference Type of this object.")]
//     public FormKey LocationRefType { get; set; }
// #endregion
//
// #region Scripts
//     [Category("Scripts")]
//     public BindingList<ScriptEntry> Scripts { get; set; } = new();
// #endregion
//
// #region Ownership
//     [Category("Ownership")]
//     [VisibilityPropertyCondition(nameof(IsItem), true)]
//     [ScopedTypes(typeof(INpcGetter), typeof(IFactionGetter))]
//     [DisplayName("Persistence Location")]
//     [Description("The owner of the item. If the owner is a faction, the item will be owned by all members of the faction."
//       + "If no owner is specified, the object will be owned by the Actor or Faction that owns its Cell, if any.")]
//     public FormKey Owner { get; set; }
//
//     public bool OwnerIsFaction { get; set; }
//
//     [Category("Ownership")]
//     [VisibilityPropertyCondition(nameof(IsItem), true)]
//     [DisplayName("Required Rank")]
//     [Description("The minimum rank in the Faction needed for someone to own the object.")]
//     public int RequiredRank { get; set; } // todo: parse faction ranks in list of strings + ids, set id in the data
// #endregion
//
// #region Count
//     [Category("Ownership")]
//     [VisibilityPropertyCondition(nameof(IsItem), true)]
//     [DisplayName("Item Count")]
//     [Description("The number of objects represented by the reference. Like when the player drops multiple items of the same type, only one physical object will be shown in the world, unless it is an arrow.")]
//     public int ItemCount { get; set; }
// #endregion
//
// #region Charge
//     [Category("Charge")]
//     [VisibilityPropertyCondition(nameof(IsEnchantedWeapon), true)]
//     [DisplayName("Charge Level")]
//     [Description("The amount of Charge the weapon begins with. If not specified, the object will begin fully charged.")]
//     public int ChargeLevel { get; set; }
// #endregion
//
// #region Leveled Actor
//     [Category("Leveled Actor")]
//     [VisibilityPropertyCondition(nameof(IsLeveledActor), true)]
//     [Description("The level sets the relative difficulty of this particular actor based on the actor's Leveled Character as well as the encounter zone.")]
//     public Level Level { get; set; } // todo: add "None" level
// #endregion
//
// #region Map Marker
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(IsMapMarker), true)]
//     [DisplayName("Marker Name")]
//     [Description("The name of the map marker as it appears on the map.")]
//     public string MarkerName { get; set; } = string.Empty;
//
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(IsMapMarker), true)]
//     [Description("The type of map marker. The type determines the icon that appears on the map.")]
//     public int Type { get; set; }
//
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(IsMapMarker), true)]
//     [Description("The radius of the map marker. The radius determines the circle around the marker object in which the marker will be discovered.")]
//     public float Radius { get; set; }
//
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(IsMapMarker), true)]
//     [Description("If checked, the map marker will be visible on the map by game start as undiscovered.")]
//     public bool Visible { get; set; }
//
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(Visible), true)]
//     [DisplayName("Can Travel To")]
//     [Description("If checked, the map marker will be visible on the map by game start as discovered.")]
//     public bool CanTravelTo { get; set; }
//
//     [Category("Map Marker")]
//     [VisibilityPropertyCondition(nameof(IsMapMarker), true)]
//     [DisplayName("Show All Hidden")]
//     [Description("If checked, the map marker won't be visible when the 'Show All Map Markers' function is called.")]
//     public bool ShowAllHidden { get; set; }
// #endregion
//
// #region Random
//     [Category("Random")]
//     [DisplayName("Hidden From Local Map")]
//     [Description("If checked, the object won't be visible on the local map.")]
//     public bool HiddenFromLocalMap { get; set; }
//
//     [Category("Random")]
//     [DisplayName("Alpha Cutoff")]
//     [Description("For some objects, a numeric value used to determine how much of the object should be shown or hidden. This value is most often used for banners and carpets, allowing them to appear more or less tattered.")]
//     public bool AlphaCutoff { get; set; }
//
//     [Category("Random")]
//     [DisplayName("Sky Marker")]
//     [Description("Flags this marker as a point in the air that can be used in a Dragon's flight path.")]
//     public bool SkyMarker { get; set; } //todo: just for xMarkerHeadings??
// #endregion
//
// #region Linked Reference
//     [Category("Linked Reference")]
//     [DisplayName("Linked References")]
//     [Description("References can be connected to other references through a system of Linked Refs. Each Reference can have only one Linked Ref per keyword.")]
//     public BindingList<LinkedReference> LinkedRefs { get; set; } = new(); // todo: Ensure only one linked ref per keyword 
//
//     [Category("Linked Reference")]
//     public LinkedReference LinkedReference { get; set; } = new();
//     [Category("Linked Reference")]
//     public Vector3 Vector3 { get; set; } = new();
//     [Category("Linked Reference")]
//     public BindingList<Vector3> Vector3s { get; set; } = new(); // todo: Ensure only one linked ref per keyword 
//     [Category("Linked Reference")]
//     [ScopedTypes(typeof(IKeywordGetter))]
//     public BindingList<FormKey> Vector3sxX { get; set; } = new(); // todo: Ensure only one linked ref per keyword 
// #endregion
//
//     public EditablePlaced(IPlaced placed, IEditorEnvironment editorEnvironment) {
//         _editorEnvironment = editorEnvironment;
//
//         if (placed.Placement != null) {
//             Position = new Vector3(placed.Placement.Position.X, placed.Placement.Position.Y, placed.Placement.Position.Z);
//             Rotation = new Vector3(placed.Placement.Rotation.X, placed.Placement.Rotation.Y, placed.Placement.Rotation.Z);
//         } else {
//             Position = new Vector3();
//             Rotation = new Vector3();
//         }
//
//         this.WhenAnyValue(x => x.EnableParent)
//             .Subscribe(x => HasEnableParent = !x.IsNull)
//             .DisposeWith(_disposableDropoff);
//
//         switch (placed) {
//             case PlacedNpc placedNpc:
//                 IsActor = true;
//
//                 var npc = placedNpc.Base.TryResolve(_editorEnvironment.LinkCache);
//                 if (npc != null) {
//                     while (!IsLeveledActor) {
//                         var npcSpawn = npc.Template.TryResolve(_editorEnvironment.LinkCache);
//                         if (npcSpawn is null) break;
//
//                         switch (npcSpawn) {
//                             case INpcGetter npcGetter:
//                                 npc = npcGetter;
//                                 break;
//                             case ILeveledNpcGetter:
//                                 IsLeveledActor = true;
//                                 break;
//                         }
//                     }
//                 }
//
//                 break;
//             case PlacedObject placedObject:
//                 var baseObject = placedObject.Base.TryResolve(_editorEnvironment.LinkCache);
//                 if (baseObject is null) {
//                     switch (baseObject) {
//                         case IDoorGetter door:
//                             IsDoor = true;
//                             break;
//                         case IFurnitureGetter:
//                         case IIdleMarkerGetter:
//                             IsPatrol = true;
//                             break;
//                         case IItemGetter item: {
//                             IsItem = true;
//
//                             if (baseObject is IWeaponGetter weapon) {
//                                 if (!weapon.ObjectEffect.IsNull) {
//                                     IsEnchantedWeapon = true;
//                                 }
//                             }
//                             break;
//                         }
//                     }
//
//                     if (baseObject?.FormKey == Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.Static.MapMarker.FormKey) {
//                         IsMapMarker = true;
//                     }
//                 }
//                 break;
//         }
//     }
//
//     public void CopyTo(IPlaced record) {
//     }
//
//     public event PropertyChangedEventHandler? PropertyChanged;
//     public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//
//     public void Dispose() => _disposableDropoff.Dispose();
// }
