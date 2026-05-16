using System.Xml.Linq;
namespace HKX2
{
    // hkpLimitedHingeConstraintDataAtoms Signatire: 0x54c7715b size: 240 flags: FLAGS_NONE

    // m_transforms m_class: hkpSetLocalTransformsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_setupStabilization m_class: hkpSetupStabilizationAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_angMotor m_class: hkpAngMotorConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_angFriction m_class: hkpAngFrictionConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_angLimit m_class: hkpAngLimitConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_2dAng m_class: hkp2dAngConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 212 flags: FLAGS_NONE enum: 
    // m_ballSocket m_class: hkpBallSocketConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 216 flags: FLAGS_NONE enum: 
    public partial class hkpLimitedHingeConstraintDataAtoms : IHavokObject, IEquatable<hkpLimitedHingeConstraintDataAtoms?>
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms { set; get; } = new();
        public hkpSetupStabilizationAtom m_setupStabilization { set; get; } = new();
        public hkpAngMotorConstraintAtom m_angMotor { set; get; } = new();
        public hkpAngFrictionConstraintAtom m_angFriction { set; get; } = new();
        public hkpAngLimitConstraintAtom m_angLimit { set; get; } = new();
        public hkp2dAngConstraintAtom m_2dAng { set; get; } = new();
        public hkpBallSocketConstraintAtom m_ballSocket { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x54c7715b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms.Read(des, br);
            m_setupStabilization.Read(des, br);
            m_angMotor.Read(des, br);
            m_angFriction.Read(des, br);
            m_angLimit.Read(des, br);
            m_2dAng.Read(des, br);
            m_ballSocket.Read(des, br);
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transforms.Write(s, bw);
            m_setupStabilization.Write(s, bw);
            m_angMotor.Write(s, bw);
            m_angFriction.Write(s, bw);
            m_angLimit.Write(s, bw);
            m_2dAng.Write(s, bw);
            m_ballSocket.Write(s, bw);
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transforms = xd.ReadClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms));
            m_setupStabilization = xd.ReadClass<hkpSetupStabilizationAtom>(xe, nameof(m_setupStabilization));
            m_angMotor = xd.ReadClass<hkpAngMotorConstraintAtom>(xe, nameof(m_angMotor));
            m_angFriction = xd.ReadClass<hkpAngFrictionConstraintAtom>(xe, nameof(m_angFriction));
            m_angLimit = xd.ReadClass<hkpAngLimitConstraintAtom>(xe, nameof(m_angLimit));
            m_2dAng = xd.ReadClass<hkp2dAngConstraintAtom>(xe, nameof(m_2dAng));
            m_ballSocket = xd.ReadClass<hkpBallSocketConstraintAtom>(xe, nameof(m_ballSocket));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTransformsConstraintAtom>(xe, nameof(m_transforms), m_transforms);
            xs.WriteClass(xe, nameof(m_setupStabilization), m_setupStabilization);
            xs.WriteClass<hkpAngMotorConstraintAtom>(xe, nameof(m_angMotor), m_angMotor);
            xs.WriteClass(xe, nameof(m_angFriction), m_angFriction);
            xs.WriteClass<hkpAngLimitConstraintAtom>(xe, nameof(m_angLimit), m_angLimit);
            xs.WriteClass(xe, nameof(m_2dAng), m_2dAng);
            xs.WriteClass<hkpBallSocketConstraintAtom>(xe, nameof(m_ballSocket), m_ballSocket);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLimitedHingeConstraintDataAtoms);
        }

        public bool Equals(hkpLimitedHingeConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_transforms is null && other.m_transforms is null) || (m_transforms is not null && other.m_transforms is not null && m_transforms.Equals((IHavokObject)other.m_transforms))) &&
                   ((m_setupStabilization is null && other.m_setupStabilization is null) || (m_setupStabilization is not null && other.m_setupStabilization is not null && m_setupStabilization.Equals((IHavokObject)other.m_setupStabilization))) &&
                   ((m_angMotor is null && other.m_angMotor is null) || (m_angMotor is not null && other.m_angMotor is not null && m_angMotor.Equals((IHavokObject)other.m_angMotor))) &&
                   ((m_angFriction is null && other.m_angFriction is null) || (m_angFriction is not null && other.m_angFriction is not null && m_angFriction.Equals((IHavokObject)other.m_angFriction))) &&
                   ((m_angLimit is null && other.m_angLimit is null) || (m_angLimit is not null && other.m_angLimit is not null && m_angLimit.Equals((IHavokObject)other.m_angLimit))) &&
                   ((m_2dAng is null && other.m_2dAng is null) || (m_2dAng is not null && other.m_2dAng is not null && m_2dAng.Equals((IHavokObject)other.m_2dAng))) &&
                   ((m_ballSocket is null && other.m_ballSocket is null) || (m_ballSocket is not null && other.m_ballSocket is not null && m_ballSocket.Equals((IHavokObject)other.m_ballSocket))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transforms);
            hashcode.Add(m_setupStabilization);
            hashcode.Add(m_angMotor);
            hashcode.Add(m_angFriction);
            hashcode.Add(m_angLimit);
            hashcode.Add(m_2dAng);
            hashcode.Add(m_ballSocket);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

