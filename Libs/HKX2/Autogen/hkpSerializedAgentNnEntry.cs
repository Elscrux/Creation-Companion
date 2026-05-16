using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedAgentNnEntry Signatire: 0x49ec7de3 size: 368 flags: FLAGS_NONE

    // m_bodyA m_class: hkpEntity Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_bodyB m_class: hkpEntity Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_bodyAId m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_bodyBId m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_useEntityIds m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_agentType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 49 flags: FLAGS_NONE enum: SerializedAgentType
    // m_atom m_class: hkpSimpleContactConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_propertiesStream m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_contactPoints m_class: hkContactPoint Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_cpIdMgr m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_nnEntryData m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 160 offset: 160 flags: FLAGS_NONE enum: 
    // m_trackInfo m_class: hkpSerializedTrack1nInfo Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 320 flags: FLAGS_NONE enum: 
    // m_endianCheckBuffer m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 4 offset: 352 flags: FLAGS_NONE enum: 
    // m_version m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 356 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedAgentNnEntry : hkReferencedObject, IEquatable<hkpSerializedAgentNnEntry?>
    {
        public hkpEntity? m_bodyA { set; get; }
        public hkpEntity? m_bodyB { set; get; }
        public ulong m_bodyAId { set; get; }
        public ulong m_bodyBId { set; get; }
        public bool m_useEntityIds { set; get; }
        public sbyte m_agentType { set; get; }
        public hkpSimpleContactConstraintAtom m_atom { set; get; } = new();
        public IList<byte> m_propertiesStream { set; get; } = Array.Empty<byte>();
        public IList<hkContactPoint> m_contactPoints { set; get; } = Array.Empty<hkContactPoint>();
        public IList<byte> m_cpIdMgr { set; get; } = Array.Empty<byte>();
        public byte[] m_nnEntryData = new byte[160];
        public hkpSerializedTrack1nInfo m_trackInfo { set; get; } = new();
        public byte[] m_endianCheckBuffer = new byte[4];
        public uint m_version { set; get; }

        public override uint Signature { set; get; } = 0x49ec7de3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bodyA = des.ReadClassPointer<hkpEntity>(br);
            m_bodyB = des.ReadClassPointer<hkpEntity>(br);
            m_bodyAId = br.ReadUInt64();
            m_bodyBId = br.ReadUInt64();
            m_useEntityIds = br.ReadBoolean();
            m_agentType = br.ReadSByte();
            br.Position += 14;
            m_atom.Read(des, br);
            m_propertiesStream = des.ReadByteArray(br);
            m_contactPoints = des.ReadClassArray<hkContactPoint>(br);
            m_cpIdMgr = des.ReadByteArray(br);
            m_nnEntryData = des.ReadByteCStyleArray(br, 160);
            m_trackInfo.Read(des, br);
            m_endianCheckBuffer = des.ReadByteCStyleArray(br, 4);
            m_version = br.ReadUInt32();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_bodyA);
            s.WriteClassPointer(bw, m_bodyB);
            bw.WriteUInt64(m_bodyAId);
            bw.WriteUInt64(m_bodyBId);
            bw.WriteBoolean(m_useEntityIds);
            bw.WriteSByte(m_agentType);
            bw.Position += 14;
            m_atom.Write(s, bw);
            s.WriteByteArray(bw, m_propertiesStream);
            s.WriteClassArray(bw, m_contactPoints);
            s.WriteByteArray(bw, m_cpIdMgr);
            s.WriteByteCStyleArray(bw, m_nnEntryData);
            m_trackInfo.Write(s, bw);
            s.WriteByteCStyleArray(bw, m_endianCheckBuffer);
            bw.WriteUInt32(m_version);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bodyA = xd.ReadClassPointer<hkpEntity>(xe, nameof(m_bodyA));
            m_bodyB = xd.ReadClassPointer<hkpEntity>(xe, nameof(m_bodyB));
            m_bodyAId = xd.ReadUInt64(xe, nameof(m_bodyAId));
            m_bodyBId = xd.ReadUInt64(xe, nameof(m_bodyBId));
            m_useEntityIds = xd.ReadBoolean(xe, nameof(m_useEntityIds));
            m_agentType = xd.ReadFlag<SerializedAgentType, sbyte>(xe, nameof(m_agentType));
            m_atom = xd.ReadClass<hkpSimpleContactConstraintAtom>(xe, nameof(m_atom));
            m_propertiesStream = xd.ReadByteArray(xe, nameof(m_propertiesStream));
            m_contactPoints = xd.ReadClassArray<hkContactPoint>(xe, nameof(m_contactPoints));
            m_cpIdMgr = xd.ReadByteArray(xe, nameof(m_cpIdMgr));
            m_nnEntryData = xd.ReadByteCStyleArray(xe, nameof(m_nnEntryData), 160);
            m_trackInfo = xd.ReadClass<hkpSerializedTrack1nInfo>(xe, nameof(m_trackInfo));
            m_endianCheckBuffer = xd.ReadByteCStyleArray(xe, nameof(m_endianCheckBuffer), 4);
            m_version = xd.ReadUInt32(xe, nameof(m_version));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_bodyA), m_bodyA);
            xs.WriteClassPointer(xe, nameof(m_bodyB), m_bodyB);
            xs.WriteNumber(xe, nameof(m_bodyAId), m_bodyAId);
            xs.WriteNumber(xe, nameof(m_bodyBId), m_bodyBId);
            xs.WriteBoolean(xe, nameof(m_useEntityIds), m_useEntityIds);
            xs.WriteEnum<SerializedAgentType, sbyte>(xe, nameof(m_agentType), m_agentType);
            xs.WriteClass<hkpSimpleContactConstraintAtom>(xe, nameof(m_atom), m_atom);
            xs.WriteNumberArray(xe, nameof(m_propertiesStream), m_propertiesStream);
            xs.WriteClassArray(xe, nameof(m_contactPoints), m_contactPoints);
            xs.WriteNumberArray(xe, nameof(m_cpIdMgr), m_cpIdMgr);
            xs.WriteNumberArray(xe, nameof(m_nnEntryData), m_nnEntryData);
            xs.WriteClass(xe, nameof(m_trackInfo), m_trackInfo);
            xs.WriteNumberArray(xe, nameof(m_endianCheckBuffer), m_endianCheckBuffer);
            xs.WriteNumber(xe, nameof(m_version), m_version);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedAgentNnEntry);
        }

        public bool Equals(hkpSerializedAgentNnEntry? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_bodyA is null && other.m_bodyA is null) || (m_bodyA is not null && other.m_bodyA is not null && m_bodyA.Equals((IHavokObject)other.m_bodyA))) &&
                   ((m_bodyB is null && other.m_bodyB is null) || (m_bodyB is not null && other.m_bodyB is not null && m_bodyB.Equals((IHavokObject)other.m_bodyB))) &&
                   m_bodyAId.Equals(other.m_bodyAId) &&
                   m_bodyBId.Equals(other.m_bodyBId) &&
                   m_useEntityIds.Equals(other.m_useEntityIds) &&
                   m_agentType.Equals(other.m_agentType) &&
                   ((m_atom is null && other.m_atom is null) || (m_atom is not null && other.m_atom is not null && m_atom.Equals((IHavokObject)other.m_atom))) &&
                   m_propertiesStream.SequenceEqual(other.m_propertiesStream) &&
                   m_contactPoints.SequenceEqual(other.m_contactPoints) &&
                   m_cpIdMgr.SequenceEqual(other.m_cpIdMgr) &&
                   m_nnEntryData.SequenceEqual(other.m_nnEntryData) &&
                   ((m_trackInfo is null && other.m_trackInfo is null) || (m_trackInfo is not null && other.m_trackInfo is not null && m_trackInfo.Equals((IHavokObject)other.m_trackInfo))) &&
                   m_endianCheckBuffer.SequenceEqual(other.m_endianCheckBuffer) &&
                   m_version.Equals(other.m_version) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bodyA);
            hashcode.Add(m_bodyB);
            hashcode.Add(m_bodyAId);
            hashcode.Add(m_bodyBId);
            hashcode.Add(m_useEntityIds);
            hashcode.Add(m_agentType);
            hashcode.Add(m_atom);
            hashcode.Add(m_propertiesStream.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_contactPoints.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_cpIdMgr.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_nnEntryData.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_trackInfo);
            hashcode.Add(m_endianCheckBuffer.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_version);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

