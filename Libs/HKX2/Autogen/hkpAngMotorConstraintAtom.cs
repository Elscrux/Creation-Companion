using System.Xml.Linq;
namespace HKX2
{
    // hkpAngMotorConstraintAtom Signatire: 0x81f087ff size: 24 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_motorAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_initializedOffset m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_previousTargetAngleOffset m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_correspondingAngLimitSolverResultOffset m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_targetAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_motor m_class: hkpConstraintMotor Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpAngMotorConstraintAtom : hkpConstraintAtom, IEquatable<hkpAngMotorConstraintAtom?>
    {
        public bool m_isEnabled { set; get; }
        public byte m_motorAxis { set; get; }
        public short m_initializedOffset { set; get; }
        public short m_previousTargetAngleOffset { set; get; }
        public short m_correspondingAngLimitSolverResultOffset { set; get; }
        public float m_targetAngle { set; get; }
        public hkpConstraintMotor? m_motor { set; get; }

        public override uint Signature { set; get; } = 0x81f087ff;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadBoolean();
            m_motorAxis = br.ReadByte();
            m_initializedOffset = br.ReadInt16();
            m_previousTargetAngleOffset = br.ReadInt16();
            m_correspondingAngLimitSolverResultOffset = br.ReadInt16();
            br.Position += 2;
            m_targetAngle = br.ReadSingle();
            m_motor = des.ReadClassPointer<hkpConstraintMotor>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteByte(m_motorAxis);
            bw.WriteInt16(m_initializedOffset);
            bw.WriteInt16(m_previousTargetAngleOffset);
            bw.WriteInt16(m_correspondingAngLimitSolverResultOffset);
            bw.Position += 2;
            bw.WriteSingle(m_targetAngle);
            s.WriteClassPointer(bw, m_motor);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadBoolean(xe, nameof(m_isEnabled));
            m_motorAxis = xd.ReadByte(xe, nameof(m_motorAxis));
            m_initializedOffset = xd.ReadInt16(xe, nameof(m_initializedOffset));
            m_previousTargetAngleOffset = xd.ReadInt16(xe, nameof(m_previousTargetAngleOffset));
            m_correspondingAngLimitSolverResultOffset = xd.ReadInt16(xe, nameof(m_correspondingAngLimitSolverResultOffset));
            m_targetAngle = xd.ReadSingle(xe, nameof(m_targetAngle));
            m_motor = xd.ReadClassPointer<hkpConstraintMotor>(xe, nameof(m_motor));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_motorAxis), m_motorAxis);
            xs.WriteNumber(xe, nameof(m_initializedOffset), m_initializedOffset);
            xs.WriteNumber(xe, nameof(m_previousTargetAngleOffset), m_previousTargetAngleOffset);
            xs.WriteNumber(xe, nameof(m_correspondingAngLimitSolverResultOffset), m_correspondingAngLimitSolverResultOffset);
            xs.WriteFloat(xe, nameof(m_targetAngle), m_targetAngle);
            xs.WriteClassPointer(xe, nameof(m_motor), m_motor);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAngMotorConstraintAtom);
        }

        public bool Equals(hkpAngMotorConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_motorAxis.Equals(other.m_motorAxis) &&
                   m_initializedOffset.Equals(other.m_initializedOffset) &&
                   m_previousTargetAngleOffset.Equals(other.m_previousTargetAngleOffset) &&
                   m_correspondingAngLimitSolverResultOffset.Equals(other.m_correspondingAngLimitSolverResultOffset) &&
                   m_targetAngle.Equals(other.m_targetAngle) &&
                   ((m_motor is null && other.m_motor is null) || (m_motor is not null && other.m_motor is not null && m_motor.Equals((IHavokObject)other.m_motor))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isEnabled);
            hashcode.Add(m_motorAxis);
            hashcode.Add(m_initializedOffset);
            hashcode.Add(m_previousTargetAngleOffset);
            hashcode.Add(m_correspondingAngLimitSolverResultOffset);
            hashcode.Add(m_targetAngle);
            hashcode.Add(m_motor);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

