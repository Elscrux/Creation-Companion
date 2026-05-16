using System.Xml.Linq;
namespace HKX2
{
    // hkpPrismaticConstraintDataAtoms Signatire: 0x7f516137 size: 208 flags: FLAGS_NONE

    // m_transforms m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_motor m_class: hkpLinMotorConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_friction m_class: hkpLinFrictionConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_ang m_class: hkpAngConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_lin0 m_class: hkpLinConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 180 flags: FLAGS_NONE enum: 
    // m_lin1 m_class: hkpLinConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_linLimit m_class: hkpLinLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 188 flags: FLAGS_NONE enum: 
    public partial class hkpPrismaticConstraintDataAtoms : IHavokObject, IEquatable<hkpPrismaticConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms { set; get; } = new();
        public hkpLinMotorConstraintAtom m_motor { set; get; } = new();
        public hkpLinFrictionConstraintAtom m_friction { set; get; } = new();
        public hkpAngConstraintAtom m_ang { set; get; } = new();
        public hkpLinConstraintAtom m_lin0 { set; get; } = new();
        public hkpLinConstraintAtom m_lin1 { set; get; } = new();
        public hkpLinLimitConstraintAtom m_linLimit { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x7f516137;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms.Read(des, br);
            m_motor.Read(des, br);
            m_friction.Read(des, br);
            m_ang.Read(des, br);
            m_lin0.Read(des, br);
            m_lin1.Read(des, br);
            m_linLimit.Read(des, br);
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transforms.Write(s, bw);
            m_motor.Write(s, bw);
            m_friction.Write(s, bw);
            m_ang.Write(s, bw);
            m_lin0.Write(s, bw);
            m_lin1.Write(s, bw);
            m_linLimit.Write(s, bw);
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transforms = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms));
            m_motor = xd.ReadClass<hkpLinMotorConstraintAtom>(xe, nameof(m_motor));
            m_friction = xd.ReadClass<hkpLinFrictionConstraintAtom>(xe, nameof(m_friction));
            m_ang = xd.ReadClass<hkpAngConstraintAtom>(xe, nameof(m_ang));
            m_lin0 = xd.ReadClass<hkpLinConstraintAtom>(xe, nameof(m_lin0));
            m_lin1 = xd.ReadClass<hkpLinConstraintAtom>(xe, nameof(m_lin1));
            m_linLimit = xd.ReadClass<hkpLinLimitConstraintAtom>(xe, nameof(m_linLimit));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms), m_transforms);
            xs.WriteClass(xe, nameof(m_motor), m_motor);
            xs.WriteClass<hkpLinFrictionConstraintAtom>(xe, nameof(m_friction), m_friction);
            xs.WriteClass(xe, nameof(m_ang), m_ang);
            xs.WriteClass<hkpLinConstraintAtom>(xe, nameof(m_lin0), m_lin0);
            xs.WriteClass(xe, nameof(m_lin1), m_lin1);
            xs.WriteClass<hkpLinLimitConstraintAtom>(xe, nameof(m_linLimit), m_linLimit);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPrismaticConstraintDataAtoms);
        }

        public bool Equals(hkpPrismaticConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_transforms is null && other.m_transforms is null) || (m_transforms is not null && other.m_transforms is not null && m_transforms.Equals((IHavokObject)other.m_transforms))) &&
                   ((m_motor is null && other.m_motor is null) || (m_motor is not null && other.m_motor is not null && m_motor.Equals((IHavokObject)other.m_motor))) &&
                   ((m_friction is null && other.m_friction is null) || (m_friction is not null && other.m_friction is not null && m_friction.Equals((IHavokObject)other.m_friction))) &&
                   ((m_ang is null && other.m_ang is null) || (m_ang is not null && other.m_ang is not null && m_ang.Equals((IHavokObject)other.m_ang))) &&
                   ((m_lin0 is null && other.m_lin0 is null) || (m_lin0 is not null && other.m_lin0 is not null && m_lin0.Equals((IHavokObject)other.m_lin0))) &&
                   ((m_lin1 is null && other.m_lin1 is null) || (m_lin1 is not null && other.m_lin1 is not null && m_lin1.Equals((IHavokObject)other.m_lin1))) &&
                   ((m_linLimit is null && other.m_linLimit is null) || (m_linLimit is not null && other.m_linLimit is not null && m_linLimit.Equals((IHavokObject)other.m_linLimit))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transforms);
            hashcode.Add(m_motor);
            hashcode.Add(m_friction);
            hashcode.Add(m_ang);
            hashcode.Add(m_lin0);
            hashcode.Add(m_lin1);
            hashcode.Add(m_linLimit);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

