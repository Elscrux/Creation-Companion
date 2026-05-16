using System.Xml.Linq;
namespace HKX2
{
    // hkpStiffSpringConstraintDataAtoms Signatire: 0x207eb376 size: 64 flags: FLAGS_NONE

    // m_pivots m_class: hkpSetLocalTranslationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_spring m_class: hkpStiffSpringConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpStiffSpringConstraintDataAtoms : IHavokObject, IEquatable<hkpStiffSpringConstraintDataAtoms?>
    {
        public hkpSetLocalTranslationsConstraintAtom m_pivots { set; get; } = new();
        public hkpStiffSpringConstraintAtom m_spring { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x207eb376;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivots.Read(des, br);
            m_spring.Read(des, br);
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_pivots.Write(s, bw);
            m_spring.Write(s, bw);
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_pivots = xd.ReadClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_pivots));
            m_spring = xd.ReadClass<hkpStiffSpringConstraintAtom>(xe, nameof(m_spring));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_pivots), m_pivots);
            xs.WriteClass(xe, nameof(m_spring), m_spring);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStiffSpringConstraintDataAtoms);
        }

        public bool Equals(hkpStiffSpringConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_pivots is null && other.m_pivots is null) || (m_pivots is not null && other.m_pivots is not null && m_pivots.Equals((IHavokObject)other.m_pivots))) &&
                   ((m_spring is null && other.m_spring is null) || (m_spring is not null && other.m_spring is not null && m_spring.Equals((IHavokObject)other.m_spring))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_pivots);
            hashcode.Add(m_spring);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

