using System.Xml.Linq;
namespace HKX2
{
    // hkpRagdollLimitsDataAtoms Signatire: 0x82b894c3 size: 176 flags: FLAGS_NONE

    // m_rotations m_class: hkpSetLocalRotationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_twistLimit m_class: hkpTwistLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_coneLimit m_class: hkpConeLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 132 flags: FLAGS_NONE enum: 
    // m_planesLimit m_class: hkpConeLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    public partial class hkpRagdollLimitsDataAtoms : IHavokObject, IEquatable<hkpRagdollLimitsDataAtoms?>
    {
        public hkpSetLocalRotationsConstraintAtom m_rotations { set; get; } = new();
        public hkpTwistLimitConstraintAtom m_twistLimit { set; get; } = new();
        public hkpConeLimitConstraintAtom m_coneLimit { set; get; } = new();
        public hkpConeLimitConstraintAtom m_planesLimit { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x82b894c3;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotations.Read(des, br);
            m_twistLimit.Read(des, br);
            m_coneLimit.Read(des, br);
            m_planesLimit.Read(des, br);
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_rotations.Write(s, bw);
            m_twistLimit.Write(s, bw);
            m_coneLimit.Write(s, bw);
            m_planesLimit.Write(s, bw);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rotations = xd.ReadClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations));
            m_twistLimit = xd.ReadClass<hkpTwistLimitConstraintAtom>(xe, nameof(m_twistLimit));
            m_coneLimit = xd.ReadClass<hkpConeLimitConstraintAtom>(xe, nameof(m_coneLimit));
            m_planesLimit = xd.ReadClass<hkpConeLimitConstraintAtom>(xe, nameof(m_planesLimit));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_rotations), m_rotations);
            xs.WriteClass(xe, nameof(m_twistLimit), m_twistLimit);
            xs.WriteClass<hkpConeLimitConstraintAtom>(xe, nameof(m_coneLimit), m_coneLimit);
            xs.WriteClass(xe, nameof(m_planesLimit), m_planesLimit);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRagdollLimitsDataAtoms);
        }

        public bool Equals(hkpRagdollLimitsDataAtoms? other)
        {
            return other is not null &&
                   ((m_rotations is null && other.m_rotations is null) || (m_rotations is not null && other.m_rotations is not null && m_rotations.Equals((IHavokObject)other.m_rotations))) &&
                   ((m_twistLimit is null && other.m_twistLimit is null) || (m_twistLimit is not null && other.m_twistLimit is not null && m_twistLimit.Equals((IHavokObject)other.m_twistLimit))) &&
                   ((m_coneLimit is null && other.m_coneLimit is null) || (m_coneLimit is not null && other.m_coneLimit is not null && m_coneLimit.Equals((IHavokObject)other.m_coneLimit))) &&
                   ((m_planesLimit is null && other.m_planesLimit is null) || (m_planesLimit is not null && other.m_planesLimit is not null && m_planesLimit.Equals((IHavokObject)other.m_planesLimit))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rotations);
            hashcode.Add(m_twistLimit);
            hashcode.Add(m_coneLimit);
            hashcode.Add(m_planesLimit);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

