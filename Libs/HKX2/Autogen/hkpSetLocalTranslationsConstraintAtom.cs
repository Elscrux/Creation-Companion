using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSetLocalTranslationsConstraintAtom Signatire: 0x5cbfcf4a size: 48 flags: FLAGS_NONE

    // m_translationA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_translationB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpSetLocalTranslationsConstraintAtom : hkpConstraintAtom, IEquatable<hkpSetLocalTranslationsConstraintAtom?>
    {
        public Vector4 m_translationA { set; get; }
        public Vector4 m_translationB { set; get; }

        public override uint Signature { set; get; } = 0x5cbfcf4a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 14;
            m_translationA = br.ReadVector4();
            m_translationB = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 14;
            bw.WriteVector4(m_translationA);
            bw.WriteVector4(m_translationB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_translationA = xd.ReadVector4(xe, nameof(m_translationA));
            m_translationB = xd.ReadVector4(xe, nameof(m_translationB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_translationA), m_translationA);
            xs.WriteVector4(xe, nameof(m_translationB), m_translationB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSetLocalTranslationsConstraintAtom);
        }

        public bool Equals(hkpSetLocalTranslationsConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_translationA.Equals(other.m_translationA) &&
                   m_translationB.Equals(other.m_translationB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_translationA);
            hashcode.Add(m_translationB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

