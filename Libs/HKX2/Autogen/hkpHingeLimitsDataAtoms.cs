using System.Xml.Linq;
namespace HKX2
{
    // hkpHingeLimitsDataAtoms Signatire: 0x555876ff size: 144 flags: FLAGS_NONE

    // m_rotations m_class: hkpSetLocalRotationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_angLimit m_class: hkpAngLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_2dAng m_class: hkp2dAngConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    public partial class hkpHingeLimitsDataAtoms : IHavokObject, IEquatable<hkpHingeLimitsDataAtoms?>
    {
        public hkpSetLocalRotationsConstraintAtom m_rotations { set; get; } = new();
        public hkpAngLimitConstraintAtom m_angLimit { set; get; } = new();
        public hkp2dAngConstraintAtom m_2dAng { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x555876ff;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotations.Read(des, br);
            m_angLimit.Read(des, br);
            m_2dAng.Read(des, br);
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_rotations.Write(s, bw);
            m_angLimit.Write(s, bw);
            m_2dAng.Write(s, bw);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rotations = xd.ReadClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations));
            m_angLimit = xd.ReadClass<hkpAngLimitConstraintAtom>(xe, nameof(m_angLimit));
            m_2dAng = xd.ReadClass<hkp2dAngConstraintAtom>(xe, nameof(m_2dAng));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations), m_rotations);
            xs.WriteClass(xe, nameof(m_angLimit), m_angLimit);
            xs.WriteClass<hkp2dAngConstraintAtom>(xe, nameof(m_2dAng), m_2dAng);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpHingeLimitsDataAtoms);
        }

        public bool Equals(hkpHingeLimitsDataAtoms? other)
        {
            return other is not null &&
                   ((m_rotations is null && other.m_rotations is null) || (m_rotations is not null && other.m_rotations is not null && m_rotations.Equals((IHavokObject)other.m_rotations))) &&
                   ((m_angLimit is null && other.m_angLimit is null) || (m_angLimit is not null && other.m_angLimit is not null && m_angLimit.Equals((IHavokObject)other.m_angLimit))) &&
                   ((m_2dAng is null && other.m_2dAng is null) || (m_2dAng is not null && other.m_2dAng is not null && m_2dAng.Equals((IHavokObject)other.m_2dAng))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rotations);
            hashcode.Add(m_angLimit);
            hashcode.Add(m_2dAng);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

