using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpCenterOfMassChangerModifierConstraintAtom Signatire: 0x1d7dbdd2 size: 80 flags: FLAGS_NONE

    // m_displacementA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_displacementB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpCenterOfMassChangerModifierConstraintAtom : hkpModifierConstraintAtom, IEquatable<hkpCenterOfMassChangerModifierConstraintAtom?>
    {
        public Vector4 m_displacementA { set; get; }
        public Vector4 m_displacementB { set; get; }

        public override uint Signature { set; get; } = 0x1d7dbdd2;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_displacementA = br.ReadVector4();
            m_displacementB = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_displacementA);
            bw.WriteVector4(m_displacementB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_displacementA = xd.ReadVector4(xe, nameof(m_displacementA));
            m_displacementB = xd.ReadVector4(xe, nameof(m_displacementB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_displacementA), m_displacementA);
            xs.WriteVector4(xe, nameof(m_displacementB), m_displacementB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCenterOfMassChangerModifierConstraintAtom);
        }

        public bool Equals(hkpCenterOfMassChangerModifierConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_displacementA.Equals(other.m_displacementA) &&
                   m_displacementB.Equals(other.m_displacementB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_displacementA);
            hashcode.Add(m_displacementB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

