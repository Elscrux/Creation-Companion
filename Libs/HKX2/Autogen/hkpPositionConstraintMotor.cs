using System.Xml.Linq;
namespace HKX2
{
    // hkpPositionConstraintMotor Signatire: 0x748fb303 size: 48 flags: FLAGS_NONE

    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_proportionalRecoveryVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_constantRecoveryVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    public partial class hkpPositionConstraintMotor : hkpLimitedForceConstraintMotor, IEquatable<hkpPositionConstraintMotor?>
    {
        public float m_tau { set; get; }
        public float m_damping { set; get; }
        public float m_proportionalRecoveryVelocity { set; get; }
        public float m_constantRecoveryVelocity { set; get; }

        public override uint Signature { set; get; } = 0x748fb303;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_proportionalRecoveryVelocity = br.ReadSingle();
            m_constantRecoveryVelocity = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_proportionalRecoveryVelocity);
            bw.WriteSingle(m_constantRecoveryVelocity);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
            m_proportionalRecoveryVelocity = xd.ReadSingle(xe, nameof(m_proportionalRecoveryVelocity));
            m_constantRecoveryVelocity = xd.ReadSingle(xe, nameof(m_constantRecoveryVelocity));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
            xs.WriteFloat(xe, nameof(m_proportionalRecoveryVelocity), m_proportionalRecoveryVelocity);
            xs.WriteFloat(xe, nameof(m_constantRecoveryVelocity), m_constantRecoveryVelocity);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPositionConstraintMotor);
        }

        public bool Equals(hkpPositionConstraintMotor? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_tau.Equals(other.m_tau) &&
                   m_damping.Equals(other.m_damping) &&
                   m_proportionalRecoveryVelocity.Equals(other.m_proportionalRecoveryVelocity) &&
                   m_constantRecoveryVelocity.Equals(other.m_constantRecoveryVelocity) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_tau);
            hashcode.Add(m_damping);
            hashcode.Add(m_proportionalRecoveryVelocity);
            hashcode.Add(m_constantRecoveryVelocity);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

