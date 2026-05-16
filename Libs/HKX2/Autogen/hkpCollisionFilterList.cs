using System.Xml.Linq;
namespace HKX2
{
    // hkpCollisionFilterList Signatire: 0x2603bf04 size: 88 flags: FLAGS_NONE

    // m_collisionFilters m_class: hkpCollisionFilter Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    public partial class hkpCollisionFilterList : hkpCollisionFilter, IEquatable<hkpCollisionFilterList?>
    {
        public IList<hkpCollisionFilter> m_collisionFilters { set; get; } = Array.Empty<hkpCollisionFilter>();

        public override uint Signature { set; get; } = 0x2603bf04;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collisionFilters = des.ReadClassPointerArray<hkpCollisionFilter>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_collisionFilters);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_collisionFilters = xd.ReadClassPointerArray<hkpCollisionFilter>(xe, nameof(m_collisionFilters));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_collisionFilters), m_collisionFilters);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCollisionFilterList);
        }

        public bool Equals(hkpCollisionFilterList? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_collisionFilters.SequenceEqual(other.m_collisionFilters) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_collisionFilters.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

