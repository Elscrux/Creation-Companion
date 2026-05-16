using System.Xml.Linq;
namespace HKX2
{
    // hkBitField Signatire: 0xda41bd9b size: 24 flags: FLAGS_NONE

    // m_words m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_numBits m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkBitField : IHavokObject, IEquatable<hkBitField?>
    {
        public IList<uint> m_words { set; get; } = Array.Empty<uint>();
        public int m_numBits { set; get; }

        public virtual uint Signature { set; get; } = 0xda41bd9b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_words = des.ReadUInt32Array(br);
            m_numBits = br.ReadInt32();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteUInt32Array(bw, m_words);
            bw.WriteInt32(m_numBits);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_words = xd.ReadUInt32Array(xe, nameof(m_words));
            m_numBits = xd.ReadInt32(xe, nameof(m_numBits));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumberArray(xe, nameof(m_words), m_words);
            xs.WriteNumber(xe, nameof(m_numBits), m_numBits);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkBitField);
        }

        public bool Equals(hkBitField? other)
        {
            return other is not null &&
                   m_words.SequenceEqual(other.m_words) &&
                   m_numBits.Equals(other.m_numBits) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_words.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_numBits);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

