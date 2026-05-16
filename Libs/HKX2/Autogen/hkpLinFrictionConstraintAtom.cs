using System.Xml.Linq;
namespace HKX2
{
    // hkpLinFrictionConstraintAtom Signatire: 0x3e94ef7c size: 8 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_frictionAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_maxFrictionForce m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkpLinFrictionConstraintAtom : hkpConstraintAtom, IEquatable<hkpLinFrictionConstraintAtom?>
    {
        public byte m_isEnabled { set; get; }
        public byte m_frictionAxis { set; get; }
        public float m_maxFrictionForce { set; get; }

        public override uint Signature { set; get; } = 0x3e94ef7c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_frictionAxis = br.ReadByte();
            m_maxFrictionForce = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_frictionAxis);
            bw.WriteSingle(m_maxFrictionForce);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadByte(xe, nameof(m_isEnabled));
            m_frictionAxis = xd.ReadByte(xe, nameof(m_frictionAxis));
            m_maxFrictionForce = xd.ReadSingle(xe, nameof(m_maxFrictionForce));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_frictionAxis), m_frictionAxis);
            xs.WriteFloat(xe, nameof(m_maxFrictionForce), m_maxFrictionForce);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpLinFrictionConstraintAtom);
        }

        public bool Equals(hkpLinFrictionConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_frictionAxis.Equals(other.m_frictionAxis) &&
                   m_maxFrictionForce.Equals(other.m_maxFrictionForce) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isEnabled);
            hashcode.Add(m_frictionAxis);
            hashcode.Add(m_maxFrictionForce);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

