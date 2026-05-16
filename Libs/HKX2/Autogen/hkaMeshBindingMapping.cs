using System.Xml.Linq;
namespace HKX2
{
    // hkaMeshBindingMapping Signatire: 0x48aceb75 size: 16 flags: FLAGS_NONE

    // m_mapping m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkaMeshBindingMapping : IHavokObject, IEquatable<hkaMeshBindingMapping?>
    {
        public IList<short> m_mapping { set; get; } = Array.Empty<short>();

        public virtual uint Signature { set; get; } = 0x48aceb75;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_mapping = des.ReadInt16Array(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteInt16Array(bw, m_mapping);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_mapping = xd.ReadInt16Array(xe, nameof(m_mapping));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumberArray(xe, nameof(m_mapping), m_mapping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaMeshBindingMapping);
        }

        public bool Equals(hkaMeshBindingMapping? other)
        {
            return other is not null &&
                   m_mapping.SequenceEqual(other.m_mapping) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_mapping.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

