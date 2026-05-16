using System.Xml.Linq;
namespace HKX2
{
    // hkpPairCollisionFilter Signatire: 0x4abc140e size: 96 flags: FLAGS_NONE

    // m_disabledPairs m_class: hkpPairCollisionFilterMapPairFilterKeyOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 72 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_childFilter m_class: hkpCollisionFilter Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    public partial class hkpPairCollisionFilter : hkpCollisionFilter, IEquatable<hkpPairCollisionFilter?>
    {
        public hkpPairCollisionFilterMapPairFilterKeyOverrideType m_disabledPairs { set; get; } = new();
        public hkpCollisionFilter? m_childFilter { set; get; }

        public override uint Signature { set; get; } = 0x4abc140e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_disabledPairs.Read(des, br);
            m_childFilter = des.ReadClassPointer<hkpCollisionFilter>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_disabledPairs.Write(s, bw);
            s.WriteClassPointer(bw, m_childFilter);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childFilter = xd.ReadClassPointer<hkpCollisionFilter>(xe, nameof(m_childFilter));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_disabledPairs));
            xs.WriteClassPointer(xe, nameof(m_childFilter), m_childFilter);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPairCollisionFilter);
        }

        public bool Equals(hkpPairCollisionFilter? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_childFilter is null && other.m_childFilter is null) || (m_childFilter is not null && other.m_childFilter is not null && m_childFilter.Equals((IHavokObject)other.m_childFilter))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_childFilter);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

