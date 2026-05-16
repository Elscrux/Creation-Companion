using System.Xml.Linq;
namespace HKX2
{
    // hkpTwistLimitConstraintAtom Signatire: 0x7c9b1052 size: 20 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_twistAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_refAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_minAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_maxAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_angularLimitsTauFactor m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpTwistLimitConstraintAtom : hkpConstraintAtom, IEquatable<hkpTwistLimitConstraintAtom?>
    {
        public byte m_isEnabled { set; get; }
        public byte m_twistAxis { set; get; }
        public byte m_refAxis { set; get; }
        public float m_minAngle { set; get; }
        public float m_maxAngle { set; get; }
        public float m_angularLimitsTauFactor { set; get; }

        public override uint Signature { set; get; } = 0x7c9b1052;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_twistAxis = br.ReadByte();
            m_refAxis = br.ReadByte();
            br.Position += 3;
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
            m_angularLimitsTauFactor = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_twistAxis);
            bw.WriteByte(m_refAxis);
            bw.Position += 3;
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
            bw.WriteSingle(m_angularLimitsTauFactor);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadByte(xe, nameof(m_isEnabled));
            m_twistAxis = xd.ReadByte(xe, nameof(m_twistAxis));
            m_refAxis = xd.ReadByte(xe, nameof(m_refAxis));
            m_minAngle = xd.ReadSingle(xe, nameof(m_minAngle));
            m_maxAngle = xd.ReadSingle(xe, nameof(m_maxAngle));
            m_angularLimitsTauFactor = xd.ReadSingle(xe, nameof(m_angularLimitsTauFactor));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_twistAxis), m_twistAxis);
            xs.WriteNumber(xe, nameof(m_refAxis), m_refAxis);
            xs.WriteFloat(xe, nameof(m_minAngle), m_minAngle);
            xs.WriteFloat(xe, nameof(m_maxAngle), m_maxAngle);
            xs.WriteFloat(xe, nameof(m_angularLimitsTauFactor), m_angularLimitsTauFactor);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpTwistLimitConstraintAtom);
        }

        public bool Equals(hkpTwistLimitConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_twistAxis.Equals(other.m_twistAxis) &&
                   m_refAxis.Equals(other.m_refAxis) &&
                   m_minAngle.Equals(other.m_minAngle) &&
                   m_maxAngle.Equals(other.m_maxAngle) &&
                   m_angularLimitsTauFactor.Equals(other.m_angularLimitsTauFactor) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isEnabled);
            hashcode.Add(m_twistAxis);
            hashcode.Add(m_refAxis);
            hashcode.Add(m_minAngle);
            hashcode.Add(m_maxAngle);
            hashcode.Add(m_angularLimitsTauFactor);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

