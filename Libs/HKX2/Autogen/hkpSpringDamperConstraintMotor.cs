using System.Xml.Linq;
namespace HKX2
{
    // hkpSpringDamperConstraintMotor Signatire: 0x7ead26f6 size: 40 flags: FLAGS_NONE

    // m_springConstant m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_springDamping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    public partial class hkpSpringDamperConstraintMotor : hkpLimitedForceConstraintMotor, IEquatable<hkpSpringDamperConstraintMotor?>
    {
        public float m_springConstant { set; get; }
        public float m_springDamping { set; get; }

        public override uint Signature { set; get; } = 0x7ead26f6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_springConstant = br.ReadSingle();
            m_springDamping = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_springConstant);
            bw.WriteSingle(m_springDamping);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_springConstant = xd.ReadSingle(xe, nameof(m_springConstant));
            m_springDamping = xd.ReadSingle(xe, nameof(m_springDamping));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_springConstant), m_springConstant);
            xs.WriteFloat(xe, nameof(m_springDamping), m_springDamping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSpringDamperConstraintMotor);
        }

        public bool Equals(hkpSpringDamperConstraintMotor? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_springConstant.Equals(other.m_springConstant) &&
                   m_springDamping.Equals(other.m_springDamping) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_springConstant);
            hashcode.Add(m_springDamping);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

