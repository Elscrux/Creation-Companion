using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexFloatDataChannel Signatire: 0xbeeb397c size: 40 flags: FLAGS_NONE

    // m_perVertexFloats m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_dimensions m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: VertexFloatDimensions
    public partial class hkxVertexFloatDataChannel : hkReferencedObject, IEquatable<hkxVertexFloatDataChannel?>
    {
        public IList<float> m_perVertexFloats { set; get; } = Array.Empty<float>();
        public byte m_dimensions { set; get; }

        public override uint Signature { set; get; } = 0xbeeb397c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_perVertexFloats = des.ReadSingleArray(br);
            m_dimensions = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_perVertexFloats);
            bw.WriteByte(m_dimensions);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_perVertexFloats = xd.ReadSingleArray(xe, nameof(m_perVertexFloats));
            m_dimensions = xd.ReadFlag<VertexFloatDimensions, byte>(xe, nameof(m_dimensions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloatArray(xe, nameof(m_perVertexFloats), m_perVertexFloats);
            xs.WriteEnum<VertexFloatDimensions, byte>(xe, nameof(m_dimensions), m_dimensions);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexFloatDataChannel);
        }

        public bool Equals(hkxVertexFloatDataChannel? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_perVertexFloats.SequenceEqual(other.m_perVertexFloats) &&
                   m_dimensions.Equals(other.m_dimensions) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_perVertexFloats.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_dimensions);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

