using System.Xml.Linq;
namespace HKX2
{
    // hkpCollidable Signatire: 0x9a0e42a5 size: 112 flags: FLAGS_NONE

    // m_ownerOffset m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 32 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_forceCollideOntoPpu m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 33 flags: FLAGS_NONE enum: 
    // m_shapeSizeOnSpu m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 34 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_broadPhaseHandle m_class: hkpTypedBroadPhaseHandle Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_boundingVolumeData m_class: hkpCollidableBoundingVolumeData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_allowedPenetrationDepth m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    public partial class hkpCollidable : hkpCdBody, IEquatable<hkpCollidable?>
    {
        private sbyte m_ownerOffset { set; get; }
        public byte m_forceCollideOntoPpu { set; get; }
        private ushort m_shapeSizeOnSpu { set; get; }
        public hkpTypedBroadPhaseHandle m_broadPhaseHandle { set; get; } = new();
        public hkpCollidableBoundingVolumeData m_boundingVolumeData { set; get; } = new();
        public float m_allowedPenetrationDepth { set; get; }

        public override uint Signature { set; get; } = 0x9a0e42a5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_ownerOffset = br.ReadSByte();
            m_forceCollideOntoPpu = br.ReadByte();
            m_shapeSizeOnSpu = br.ReadUInt16();
            m_broadPhaseHandle.Read(des, br);
            m_boundingVolumeData.Read(des, br);
            m_allowedPenetrationDepth = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSByte(m_ownerOffset);
            bw.WriteByte(m_forceCollideOntoPpu);
            bw.WriteUInt16(m_shapeSizeOnSpu);
            m_broadPhaseHandle.Write(s, bw);
            m_boundingVolumeData.Write(s, bw);
            bw.WriteSingle(m_allowedPenetrationDepth);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_forceCollideOntoPpu = xd.ReadByte(xe, nameof(m_forceCollideOntoPpu));
            m_broadPhaseHandle = xd.ReadClass<hkpTypedBroadPhaseHandle>(xe, nameof(m_broadPhaseHandle));
            m_allowedPenetrationDepth = xd.ReadSingle(xe, nameof(m_allowedPenetrationDepth));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_ownerOffset));
            xs.WriteNumber(xe, nameof(m_forceCollideOntoPpu), m_forceCollideOntoPpu);
            xs.WriteSerializeIgnored(xe, nameof(m_shapeSizeOnSpu));
            xs.WriteClass<hkpTypedBroadPhaseHandle>(xe, nameof(m_broadPhaseHandle), m_broadPhaseHandle);
            xs.WriteSerializeIgnored(xe, nameof(m_boundingVolumeData));
            xs.WriteFloat(xe, nameof(m_allowedPenetrationDepth), m_allowedPenetrationDepth);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCollidable);
        }

        public bool Equals(hkpCollidable? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_forceCollideOntoPpu.Equals(other.m_forceCollideOntoPpu) &&
                   ((m_broadPhaseHandle is null && other.m_broadPhaseHandle is null) || (m_broadPhaseHandle is not null && other.m_broadPhaseHandle is not null && m_broadPhaseHandle.Equals((IHavokObject)other.m_broadPhaseHandle))) &&
                   m_allowedPenetrationDepth.Equals(other.m_allowedPenetrationDepth) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_forceCollideOntoPpu);
            hashcode.Add(m_broadPhaseHandle);
            hashcode.Add(m_allowedPenetrationDepth);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

