using System.Xml.Linq;
namespace HKX2
{
    // hkbPoweredRagdollControlData Signatire: 0xf5ba21b size: 32 flags: FLAGS_NONE

    // m_maxForce m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_proportionalRecoveryVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_constantRecoveryVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbPoweredRagdollControlData : IHavokObject, IEquatable<hkbPoweredRagdollControlData?>
    {
        public float m_maxForce { set; get; }
        public float m_tau { set; get; }
        public float m_damping { set; get; }
        public float m_proportionalRecoveryVelocity { set; get; }
        public float m_constantRecoveryVelocity { set; get; }

        public virtual uint Signature { set; get; } = 0xf5ba21b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_maxForce = br.ReadSingle();
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_proportionalRecoveryVelocity = br.ReadSingle();
            m_constantRecoveryVelocity = br.ReadSingle();
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_maxForce);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_proportionalRecoveryVelocity);
            bw.WriteSingle(m_constantRecoveryVelocity);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_maxForce = xd.ReadSingle(xe, nameof(m_maxForce));
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
            m_proportionalRecoveryVelocity = xd.ReadSingle(xe, nameof(m_proportionalRecoveryVelocity));
            m_constantRecoveryVelocity = xd.ReadSingle(xe, nameof(m_constantRecoveryVelocity));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_maxForce), m_maxForce);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
            xs.WriteFloat(xe, nameof(m_proportionalRecoveryVelocity), m_proportionalRecoveryVelocity);
            xs.WriteFloat(xe, nameof(m_constantRecoveryVelocity), m_constantRecoveryVelocity);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbPoweredRagdollControlData);
        }

        public bool Equals(hkbPoweredRagdollControlData? other)
        {
            return other is not null &&
                   m_maxForce.Equals(other.m_maxForce) &&
                   m_tau.Equals(other.m_tau) &&
                   m_damping.Equals(other.m_damping) &&
                   m_proportionalRecoveryVelocity.Equals(other.m_proportionalRecoveryVelocity) &&
                   m_constantRecoveryVelocity.Equals(other.m_constantRecoveryVelocity) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_maxForce);
            hashcode.Add(m_tau);
            hashcode.Add(m_damping);
            hashcode.Add(m_proportionalRecoveryVelocity);
            hashcode.Add(m_constantRecoveryVelocity);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

