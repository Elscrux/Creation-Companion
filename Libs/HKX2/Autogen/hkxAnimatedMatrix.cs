using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxAnimatedMatrix Signatire: 0x5838e337 size: 40 flags: FLAGS_NONE

    // m_matrices m_class:  Type.TYPE_ARRAY Type.TYPE_MATRIX4 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_hint m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: Hint
    public partial class hkxAnimatedMatrix : hkReferencedObject, IEquatable<hkxAnimatedMatrix?>
    {
        public IList<Matrix4x4> m_matrices { set; get; } = Array.Empty<Matrix4x4>();
        public byte m_hint { set; get; }

        public override uint Signature { set; get; } = 0x5838e337;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_matrices = des.ReadMatrix4Array(br);
            m_hint = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteMatrix4Array(bw, m_matrices);
            bw.WriteByte(m_hint);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_matrices = xd.ReadMatrix4Array(xe, nameof(m_matrices));
            m_hint = xd.ReadFlag<Hint, byte>(xe, nameof(m_hint));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteMatrix4Array(xe, nameof(m_matrices), m_matrices);
            xs.WriteEnum<Hint, byte>(xe, nameof(m_hint), m_hint);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAnimatedMatrix);
        }

        public bool Equals(hkxAnimatedMatrix? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_matrices.SequenceEqual(other.m_matrices) &&
                   m_hint.Equals(other.m_hint) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_matrices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_hint);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

