using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSetLocalTransformsConstraintAtom Signatire: 0x6e2a5198 size: 144 flags: FLAGS_NONE

    // m_transformA m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_transformB m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkpSetLocalTransformsConstraintAtom : hkpConstraintAtom, IEquatable<hkpSetLocalTransformsConstraintAtom?>
    {
        public Matrix4x4 m_transformA { set; get; }
        public Matrix4x4 m_transformB { set; get; }

        public override uint Signature { set; get; } = 0x6e2a5198;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 14;
            m_transformA = des.ReadTransform(br);
            m_transformB = des.ReadTransform(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 14;
            s.WriteTransform(bw, m_transformA);
            s.WriteTransform(bw, m_transformB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transformA = xd.ReadTransform(xe, nameof(m_transformA));
            m_transformB = xd.ReadTransform(xe, nameof(m_transformB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteTransform(xe, nameof(m_transformA), m_transformA);
            xs.WriteTransform(xe, nameof(m_transformB), m_transformB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSetLocalTransformsConstraintAtom);
        }

        public bool Equals(hkpSetLocalTransformsConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transformA.Equals(other.m_transformA) &&
                   m_transformB.Equals(other.m_transformB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transformA);
            hashcode.Add(m_transformB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

