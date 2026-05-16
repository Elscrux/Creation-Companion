using System.Xml.Linq;
namespace HKX2
{
    // hkxEnum Signatire: 0xc4e1211 size: 32 flags: FLAGS_NONE

    // m_items m_class: hkxEnumItem Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxEnum : hkReferencedObject, IEquatable<hkxEnum?>
    {
        public IList<hkxEnumItem> m_items { set; get; } = Array.Empty<hkxEnumItem>();

        public override uint Signature { set; get; } = 0xc4e1211;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_items = des.ReadClassArray<hkxEnumItem>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_items);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_items = xd.ReadClassArray<hkxEnumItem>(xe, nameof(m_items));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_items), m_items);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxEnum);
        }

        public bool Equals(hkxEnum? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_items.SequenceEqual(other.m_items) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_items.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

