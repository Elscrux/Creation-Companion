using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorGraphInternalStateInfo Signatire: 0x645f898b size: 80 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_internalState m_class: hkbBehaviorGraphInternalState Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_auxiliaryNodeInfo m_class: hkbAuxiliaryNodeInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_activeEventIds m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_activeVariableIds m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkbBehaviorGraphInternalStateInfo : hkReferencedObject, IEquatable<hkbBehaviorGraphInternalStateInfo?>
    {
        public ulong m_characterId { set; get; }
        public hkbBehaviorGraphInternalState? m_internalState { set; get; }
        public IList<hkbAuxiliaryNodeInfo> m_auxiliaryNodeInfo { set; get; } = Array.Empty<hkbAuxiliaryNodeInfo>();
        public IList<short> m_activeEventIds { set; get; } = Array.Empty<short>();
        public IList<short> m_activeVariableIds { set; get; } = Array.Empty<short>();

        public override uint Signature { set; get; } = 0x645f898b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_internalState = des.ReadClassPointer<hkbBehaviorGraphInternalState>(br);
            m_auxiliaryNodeInfo = des.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(br);
            m_activeEventIds = des.ReadInt16Array(br);
            m_activeVariableIds = des.ReadInt16Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            s.WriteClassPointer(bw, m_internalState);
            s.WriteClassPointerArray(bw, m_auxiliaryNodeInfo);
            s.WriteInt16Array(bw, m_activeEventIds);
            s.WriteInt16Array(bw, m_activeVariableIds);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_internalState = xd.ReadClassPointer<hkbBehaviorGraphInternalState>(xe, nameof(m_internalState));
            m_auxiliaryNodeInfo = xd.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(xe, nameof(m_auxiliaryNodeInfo));
            m_activeEventIds = xd.ReadInt16Array(xe, nameof(m_activeEventIds));
            m_activeVariableIds = xd.ReadInt16Array(xe, nameof(m_activeVariableIds));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteClassPointer(xe, nameof(m_internalState), m_internalState);
            xs.WriteClassPointerArray(xe, nameof(m_auxiliaryNodeInfo), m_auxiliaryNodeInfo);
            xs.WriteNumberArray(xe, nameof(m_activeEventIds), m_activeEventIds);
            xs.WriteNumberArray(xe, nameof(m_activeVariableIds), m_activeVariableIds);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorGraphInternalStateInfo);
        }

        public bool Equals(hkbBehaviorGraphInternalStateInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   ((m_internalState is null && other.m_internalState is null) || (m_internalState is not null && other.m_internalState is not null && m_internalState.Equals((IHavokObject)other.m_internalState))) &&
                   m_auxiliaryNodeInfo.SequenceEqual(other.m_auxiliaryNodeInfo) &&
                   m_activeEventIds.SequenceEqual(other.m_activeEventIds) &&
                   m_activeVariableIds.SequenceEqual(other.m_activeVariableIds) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_internalState);
            hashcode.Add(m_auxiliaryNodeInfo.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_activeEventIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_activeVariableIds.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

