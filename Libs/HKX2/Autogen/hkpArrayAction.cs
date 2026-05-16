using System.Xml.Linq;
namespace HKX2
{
    // hkpArrayAction Signatire: 0x674bcd2d size: 64 flags: FLAGS_NONE

    // m_entities m_class: hkpEntity Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpArrayAction : hkpAction, IEquatable<hkpArrayAction?>
    {
        public IList<hkpEntity> m_entities { set; get; } = Array.Empty<hkpEntity>();

        public override uint Signature { set; get; } = 0x674bcd2d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_entities = des.ReadClassPointerArray<hkpEntity>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_entities);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_entities = xd.ReadClassPointerArray<hkpEntity>(xe, nameof(m_entities));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_entities), m_entities);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpArrayAction);
        }

        public bool Equals(hkpArrayAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_entities.SequenceEqual(other.m_entities) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_entities.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

