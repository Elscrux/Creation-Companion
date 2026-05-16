using System.Xml.Linq;
namespace HKX2
{
    // hkpDisableEntityCollisionFilter Signatire: 0xfac3351c size: 96 flags: FLAGS_NONE

    // m_disabledEntities m_class: hkpEntity Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkpDisableEntityCollisionFilter : hkpCollisionFilter, IEquatable<hkpDisableEntityCollisionFilter?>
    {
        public IList<hkpEntity> m_disabledEntities { set; get; } = Array.Empty<hkpEntity>();

        public override uint Signature { set; get; } = 0xfac3351c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_disabledEntities = des.ReadClassPointerArray<hkpEntity>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteClassPointerArray(bw, m_disabledEntities);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_disabledEntities = xd.ReadClassPointerArray<hkpEntity>(xe, nameof(m_disabledEntities));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_disabledEntities), m_disabledEntities);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpDisableEntityCollisionFilter);
        }

        public bool Equals(hkpDisableEntityCollisionFilter? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_disabledEntities.SequenceEqual(other.m_disabledEntities) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_disabledEntities.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

