using System.Xml.Linq;
namespace HKX2
{
    // hkpWheelConstraintDataAtoms Signatire: 0x1188cbe1 size: 304 flags: FLAGS_NONE

    // m_suspensionBase m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_lin0Limit m_class: hkpLinLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_lin0Soft m_class: hkpLinSoftConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 156 flags: FLAGS_NONE enum: 
    // m_lin1 m_class: hkpLinConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_lin2 m_class: hkpLinConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 172 flags: FLAGS_NONE enum: 
    // m_steeringBase m_class: hkpSetLocalRotationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_2dAng m_class: hkp2dAngConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 288 flags: FLAGS_NONE enum: 
    public partial class hkpWheelConstraintDataAtoms : IHavokObject, IEquatable<hkpWheelConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_suspensionBase { set; get; } = new();
        public hkpLinLimitConstraintAtom m_lin0Limit { set; get; } = new();
        public hkpLinSoftConstraintAtom m_lin0Soft { set; get; } = new();
        public hkpLinConstraintAtom m_lin1 { set; get; } = new();
        public hkpLinConstraintAtom m_lin2 { set; get; } = new();
        public hkpSetLocalRotationsConstraintAtom m_steeringBase { set; get; } = new();
        public hkp2dAngConstraintAtom m_2dAng { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x1188cbe1;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_suspensionBase.Read(des, br);
            m_lin0Limit.Read(des, br);
            m_lin0Soft.Read(des, br);
            m_lin1.Read(des, br);
            m_lin2.Read(des, br);
            m_steeringBase.Read(des, br);
            m_2dAng.Read(des, br);
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_suspensionBase.Write(s, bw);
            m_lin0Limit.Write(s, bw);
            m_lin0Soft.Write(s, bw);
            m_lin1.Write(s, bw);
            m_lin2.Write(s, bw);
            m_steeringBase.Write(s, bw);
            m_2dAng.Write(s, bw);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_suspensionBase = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_suspensionBase));
            m_lin0Limit = xd.ReadClass<hkpLinLimitConstraintAtom>(xe, nameof(m_lin0Limit));
            m_lin0Soft = xd.ReadClass<hkpLinSoftConstraintAtom>(xe, nameof(m_lin0Soft));
            m_lin1 = xd.ReadClass<hkpLinConstraintAtom>(xe, nameof(m_lin1));
            m_lin2 = xd.ReadClass<hkpLinConstraintAtom>(xe, nameof(m_lin2));
            m_steeringBase = xd.ReadClass<hkpSetLocalRotationsConstraintAtom>(xe, nameof(m_steeringBase));
            m_2dAng = xd.ReadClass<hkp2dAngConstraintAtom>(xe, nameof(m_2dAng));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_suspensionBase), m_suspensionBase);
            xs.WriteClass(xe, nameof(m_lin0Limit), m_lin0Limit);
            xs.WriteClass<hkpLinSoftConstraintAtom>(xe, nameof(m_lin0Soft), m_lin0Soft);
            xs.WriteClass(xe, nameof(m_lin1), m_lin1);
            xs.WriteClass<hkpLinConstraintAtom>(xe, nameof(m_lin2), m_lin2);
            xs.WriteClass(xe, nameof(m_steeringBase), m_steeringBase);
            xs.WriteClass<hkp2dAngConstraintAtom>(xe, nameof(m_2dAng), m_2dAng);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpWheelConstraintDataAtoms);
        }

        public bool Equals(hkpWheelConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_suspensionBase is null && other.m_suspensionBase is null) || (m_suspensionBase is not null && other.m_suspensionBase is not null && m_suspensionBase.Equals((IHavokObject)other.m_suspensionBase))) &&
                   ((m_lin0Limit is null && other.m_lin0Limit is null) || (m_lin0Limit is not null && other.m_lin0Limit is not null && m_lin0Limit.Equals((IHavokObject)other.m_lin0Limit))) &&
                   ((m_lin0Soft is null && other.m_lin0Soft is null) || (m_lin0Soft is not null && other.m_lin0Soft is not null && m_lin0Soft.Equals((IHavokObject)other.m_lin0Soft))) &&
                   ((m_lin1 is null && other.m_lin1 is null) || (m_lin1 is not null && other.m_lin1 is not null && m_lin1.Equals((IHavokObject)other.m_lin1))) &&
                   ((m_lin2 is null && other.m_lin2 is null) || (m_lin2 is not null && other.m_lin2 is not null && m_lin2.Equals((IHavokObject)other.m_lin2))) &&
                   ((m_steeringBase is null && other.m_steeringBase is null) || (m_steeringBase is not null && other.m_steeringBase is not null && m_steeringBase.Equals((IHavokObject)other.m_steeringBase))) &&
                   ((m_2dAng is null && other.m_2dAng is null) || (m_2dAng is not null && other.m_2dAng is not null && m_2dAng.Equals((IHavokObject)other.m_2dAng))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_suspensionBase);
            hashcode.Add(m_lin0Limit);
            hashcode.Add(m_lin0Soft);
            hashcode.Add(m_lin1);
            hashcode.Add(m_lin2);
            hashcode.Add(m_steeringBase);
            hashcode.Add(m_2dAng);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

