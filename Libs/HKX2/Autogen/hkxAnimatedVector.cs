using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxAnimatedVector Signatire: 0x34b1a197 size: 40 flags: FLAGS_NONE

    // m_vectors m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_hint m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: Hint
    public partial class hkxAnimatedVector : hkReferencedObject, IEquatable<hkxAnimatedVector?>
    {
        public IList<Vector4> m_vectors { set; get; } = Array.Empty<Vector4>();
        public byte m_hint { set; get; }

        public override uint Signature { set; get; } = 0x34b1a197;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vectors = des.ReadVector4Array(br);
            m_hint = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVector4Array(bw, m_vectors);
            bw.WriteByte(m_hint);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vectors = xd.ReadVector4Array(xe, nameof(m_vectors));
            m_hint = xd.ReadFlag<Hint, byte>(xe, nameof(m_hint));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4Array(xe, nameof(m_vectors), m_vectors);
            xs.WriteEnum<Hint, byte>(xe, nameof(m_hint), m_hint);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAnimatedVector);
        }

        public bool Equals(hkxAnimatedVector? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_vectors.SequenceEqual(other.m_vectors) &&
                   m_hint.Equals(other.m_hint) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vectors.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_hint);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

