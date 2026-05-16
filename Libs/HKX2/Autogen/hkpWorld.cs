using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpWorld Signatire: 0xaadcec37 size: 1072 flags: FLAGS_NOT_SERIALIZABLE

    // m_simulation m_class: hkpSimulation Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_gravity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_fixedIsland m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_fixedRigidBody m_class: hkpRigidBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_activeSimulationIslands m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 64 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_inactiveSimulationIslands m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_dirtySimulationIslands m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_maintenanceMgr m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_memoryWatchDog m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 120 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_assertOnRunningOutOfSolverMemory m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_broadPhase m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 136 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_kdTreeManager m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 144 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_autoUpdateTree m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_broadPhaseDispatcher m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 160 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_phantomBroadPhaseListener m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 168 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_entityEntityBroadPhaseListener m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 176 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_broadPhaseBorderListener m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 184 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_multithreadedSimulationJobData m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 192 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_collisionInput m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 200 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_collisionFilter m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 208 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_collisionDispatcher m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 216 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_convexListFilter m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pendingOperations m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 232 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pendingOperationsCount m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    // m_pendingBodyOperationsCount m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 244 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_criticalOperationsLockCount m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 248 flags: FLAGS_NONE enum: 
    // m_criticalOperationsLockCountForPhantoms m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 252 flags: FLAGS_NONE enum: 
    // m_blockExecutingPendingOperations m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 256 flags: FLAGS_NONE enum: 
    // m_criticalOperationsAllowed m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 257 flags: FLAGS_NONE enum: 
    // m_pendingOperationQueues m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 264 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pendingOperationQueueCount m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 272 flags: FLAGS_NONE enum: 
    // m_multiThreadCheck m_class: hkMultiThreadCheck Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 276 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_processActionsInSingleThread m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 288 flags: FLAGS_NONE enum: 
    // m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 289 flags: FLAGS_NONE enum: 
    // m_minDesiredIslandSize m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 292 flags: FLAGS_NONE enum: 
    // m_modifyConstraintCriticalSection m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 296 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isLocked m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 304 flags: FLAGS_NONE enum: 
    // m_islandDirtyListCriticalSection m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 312 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_propertyMasterLock m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 320 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_wantSimulationIslands m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 328 flags: FLAGS_NONE enum: 
    // m_useHybridBroadphase m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 329 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_snapCollisionToConvexEdgeThreshold m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 332 flags: FLAGS_NONE enum: 
    // m_snapCollisionToConcaveEdgeThreshold m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 336 flags: FLAGS_NONE enum: 
    // m_enableToiWeldRejection m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 340 flags: FLAGS_NONE enum: 
    // m_wantDeactivation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 341 flags: FLAGS_NONE enum: 
    // m_shouldActivateOnRigidBodyTransformChange m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 342 flags: FLAGS_NONE enum: 
    // m_deactivationReferenceDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 344 flags: FLAGS_NONE enum: 
    // m_toiCollisionResponseRotateNormal m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 348 flags: FLAGS_NONE enum: 
    // m_maxSectorsPerMidphaseCollideTask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 352 flags: FLAGS_NONE enum: 
    // m_maxSectorsPerNarrowphaseCollideTask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 356 flags: FLAGS_NONE enum: 
    // m_processToisMultithreaded m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 360 flags: FLAGS_NONE enum: 
    // m_maxEntriesPerToiMidphaseCollideTask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 364 flags: FLAGS_NONE enum: 
    // m_maxEntriesPerToiNarrowphaseCollideTask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 368 flags: FLAGS_NONE enum: 
    // m_maxNumToiCollisionPairsSinglethreaded m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 372 flags: FLAGS_NONE enum: 
    // m_simulationType m_class:  Type.TYPE_ENUM Type.TYPE_INT32 arrSize: 0 offset: 376 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_numToisTillAllowedPenetrationSimplifiedToi m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 380 flags: FLAGS_NONE enum: 
    // m_numToisTillAllowedPenetrationToi m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 384 flags: FLAGS_NONE enum: 
    // m_numToisTillAllowedPenetrationToiHigher m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 388 flags: FLAGS_NONE enum: 
    // m_numToisTillAllowedPenetrationToiForced m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 392 flags: FLAGS_NONE enum: 
    // m_lastEntityUid m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 396 flags: FLAGS_NONE enum: 
    // m_lastIslandUid m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 400 flags: FLAGS_NONE enum: 
    // m_lastConstraintUid m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 404 flags: FLAGS_NONE enum: 
    // m_phantoms m_class: hkpPhantom Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 408 flags: FLAGS_NONE enum: 
    // m_actionListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 424 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_entityListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 440 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_phantomListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 456 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_constraintListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 472 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_worldDeletionListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 488 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_islandActivationListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 504 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_worldPostSimulationListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 520 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_worldPostIntegrateListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 536 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_worldPostCollideListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 552 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_islandPostIntegrateListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 568 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_islandPostCollideListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 584 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_contactListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 600 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_contactImpulseLimitBreachedListeners m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 616 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_worldExtensions m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 632 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_violatedConstraintArray m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 648 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_broadPhaseBorder m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 656 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_destructionWorld m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 664 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_npWorld m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 672 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_broadPhaseExtents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 2 offset: 1008 flags: FLAGS_NONE enum: 
    // m_broadPhaseNumMarkers m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 1040 flags: FLAGS_NONE enum: 
    // m_sizeOfToiEventQueue m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 1044 flags: FLAGS_NONE enum: 
    // m_broadPhaseQuerySize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 1048 flags: FLAGS_NONE enum: 
    // m_broadPhaseUpdateSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 1052 flags: FLAGS_NONE enum: 
    // m_contactPointGeneration m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 1056 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpWorld : hkReferencedObject, IEquatable<hkpWorld?>
    {
        public hkpSimulation? m_simulation { set; get; }
        public Vector4 m_gravity { set; get; }
        private object? m_fixedIsland { set; get; }
        public hkpRigidBody? m_fixedRigidBody { set; get; }
        public IList<object> m_activeSimulationIslands { set; get; } = Array.Empty<object>();
        public IList<object> m_inactiveSimulationIslands { set; get; } = Array.Empty<object>();
        public IList<object> m_dirtySimulationIslands { set; get; } = Array.Empty<object>();
        private object? m_maintenanceMgr { set; get; }
        private object? m_memoryWatchDog { set; get; }
        private bool m_assertOnRunningOutOfSolverMemory { set; get; }
        private object? m_broadPhase { set; get; }
        private object? m_kdTreeManager { set; get; }
        public bool m_autoUpdateTree { set; get; }
        private object? m_broadPhaseDispatcher { set; get; }
        private object? m_phantomBroadPhaseListener { set; get; }
        private object? m_entityEntityBroadPhaseListener { set; get; }
        private object? m_broadPhaseBorderListener { set; get; }
        private object? m_multithreadedSimulationJobData { set; get; }
        private object? m_collisionInput { set; get; }
        private object? m_collisionFilter { set; get; }
        private object? m_collisionDispatcher { set; get; }
        private object? m_convexListFilter { set; get; }
        private object? m_pendingOperations { set; get; }
        public int m_pendingOperationsCount { set; get; }
        private int m_pendingBodyOperationsCount { set; get; }
        public int m_criticalOperationsLockCount { set; get; }
        public int m_criticalOperationsLockCountForPhantoms { set; get; }
        public bool m_blockExecutingPendingOperations { set; get; }
        public bool m_criticalOperationsAllowed { set; get; }
        private object? m_pendingOperationQueues { set; get; }
        public int m_pendingOperationQueueCount { set; get; }
        public hkMultiThreadCheck m_multiThreadCheck { set; get; } = new();
        public bool m_processActionsInSingleThread { set; get; }
        public bool m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob { set; get; }
        public uint m_minDesiredIslandSize { set; get; }
        private object? m_modifyConstraintCriticalSection { set; get; }
        public int m_isLocked { set; get; }
        private object? m_islandDirtyListCriticalSection { set; get; }
        private object? m_propertyMasterLock { set; get; }
        public bool m_wantSimulationIslands { set; get; }
        private bool m_useHybridBroadphase { set; get; }
        public float m_snapCollisionToConvexEdgeThreshold { set; get; }
        public float m_snapCollisionToConcaveEdgeThreshold { set; get; }
        public bool m_enableToiWeldRejection { set; get; }
        public bool m_wantDeactivation { set; get; }
        public bool m_shouldActivateOnRigidBodyTransformChange { set; get; }
        public float m_deactivationReferenceDistance { set; get; }
        public float m_toiCollisionResponseRotateNormal { set; get; }
        public int m_maxSectorsPerMidphaseCollideTask { set; get; }
        public int m_maxSectorsPerNarrowphaseCollideTask { set; get; }
        public bool m_processToisMultithreaded { set; get; }
        public int m_maxEntriesPerToiMidphaseCollideTask { set; get; }
        public int m_maxEntriesPerToiNarrowphaseCollideTask { set; get; }
        public int m_maxNumToiCollisionPairsSinglethreaded { set; get; }
        private int m_simulationType { set; get; }
        public float m_numToisTillAllowedPenetrationSimplifiedToi { set; get; }
        public float m_numToisTillAllowedPenetrationToi { set; get; }
        public float m_numToisTillAllowedPenetrationToiHigher { set; get; }
        public float m_numToisTillAllowedPenetrationToiForced { set; get; }
        public uint m_lastEntityUid { set; get; }
        public uint m_lastIslandUid { set; get; }
        public uint m_lastConstraintUid { set; get; }
        public IList<hkpPhantom> m_phantoms { set; get; } = Array.Empty<hkpPhantom>();
        public IList<object> m_actionListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_entityListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_phantomListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_constraintListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_worldDeletionListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_islandActivationListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_worldPostSimulationListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_worldPostIntegrateListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_worldPostCollideListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_islandPostIntegrateListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_islandPostCollideListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_contactListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_contactImpulseLimitBreachedListeners { set; get; } = Array.Empty<object>();
        public IList<object> m_worldExtensions { set; get; } = Array.Empty<object>();
        private object? m_violatedConstraintArray { set; get; }
        private object? m_broadPhaseBorder { set; get; }
        private object? m_destructionWorld { set; get; }
        private object? m_npWorld { set; get; }
        public Vector4[] m_broadPhaseExtents = new Vector4[2];
        public int m_broadPhaseNumMarkers { set; get; }
        public int m_sizeOfToiEventQueue { set; get; }
        public int m_broadPhaseQuerySize { set; get; }
        public int m_broadPhaseUpdateSize { set; get; }
        private sbyte m_contactPointGeneration { set; get; }

        public override uint Signature { set; get; } = 0xaadcec37;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_simulation = des.ReadClassPointer<hkpSimulation>(br);
            br.Position += 8;
            m_gravity = br.ReadVector4();
            des.ReadEmptyPointer(br);
            m_fixedRigidBody = des.ReadClassPointer<hkpRigidBody>(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_assertOnRunningOutOfSolverMemory = br.ReadBoolean();
            br.Position += 7;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_autoUpdateTree = br.ReadBoolean();
            br.Position += 7;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_pendingOperationsCount = br.ReadInt32();
            m_pendingBodyOperationsCount = br.ReadInt32();
            m_criticalOperationsLockCount = br.ReadInt32();
            m_criticalOperationsLockCountForPhantoms = br.ReadInt32();
            m_blockExecutingPendingOperations = br.ReadBoolean();
            m_criticalOperationsAllowed = br.ReadBoolean();
            br.Position += 6;
            des.ReadEmptyPointer(br);
            m_pendingOperationQueueCount = br.ReadInt32();
            m_multiThreadCheck.Read(des, br);
            m_processActionsInSingleThread = br.ReadBoolean();
            m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob = br.ReadBoolean();
            br.Position += 2;
            m_minDesiredIslandSize = br.ReadUInt32();
            des.ReadEmptyPointer(br);
            m_isLocked = br.ReadInt32();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_wantSimulationIslands = br.ReadBoolean();
            m_useHybridBroadphase = br.ReadBoolean();
            br.Position += 2;
            m_snapCollisionToConvexEdgeThreshold = br.ReadSingle();
            m_snapCollisionToConcaveEdgeThreshold = br.ReadSingle();
            m_enableToiWeldRejection = br.ReadBoolean();
            m_wantDeactivation = br.ReadBoolean();
            m_shouldActivateOnRigidBodyTransformChange = br.ReadBoolean();
            br.Position += 1;
            m_deactivationReferenceDistance = br.ReadSingle();
            m_toiCollisionResponseRotateNormal = br.ReadSingle();
            m_maxSectorsPerMidphaseCollideTask = br.ReadInt32();
            m_maxSectorsPerNarrowphaseCollideTask = br.ReadInt32();
            m_processToisMultithreaded = br.ReadBoolean();
            br.Position += 3;
            m_maxEntriesPerToiMidphaseCollideTask = br.ReadInt32();
            m_maxEntriesPerToiNarrowphaseCollideTask = br.ReadInt32();
            m_maxNumToiCollisionPairsSinglethreaded = br.ReadInt32();
            m_simulationType = br.ReadInt32();
            m_numToisTillAllowedPenetrationSimplifiedToi = br.ReadSingle();
            m_numToisTillAllowedPenetrationToi = br.ReadSingle();
            m_numToisTillAllowedPenetrationToiHigher = br.ReadSingle();
            m_numToisTillAllowedPenetrationToiForced = br.ReadSingle();
            m_lastEntityUid = br.ReadUInt32();
            m_lastIslandUid = br.ReadUInt32();
            m_lastConstraintUid = br.ReadUInt32();
            m_phantoms = des.ReadClassPointerArray<hkpPhantom>(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            br.Position += 328;
            m_broadPhaseExtents = des.ReadVector4CStyleArray(br, 2);
            m_broadPhaseNumMarkers = br.ReadInt32();
            m_sizeOfToiEventQueue = br.ReadInt32();
            m_broadPhaseQuerySize = br.ReadInt32();
            m_broadPhaseUpdateSize = br.ReadInt32();
            m_contactPointGeneration = br.ReadSByte();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_simulation);
            bw.Position += 8;
            bw.WriteVector4(m_gravity);
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_fixedRigidBody);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteBoolean(m_assertOnRunningOutOfSolverMemory);
            bw.Position += 7;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteBoolean(m_autoUpdateTree);
            bw.Position += 7;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_pendingOperationsCount);
            bw.WriteInt32(m_pendingBodyOperationsCount);
            bw.WriteInt32(m_criticalOperationsLockCount);
            bw.WriteInt32(m_criticalOperationsLockCountForPhantoms);
            bw.WriteBoolean(m_blockExecutingPendingOperations);
            bw.WriteBoolean(m_criticalOperationsAllowed);
            bw.Position += 6;
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_pendingOperationQueueCount);
            m_multiThreadCheck.Write(s, bw);
            bw.WriteBoolean(m_processActionsInSingleThread);
            bw.WriteBoolean(m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob);
            bw.Position += 2;
            bw.WriteUInt32(m_minDesiredIslandSize);
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_isLocked);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteBoolean(m_wantSimulationIslands);
            bw.WriteBoolean(m_useHybridBroadphase);
            bw.Position += 2;
            bw.WriteSingle(m_snapCollisionToConvexEdgeThreshold);
            bw.WriteSingle(m_snapCollisionToConcaveEdgeThreshold);
            bw.WriteBoolean(m_enableToiWeldRejection);
            bw.WriteBoolean(m_wantDeactivation);
            bw.WriteBoolean(m_shouldActivateOnRigidBodyTransformChange);
            bw.Position += 1;
            bw.WriteSingle(m_deactivationReferenceDistance);
            bw.WriteSingle(m_toiCollisionResponseRotateNormal);
            bw.WriteInt32(m_maxSectorsPerMidphaseCollideTask);
            bw.WriteInt32(m_maxSectorsPerNarrowphaseCollideTask);
            bw.WriteBoolean(m_processToisMultithreaded);
            bw.Position += 3;
            bw.WriteInt32(m_maxEntriesPerToiMidphaseCollideTask);
            bw.WriteInt32(m_maxEntriesPerToiNarrowphaseCollideTask);
            bw.WriteInt32(m_maxNumToiCollisionPairsSinglethreaded);
            bw.WriteInt32(m_simulationType);
            bw.WriteSingle(m_numToisTillAllowedPenetrationSimplifiedToi);
            bw.WriteSingle(m_numToisTillAllowedPenetrationToi);
            bw.WriteSingle(m_numToisTillAllowedPenetrationToiHigher);
            bw.WriteSingle(m_numToisTillAllowedPenetrationToiForced);
            bw.WriteUInt32(m_lastEntityUid);
            bw.WriteUInt32(m_lastIslandUid);
            bw.WriteUInt32(m_lastConstraintUid);
            s.WriteClassPointerArray(bw, m_phantoms);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.Position += 328;
            s.WriteVector4CStyleArray(bw, m_broadPhaseExtents);
            bw.WriteInt32(m_broadPhaseNumMarkers);
            bw.WriteInt32(m_sizeOfToiEventQueue);
            bw.WriteInt32(m_broadPhaseQuerySize);
            bw.WriteInt32(m_broadPhaseUpdateSize);
            bw.WriteSByte(m_contactPointGeneration);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_simulation = xd.ReadClassPointer<hkpSimulation>(xe, nameof(m_simulation));
            m_gravity = xd.ReadVector4(xe, nameof(m_gravity));
            m_fixedRigidBody = xd.ReadClassPointer<hkpRigidBody>(xe, nameof(m_fixedRigidBody));
            m_autoUpdateTree = xd.ReadBoolean(xe, nameof(m_autoUpdateTree));
            m_pendingOperationsCount = xd.ReadInt32(xe, nameof(m_pendingOperationsCount));
            m_criticalOperationsLockCount = xd.ReadInt32(xe, nameof(m_criticalOperationsLockCount));
            m_criticalOperationsLockCountForPhantoms = xd.ReadInt32(xe, nameof(m_criticalOperationsLockCountForPhantoms));
            m_blockExecutingPendingOperations = xd.ReadBoolean(xe, nameof(m_blockExecutingPendingOperations));
            m_criticalOperationsAllowed = xd.ReadBoolean(xe, nameof(m_criticalOperationsAllowed));
            m_pendingOperationQueueCount = xd.ReadInt32(xe, nameof(m_pendingOperationQueueCount));
            m_processActionsInSingleThread = xd.ReadBoolean(xe, nameof(m_processActionsInSingleThread));
            m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob = xd.ReadBoolean(xe, nameof(m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob));
            m_minDesiredIslandSize = xd.ReadUInt32(xe, nameof(m_minDesiredIslandSize));
            m_isLocked = xd.ReadInt32(xe, nameof(m_isLocked));
            m_wantSimulationIslands = xd.ReadBoolean(xe, nameof(m_wantSimulationIslands));
            m_snapCollisionToConvexEdgeThreshold = xd.ReadSingle(xe, nameof(m_snapCollisionToConvexEdgeThreshold));
            m_snapCollisionToConcaveEdgeThreshold = xd.ReadSingle(xe, nameof(m_snapCollisionToConcaveEdgeThreshold));
            m_enableToiWeldRejection = xd.ReadBoolean(xe, nameof(m_enableToiWeldRejection));
            m_wantDeactivation = xd.ReadBoolean(xe, nameof(m_wantDeactivation));
            m_shouldActivateOnRigidBodyTransformChange = xd.ReadBoolean(xe, nameof(m_shouldActivateOnRigidBodyTransformChange));
            m_deactivationReferenceDistance = xd.ReadSingle(xe, nameof(m_deactivationReferenceDistance));
            m_toiCollisionResponseRotateNormal = xd.ReadSingle(xe, nameof(m_toiCollisionResponseRotateNormal));
            m_maxSectorsPerMidphaseCollideTask = xd.ReadInt32(xe, nameof(m_maxSectorsPerMidphaseCollideTask));
            m_maxSectorsPerNarrowphaseCollideTask = xd.ReadInt32(xe, nameof(m_maxSectorsPerNarrowphaseCollideTask));
            m_processToisMultithreaded = xd.ReadBoolean(xe, nameof(m_processToisMultithreaded));
            m_maxEntriesPerToiMidphaseCollideTask = xd.ReadInt32(xe, nameof(m_maxEntriesPerToiMidphaseCollideTask));
            m_maxEntriesPerToiNarrowphaseCollideTask = xd.ReadInt32(xe, nameof(m_maxEntriesPerToiNarrowphaseCollideTask));
            m_maxNumToiCollisionPairsSinglethreaded = xd.ReadInt32(xe, nameof(m_maxNumToiCollisionPairsSinglethreaded));
            m_numToisTillAllowedPenetrationSimplifiedToi = xd.ReadSingle(xe, nameof(m_numToisTillAllowedPenetrationSimplifiedToi));
            m_numToisTillAllowedPenetrationToi = xd.ReadSingle(xe, nameof(m_numToisTillAllowedPenetrationToi));
            m_numToisTillAllowedPenetrationToiHigher = xd.ReadSingle(xe, nameof(m_numToisTillAllowedPenetrationToiHigher));
            m_numToisTillAllowedPenetrationToiForced = xd.ReadSingle(xe, nameof(m_numToisTillAllowedPenetrationToiForced));
            m_lastEntityUid = xd.ReadUInt32(xe, nameof(m_lastEntityUid));
            m_lastIslandUid = xd.ReadUInt32(xe, nameof(m_lastIslandUid));
            m_lastConstraintUid = xd.ReadUInt32(xe, nameof(m_lastConstraintUid));
            m_phantoms = xd.ReadClassPointerArray<hkpPhantom>(xe, nameof(m_phantoms));
            m_broadPhaseExtents = xd.ReadVector4CStyleArray(xe, nameof(m_broadPhaseExtents), 2);
            m_broadPhaseNumMarkers = xd.ReadInt32(xe, nameof(m_broadPhaseNumMarkers));
            m_sizeOfToiEventQueue = xd.ReadInt32(xe, nameof(m_sizeOfToiEventQueue));
            m_broadPhaseQuerySize = xd.ReadInt32(xe, nameof(m_broadPhaseQuerySize));
            m_broadPhaseUpdateSize = xd.ReadInt32(xe, nameof(m_broadPhaseUpdateSize));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_simulation), m_simulation);
            xs.WriteVector4(xe, nameof(m_gravity), m_gravity);
            xs.WriteSerializeIgnored(xe, nameof(m_fixedIsland));
            xs.WriteClassPointer(xe, nameof(m_fixedRigidBody), m_fixedRigidBody);
            xs.WriteSerializeIgnored(xe, nameof(m_activeSimulationIslands));
            xs.WriteSerializeIgnored(xe, nameof(m_inactiveSimulationIslands));
            xs.WriteSerializeIgnored(xe, nameof(m_dirtySimulationIslands));
            xs.WriteSerializeIgnored(xe, nameof(m_maintenanceMgr));
            xs.WriteSerializeIgnored(xe, nameof(m_memoryWatchDog));
            xs.WriteSerializeIgnored(xe, nameof(m_assertOnRunningOutOfSolverMemory));
            xs.WriteSerializeIgnored(xe, nameof(m_broadPhase));
            xs.WriteSerializeIgnored(xe, nameof(m_kdTreeManager));
            xs.WriteBoolean(xe, nameof(m_autoUpdateTree), m_autoUpdateTree);
            xs.WriteSerializeIgnored(xe, nameof(m_broadPhaseDispatcher));
            xs.WriteSerializeIgnored(xe, nameof(m_phantomBroadPhaseListener));
            xs.WriteSerializeIgnored(xe, nameof(m_entityEntityBroadPhaseListener));
            xs.WriteSerializeIgnored(xe, nameof(m_broadPhaseBorderListener));
            xs.WriteSerializeIgnored(xe, nameof(m_multithreadedSimulationJobData));
            xs.WriteSerializeIgnored(xe, nameof(m_collisionInput));
            xs.WriteSerializeIgnored(xe, nameof(m_collisionFilter));
            xs.WriteSerializeIgnored(xe, nameof(m_collisionDispatcher));
            xs.WriteSerializeIgnored(xe, nameof(m_convexListFilter));
            xs.WriteSerializeIgnored(xe, nameof(m_pendingOperations));
            xs.WriteNumber(xe, nameof(m_pendingOperationsCount), m_pendingOperationsCount);
            xs.WriteSerializeIgnored(xe, nameof(m_pendingBodyOperationsCount));
            xs.WriteNumber(xe, nameof(m_criticalOperationsLockCount), m_criticalOperationsLockCount);
            xs.WriteNumber(xe, nameof(m_criticalOperationsLockCountForPhantoms), m_criticalOperationsLockCountForPhantoms);
            xs.WriteBoolean(xe, nameof(m_blockExecutingPendingOperations), m_blockExecutingPendingOperations);
            xs.WriteBoolean(xe, nameof(m_criticalOperationsAllowed), m_criticalOperationsAllowed);
            xs.WriteSerializeIgnored(xe, nameof(m_pendingOperationQueues));
            xs.WriteNumber(xe, nameof(m_pendingOperationQueueCount), m_pendingOperationQueueCount);
            xs.WriteSerializeIgnored(xe, nameof(m_multiThreadCheck));
            xs.WriteBoolean(xe, nameof(m_processActionsInSingleThread), m_processActionsInSingleThread);
            xs.WriteBoolean(xe, nameof(m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob), m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob);
            xs.WriteNumber(xe, nameof(m_minDesiredIslandSize), m_minDesiredIslandSize);
            xs.WriteSerializeIgnored(xe, nameof(m_modifyConstraintCriticalSection));
            xs.WriteNumber(xe, nameof(m_isLocked), m_isLocked);
            xs.WriteSerializeIgnored(xe, nameof(m_islandDirtyListCriticalSection));
            xs.WriteSerializeIgnored(xe, nameof(m_propertyMasterLock));
            xs.WriteBoolean(xe, nameof(m_wantSimulationIslands), m_wantSimulationIslands);
            xs.WriteSerializeIgnored(xe, nameof(m_useHybridBroadphase));
            xs.WriteFloat(xe, nameof(m_snapCollisionToConvexEdgeThreshold), m_snapCollisionToConvexEdgeThreshold);
            xs.WriteFloat(xe, nameof(m_snapCollisionToConcaveEdgeThreshold), m_snapCollisionToConcaveEdgeThreshold);
            xs.WriteBoolean(xe, nameof(m_enableToiWeldRejection), m_enableToiWeldRejection);
            xs.WriteBoolean(xe, nameof(m_wantDeactivation), m_wantDeactivation);
            xs.WriteBoolean(xe, nameof(m_shouldActivateOnRigidBodyTransformChange), m_shouldActivateOnRigidBodyTransformChange);
            xs.WriteFloat(xe, nameof(m_deactivationReferenceDistance), m_deactivationReferenceDistance);
            xs.WriteFloat(xe, nameof(m_toiCollisionResponseRotateNormal), m_toiCollisionResponseRotateNormal);
            xs.WriteNumber(xe, nameof(m_maxSectorsPerMidphaseCollideTask), m_maxSectorsPerMidphaseCollideTask);
            xs.WriteNumber(xe, nameof(m_maxSectorsPerNarrowphaseCollideTask), m_maxSectorsPerNarrowphaseCollideTask);
            xs.WriteBoolean(xe, nameof(m_processToisMultithreaded), m_processToisMultithreaded);
            xs.WriteNumber(xe, nameof(m_maxEntriesPerToiMidphaseCollideTask), m_maxEntriesPerToiMidphaseCollideTask);
            xs.WriteNumber(xe, nameof(m_maxEntriesPerToiNarrowphaseCollideTask), m_maxEntriesPerToiNarrowphaseCollideTask);
            xs.WriteNumber(xe, nameof(m_maxNumToiCollisionPairsSinglethreaded), m_maxNumToiCollisionPairsSinglethreaded);
            xs.WriteSerializeIgnored(xe, nameof(m_simulationType));
            xs.WriteFloat(xe, nameof(m_numToisTillAllowedPenetrationSimplifiedToi), m_numToisTillAllowedPenetrationSimplifiedToi);
            xs.WriteFloat(xe, nameof(m_numToisTillAllowedPenetrationToi), m_numToisTillAllowedPenetrationToi);
            xs.WriteFloat(xe, nameof(m_numToisTillAllowedPenetrationToiHigher), m_numToisTillAllowedPenetrationToiHigher);
            xs.WriteFloat(xe, nameof(m_numToisTillAllowedPenetrationToiForced), m_numToisTillAllowedPenetrationToiForced);
            xs.WriteNumber(xe, nameof(m_lastEntityUid), m_lastEntityUid);
            xs.WriteNumber(xe, nameof(m_lastIslandUid), m_lastIslandUid);
            xs.WriteNumber(xe, nameof(m_lastConstraintUid), m_lastConstraintUid);
            xs.WriteClassPointerArray(xe, nameof(m_phantoms), m_phantoms);
            xs.WriteSerializeIgnored(xe, nameof(m_actionListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_entityListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_phantomListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_constraintListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_worldDeletionListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_islandActivationListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_worldPostSimulationListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_worldPostIntegrateListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_worldPostCollideListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_islandPostIntegrateListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_islandPostCollideListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_contactListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_contactImpulseLimitBreachedListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_worldExtensions));
            xs.WriteSerializeIgnored(xe, nameof(m_violatedConstraintArray));
            xs.WriteSerializeIgnored(xe, nameof(m_broadPhaseBorder));
            xs.WriteSerializeIgnored(xe, nameof(m_destructionWorld));
            xs.WriteSerializeIgnored(xe, nameof(m_npWorld));
            xs.WriteVector4Array(xe, nameof(m_broadPhaseExtents), m_broadPhaseExtents);
            xs.WriteNumber(xe, nameof(m_broadPhaseNumMarkers), m_broadPhaseNumMarkers);
            xs.WriteNumber(xe, nameof(m_sizeOfToiEventQueue), m_sizeOfToiEventQueue);
            xs.WriteNumber(xe, nameof(m_broadPhaseQuerySize), m_broadPhaseQuerySize);
            xs.WriteNumber(xe, nameof(m_broadPhaseUpdateSize), m_broadPhaseUpdateSize);
            xs.WriteSerializeIgnored(xe, nameof(m_contactPointGeneration));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpWorld);
        }

        public bool Equals(hkpWorld? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_simulation is null && other.m_simulation is null) || (m_simulation is not null && other.m_simulation is not null && m_simulation.Equals((IHavokObject)other.m_simulation))) &&
                   m_gravity.Equals(other.m_gravity) &&
                   ((m_fixedRigidBody is null && other.m_fixedRigidBody is null) || (m_fixedRigidBody is not null && other.m_fixedRigidBody is not null && m_fixedRigidBody.Equals((IHavokObject)other.m_fixedRigidBody))) &&
                   m_autoUpdateTree.Equals(other.m_autoUpdateTree) &&
                   m_pendingOperationsCount.Equals(other.m_pendingOperationsCount) &&
                   m_criticalOperationsLockCount.Equals(other.m_criticalOperationsLockCount) &&
                   m_criticalOperationsLockCountForPhantoms.Equals(other.m_criticalOperationsLockCountForPhantoms) &&
                   m_blockExecutingPendingOperations.Equals(other.m_blockExecutingPendingOperations) &&
                   m_criticalOperationsAllowed.Equals(other.m_criticalOperationsAllowed) &&
                   m_pendingOperationQueueCount.Equals(other.m_pendingOperationQueueCount) &&
                   m_processActionsInSingleThread.Equals(other.m_processActionsInSingleThread) &&
                   m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob.Equals(other.m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob) &&
                   m_minDesiredIslandSize.Equals(other.m_minDesiredIslandSize) &&
                   m_isLocked.Equals(other.m_isLocked) &&
                   m_wantSimulationIslands.Equals(other.m_wantSimulationIslands) &&
                   m_snapCollisionToConvexEdgeThreshold.Equals(other.m_snapCollisionToConvexEdgeThreshold) &&
                   m_snapCollisionToConcaveEdgeThreshold.Equals(other.m_snapCollisionToConcaveEdgeThreshold) &&
                   m_enableToiWeldRejection.Equals(other.m_enableToiWeldRejection) &&
                   m_wantDeactivation.Equals(other.m_wantDeactivation) &&
                   m_shouldActivateOnRigidBodyTransformChange.Equals(other.m_shouldActivateOnRigidBodyTransformChange) &&
                   m_deactivationReferenceDistance.Equals(other.m_deactivationReferenceDistance) &&
                   m_toiCollisionResponseRotateNormal.Equals(other.m_toiCollisionResponseRotateNormal) &&
                   m_maxSectorsPerMidphaseCollideTask.Equals(other.m_maxSectorsPerMidphaseCollideTask) &&
                   m_maxSectorsPerNarrowphaseCollideTask.Equals(other.m_maxSectorsPerNarrowphaseCollideTask) &&
                   m_processToisMultithreaded.Equals(other.m_processToisMultithreaded) &&
                   m_maxEntriesPerToiMidphaseCollideTask.Equals(other.m_maxEntriesPerToiMidphaseCollideTask) &&
                   m_maxEntriesPerToiNarrowphaseCollideTask.Equals(other.m_maxEntriesPerToiNarrowphaseCollideTask) &&
                   m_maxNumToiCollisionPairsSinglethreaded.Equals(other.m_maxNumToiCollisionPairsSinglethreaded) &&
                   m_numToisTillAllowedPenetrationSimplifiedToi.Equals(other.m_numToisTillAllowedPenetrationSimplifiedToi) &&
                   m_numToisTillAllowedPenetrationToi.Equals(other.m_numToisTillAllowedPenetrationToi) &&
                   m_numToisTillAllowedPenetrationToiHigher.Equals(other.m_numToisTillAllowedPenetrationToiHigher) &&
                   m_numToisTillAllowedPenetrationToiForced.Equals(other.m_numToisTillAllowedPenetrationToiForced) &&
                   m_lastEntityUid.Equals(other.m_lastEntityUid) &&
                   m_lastIslandUid.Equals(other.m_lastIslandUid) &&
                   m_lastConstraintUid.Equals(other.m_lastConstraintUid) &&
                   m_phantoms.SequenceEqual(other.m_phantoms) &&
                   m_broadPhaseExtents.SequenceEqual(other.m_broadPhaseExtents) &&
                   m_broadPhaseNumMarkers.Equals(other.m_broadPhaseNumMarkers) &&
                   m_sizeOfToiEventQueue.Equals(other.m_sizeOfToiEventQueue) &&
                   m_broadPhaseQuerySize.Equals(other.m_broadPhaseQuerySize) &&
                   m_broadPhaseUpdateSize.Equals(other.m_broadPhaseUpdateSize) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_simulation);
            hashcode.Add(m_gravity);
            hashcode.Add(m_fixedRigidBody);
            hashcode.Add(m_autoUpdateTree);
            hashcode.Add(m_pendingOperationsCount);
            hashcode.Add(m_criticalOperationsLockCount);
            hashcode.Add(m_criticalOperationsLockCountForPhantoms);
            hashcode.Add(m_blockExecutingPendingOperations);
            hashcode.Add(m_criticalOperationsAllowed);
            hashcode.Add(m_pendingOperationQueueCount);
            hashcode.Add(m_processActionsInSingleThread);
            hashcode.Add(m_allowIntegrationOfIslandsWithoutConstraintsInASeparateJob);
            hashcode.Add(m_minDesiredIslandSize);
            hashcode.Add(m_isLocked);
            hashcode.Add(m_wantSimulationIslands);
            hashcode.Add(m_snapCollisionToConvexEdgeThreshold);
            hashcode.Add(m_snapCollisionToConcaveEdgeThreshold);
            hashcode.Add(m_enableToiWeldRejection);
            hashcode.Add(m_wantDeactivation);
            hashcode.Add(m_shouldActivateOnRigidBodyTransformChange);
            hashcode.Add(m_deactivationReferenceDistance);
            hashcode.Add(m_toiCollisionResponseRotateNormal);
            hashcode.Add(m_maxSectorsPerMidphaseCollideTask);
            hashcode.Add(m_maxSectorsPerNarrowphaseCollideTask);
            hashcode.Add(m_processToisMultithreaded);
            hashcode.Add(m_maxEntriesPerToiMidphaseCollideTask);
            hashcode.Add(m_maxEntriesPerToiNarrowphaseCollideTask);
            hashcode.Add(m_maxNumToiCollisionPairsSinglethreaded);
            hashcode.Add(m_numToisTillAllowedPenetrationSimplifiedToi);
            hashcode.Add(m_numToisTillAllowedPenetrationToi);
            hashcode.Add(m_numToisTillAllowedPenetrationToiHigher);
            hashcode.Add(m_numToisTillAllowedPenetrationToiForced);
            hashcode.Add(m_lastEntityUid);
            hashcode.Add(m_lastIslandUid);
            hashcode.Add(m_lastConstraintUid);
            hashcode.Add(m_phantoms.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_broadPhaseExtents.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_broadPhaseNumMarkers);
            hashcode.Add(m_sizeOfToiEventQueue);
            hashcode.Add(m_broadPhaseQuerySize);
            hashcode.Add(m_broadPhaseUpdateSize);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

