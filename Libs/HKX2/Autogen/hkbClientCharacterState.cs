using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbClientCharacterState Signatire: 0xa2624c97 size: 272 flags: FLAGS_NONE

    // m_deformableSkinIds m_class:  Type.TYPE_ARRAY Type.TYPE_UINT64 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_rigidSkinIds m_class:  Type.TYPE_ARRAY Type.TYPE_UINT64 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_externalEventIds m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_auxiliaryInfo m_class: hkbAuxiliaryNodeInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_activeEventIds m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_activeVariableIds m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_instanceName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_templateName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_fullPathToProject m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_behaviorData m_class: hkbBehaviorGraphData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_behaviorInternalState m_class: hkbBehaviorGraphInternalState Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_nodeIdToInternalStateMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 160 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_visible m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_elapsedSimulationTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 172 flags: FLAGS_NONE enum: 
    // m_skeleton m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_worldFromModel m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_poseModelSpace m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    // m_rigidAttachmentTransforms m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 256 flags: FLAGS_NONE enum: 
    public partial class hkbClientCharacterState : hkReferencedObject, IEquatable<hkbClientCharacterState?>
    {
        public IList<ulong> m_deformableSkinIds { set; get; } = Array.Empty<ulong>();
        public IList<ulong> m_rigidSkinIds { set; get; } = Array.Empty<ulong>();
        public IList<short> m_externalEventIds { set; get; } = Array.Empty<short>();
        public IList<hkbAuxiliaryNodeInfo> m_auxiliaryInfo { set; get; } = Array.Empty<hkbAuxiliaryNodeInfo>();
        public IList<short> m_activeEventIds { set; get; } = Array.Empty<short>();
        public IList<short> m_activeVariableIds { set; get; } = Array.Empty<short>();
        public ulong m_characterId { set; get; }
        public string m_instanceName { set; get; } = "";
        public string m_templateName { set; get; } = "";
        public string m_fullPathToProject { set; get; } = "";
        public hkbBehaviorGraphData? m_behaviorData { set; get; }
        public hkbBehaviorGraphInternalState? m_behaviorInternalState { set; get; }
        private object? m_nodeIdToInternalStateMap { set; get; }
        public bool m_visible { set; get; }
        public float m_elapsedSimulationTime { set; get; }
        public hkaSkeleton? m_skeleton { set; get; }
        public Matrix4x4 m_worldFromModel { set; get; }
        public IList<Matrix4x4> m_poseModelSpace { set; get; } = Array.Empty<Matrix4x4>();
        public IList<Matrix4x4> m_rigidAttachmentTransforms { set; get; } = Array.Empty<Matrix4x4>();

        public override uint Signature { set; get; } = 0xa2624c97;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_deformableSkinIds = des.ReadUInt64Array(br);
            m_rigidSkinIds = des.ReadUInt64Array(br);
            m_externalEventIds = des.ReadInt16Array(br);
            m_auxiliaryInfo = des.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(br);
            m_activeEventIds = des.ReadInt16Array(br);
            m_activeVariableIds = des.ReadInt16Array(br);
            m_characterId = br.ReadUInt64();
            m_instanceName = des.ReadStringPointer(br);
            m_templateName = des.ReadStringPointer(br);
            m_fullPathToProject = des.ReadStringPointer(br);
            m_behaviorData = des.ReadClassPointer<hkbBehaviorGraphData>(br);
            m_behaviorInternalState = des.ReadClassPointer<hkbBehaviorGraphInternalState>(br);
            des.ReadEmptyPointer(br);
            m_visible = br.ReadBoolean();
            br.Position += 3;
            m_elapsedSimulationTime = br.ReadSingle();
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
            br.Position += 8;
            m_worldFromModel = des.ReadQSTransform(br);
            m_poseModelSpace = des.ReadQSTransformArray(br);
            m_rigidAttachmentTransforms = des.ReadQSTransformArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteUInt64Array(bw, m_deformableSkinIds);
            s.WriteUInt64Array(bw, m_rigidSkinIds);
            s.WriteInt16Array(bw, m_externalEventIds);
            s.WriteClassPointerArray(bw, m_auxiliaryInfo);
            s.WriteInt16Array(bw, m_activeEventIds);
            s.WriteInt16Array(bw, m_activeVariableIds);
            bw.WriteUInt64(m_characterId);
            s.WriteStringPointer(bw, m_instanceName);
            s.WriteStringPointer(bw, m_templateName);
            s.WriteStringPointer(bw, m_fullPathToProject);
            s.WriteClassPointer(bw, m_behaviorData);
            s.WriteClassPointer(bw, m_behaviorInternalState);
            s.WriteVoidPointer(bw);
            bw.WriteBoolean(m_visible);
            bw.Position += 3;
            bw.WriteSingle(m_elapsedSimulationTime);
            s.WriteClassPointer(bw, m_skeleton);
            bw.Position += 8;
            s.WriteQSTransform(bw, m_worldFromModel);
            s.WriteQSTransformArray(bw, m_poseModelSpace);
            s.WriteQSTransformArray(bw, m_rigidAttachmentTransforms);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_deformableSkinIds = xd.ReadUInt64Array(xe, nameof(m_deformableSkinIds));
            m_rigidSkinIds = xd.ReadUInt64Array(xe, nameof(m_rigidSkinIds));
            m_externalEventIds = xd.ReadInt16Array(xe, nameof(m_externalEventIds));
            m_auxiliaryInfo = xd.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(xe, nameof(m_auxiliaryInfo));
            m_activeEventIds = xd.ReadInt16Array(xe, nameof(m_activeEventIds));
            m_activeVariableIds = xd.ReadInt16Array(xe, nameof(m_activeVariableIds));
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_instanceName = xd.ReadString(xe, nameof(m_instanceName));
            m_templateName = xd.ReadString(xe, nameof(m_templateName));
            m_fullPathToProject = xd.ReadString(xe, nameof(m_fullPathToProject));
            m_behaviorData = xd.ReadClassPointer<hkbBehaviorGraphData>(xe, nameof(m_behaviorData));
            m_behaviorInternalState = xd.ReadClassPointer<hkbBehaviorGraphInternalState>(xe, nameof(m_behaviorInternalState));
            m_visible = xd.ReadBoolean(xe, nameof(m_visible));
            m_elapsedSimulationTime = xd.ReadSingle(xe, nameof(m_elapsedSimulationTime));
            m_skeleton = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_skeleton));
            m_worldFromModel = xd.ReadQSTransform(xe, nameof(m_worldFromModel));
            m_poseModelSpace = xd.ReadQSTransformArray(xe, nameof(m_poseModelSpace));
            m_rigidAttachmentTransforms = xd.ReadQSTransformArray(xe, nameof(m_rigidAttachmentTransforms));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_deformableSkinIds), m_deformableSkinIds);
            xs.WriteNumberArray(xe, nameof(m_rigidSkinIds), m_rigidSkinIds);
            xs.WriteNumberArray(xe, nameof(m_externalEventIds), m_externalEventIds);
            xs.WriteClassPointerArray(xe, nameof(m_auxiliaryInfo), m_auxiliaryInfo);
            xs.WriteNumberArray(xe, nameof(m_activeEventIds), m_activeEventIds);
            xs.WriteNumberArray(xe, nameof(m_activeVariableIds), m_activeVariableIds);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteString(xe, nameof(m_instanceName), m_instanceName);
            xs.WriteString(xe, nameof(m_templateName), m_templateName);
            xs.WriteString(xe, nameof(m_fullPathToProject), m_fullPathToProject);
            xs.WriteClassPointer(xe, nameof(m_behaviorData), m_behaviorData);
            xs.WriteClassPointer(xe, nameof(m_behaviorInternalState), m_behaviorInternalState);
            xs.WriteSerializeIgnored(xe, nameof(m_nodeIdToInternalStateMap));
            xs.WriteBoolean(xe, nameof(m_visible), m_visible);
            xs.WriteFloat(xe, nameof(m_elapsedSimulationTime), m_elapsedSimulationTime);
            xs.WriteClassPointer(xe, nameof(m_skeleton), m_skeleton);
            xs.WriteQSTransform(xe, nameof(m_worldFromModel), m_worldFromModel);
            xs.WriteQSTransformArray(xe, nameof(m_poseModelSpace), m_poseModelSpace);
            xs.WriteQSTransformArray(xe, nameof(m_rigidAttachmentTransforms), m_rigidAttachmentTransforms);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbClientCharacterState);
        }

        public bool Equals(hkbClientCharacterState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_deformableSkinIds.SequenceEqual(other.m_deformableSkinIds) &&
                   m_rigidSkinIds.SequenceEqual(other.m_rigidSkinIds) &&
                   m_externalEventIds.SequenceEqual(other.m_externalEventIds) &&
                   m_auxiliaryInfo.SequenceEqual(other.m_auxiliaryInfo) &&
                   m_activeEventIds.SequenceEqual(other.m_activeEventIds) &&
                   m_activeVariableIds.SequenceEqual(other.m_activeVariableIds) &&
                   m_characterId.Equals(other.m_characterId) &&
                   (m_instanceName is null && other.m_instanceName is null || m_instanceName == other.m_instanceName || m_instanceName is null && other.m_instanceName == "" || m_instanceName == "" && other.m_instanceName is null) &&
                   (m_templateName is null && other.m_templateName is null || m_templateName == other.m_templateName || m_templateName is null && other.m_templateName == "" || m_templateName == "" && other.m_templateName is null) &&
                   (m_fullPathToProject is null && other.m_fullPathToProject is null || m_fullPathToProject == other.m_fullPathToProject || m_fullPathToProject is null && other.m_fullPathToProject == "" || m_fullPathToProject == "" && other.m_fullPathToProject is null) &&
                   ((m_behaviorData is null && other.m_behaviorData is null) || (m_behaviorData is not null && other.m_behaviorData is not null && m_behaviorData.Equals((IHavokObject)other.m_behaviorData))) &&
                   ((m_behaviorInternalState is null && other.m_behaviorInternalState is null) || (m_behaviorInternalState is not null && other.m_behaviorInternalState is not null && m_behaviorInternalState.Equals((IHavokObject)other.m_behaviorInternalState))) &&
                   m_visible.Equals(other.m_visible) &&
                   m_elapsedSimulationTime.Equals(other.m_elapsedSimulationTime) &&
                   ((m_skeleton is null && other.m_skeleton is null) || (m_skeleton is not null && other.m_skeleton is not null && m_skeleton.Equals((IHavokObject)other.m_skeleton))) &&
                   m_worldFromModel.Equals(other.m_worldFromModel) &&
                   m_poseModelSpace.SequenceEqual(other.m_poseModelSpace) &&
                   m_rigidAttachmentTransforms.SequenceEqual(other.m_rigidAttachmentTransforms) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_deformableSkinIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_rigidSkinIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_externalEventIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_auxiliaryInfo.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_activeEventIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_activeVariableIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_characterId);
            hashcode.Add(m_instanceName);
            hashcode.Add(m_templateName);
            hashcode.Add(m_fullPathToProject);
            hashcode.Add(m_behaviorData);
            hashcode.Add(m_behaviorInternalState);
            hashcode.Add(m_visible);
            hashcode.Add(m_elapsedSimulationTime);
            hashcode.Add(m_skeleton);
            hashcode.Add(m_worldFromModel);
            hashcode.Add(m_poseModelSpace.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_rigidAttachmentTransforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

