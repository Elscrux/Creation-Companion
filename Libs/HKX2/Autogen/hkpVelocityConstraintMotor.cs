using System.Xml.Linq;
namespace HKX2
{
    // hkpVelocityConstraintMotor Signatire: 0xfca2fcc3 size: 48 flags: FLAGS_NONE

    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_velocityTarget m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_useVelocityTargetFromConstraintTargets m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkpVelocityConstraintMotor : hkpLimitedForceConstraintMotor, IEquatable<hkpVelocityConstraintMotor?>
    {
        public float m_tau { set; get; }
        public float m_velocityTarget { set; get; }
        public bool m_useVelocityTargetFromConstraintTargets { set; get; }

        public override uint Signature { set; get; } = 0xfca2fcc3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_velocityTarget = br.ReadSingle();
            m_useVelocityTargetFromConstraintTargets = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_velocityTarget);
            bw.WriteBoolean(m_useVelocityTargetFromConstraintTargets);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_velocityTarget = xd.ReadSingle(xe, nameof(m_velocityTarget));
            m_useVelocityTargetFromConstraintTargets = xd.ReadBoolean(xe, nameof(m_useVelocityTargetFromConstraintTargets));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_velocityTarget), m_velocityTarget);
            xs.WriteBoolean(xe, nameof(m_useVelocityTargetFromConstraintTargets), m_useVelocityTargetFromConstraintTargets);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpVelocityConstraintMotor);
        }

        public bool Equals(hkpVelocityConstraintMotor? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_tau.Equals(other.m_tau) &&
                   m_velocityTarget.Equals(other.m_velocityTarget) &&
                   m_useVelocityTargetFromConstraintTargets.Equals(other.m_useVelocityTargetFromConstraintTargets) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_tau);
            hashcode.Add(m_velocityTarget);
            hashcode.Add(m_useVelocityTargetFromConstraintTargets);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

