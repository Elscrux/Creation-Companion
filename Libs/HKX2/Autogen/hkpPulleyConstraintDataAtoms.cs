using System.Xml.Linq;
namespace HKX2
{
    // hkpPulleyConstraintDataAtoms Signatire: 0xb149e5a size: 112 flags: FLAGS_NONE

    // m_translations m_class: hkpSetLocalTranslationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_pulley m_class: hkpPulleyConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpPulleyConstraintDataAtoms : IHavokObject, IEquatable<hkpPulleyConstraintDataAtoms?>
    {
        public hkpSetLocalTranslationsConstraintAtom m_translations { set; get; } = new();
        public hkpPulleyConstraintAtom m_pulley { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xb149e5a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_translations.Read(des, br);
            m_pulley.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_translations.Write(s, bw);
            m_pulley.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_translations = xd.ReadClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_translations));
            m_pulley = xd.ReadClass<hkpPulleyConstraintAtom>(xe, nameof(m_pulley));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_translations), m_translations);
            xs.WriteClass(xe, nameof(m_pulley), m_pulley);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPulleyConstraintDataAtoms);
        }

        public bool Equals(hkpPulleyConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_translations is null && other.m_translations is null) || (m_translations is not null && other.m_translations is not null && m_translations.Equals((IHavokObject)other.m_translations))) &&
                   ((m_pulley is null && other.m_pulley is null) || (m_pulley is not null && other.m_pulley is not null && m_pulley.Equals((IHavokObject)other.m_pulley))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_translations);
            hashcode.Add(m_pulley);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

