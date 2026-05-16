using System.Xml.Linq;
namespace HKX2
{
    // hkpRotationalConstraintDataAtoms Signatire: 0xa0c64586 size: 128 flags: FLAGS_NONE

    // m_rotations m_class: hkpSetLocalRotationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_ang m_class: hkpAngConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkpRotationalConstraintDataAtoms : IHavokObject, IEquatable<hkpRotationalConstraintDataAtoms?>
    {
        public hkpSetLocalRotationsConstraintAtom m_rotations { set; get; } = new();
        public hkpAngConstraintAtom m_ang { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xa0c64586;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotations.Read(des, br);
            m_ang.Read(des, br);
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_rotations.Write(s, bw);
            m_ang.Write(s, bw);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rotations = xd.ReadClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations));
            m_ang = xd.ReadClass<hkpAngConstraintAtom>(xe, nameof(m_ang));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations), m_rotations);
            xs.WriteClass(xe, nameof(m_ang), m_ang);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRotationalConstraintDataAtoms);
        }

        public bool Equals(hkpRotationalConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_rotations is null && other.m_rotations is null) || (m_rotations is not null && other.m_rotations is not null && m_rotations.Equals((IHavokObject)other.m_rotations))) &&
                   ((m_ang is null && other.m_ang is null) || (m_ang is not null && other.m_ang is not null && m_ang.Equals((IHavokObject)other.m_ang))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rotations);
            hashcode.Add(m_ang);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

