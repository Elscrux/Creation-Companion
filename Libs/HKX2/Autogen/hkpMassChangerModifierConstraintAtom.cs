using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMassChangerModifierConstraintAtom Signatire: 0xb6b28240 size: 80 flags: FLAGS_NONE

    // m_factorA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_factorB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpMassChangerModifierConstraintAtom : hkpModifierConstraintAtom, IEquatable<hkpMassChangerModifierConstraintAtom?>
    {
        public Vector4 m_factorA { set; get; }
        public Vector4 m_factorB { set; get; }

        public override uint Signature { set; get; } = 0xb6b28240;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_factorA = br.ReadVector4();
            m_factorB = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_factorA);
            bw.WriteVector4(m_factorB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_factorA = xd.ReadVector4(xe, nameof(m_factorA));
            m_factorB = xd.ReadVector4(xe, nameof(m_factorB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_factorA), m_factorA);
            xs.WriteVector4(xe, nameof(m_factorB), m_factorB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMassChangerModifierConstraintAtom);
        }

        public bool Equals(hkpMassChangerModifierConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_factorA.Equals(other.m_factorA) &&
                   m_factorB.Equals(other.m_factorB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_factorA);
            hashcode.Add(m_factorB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

