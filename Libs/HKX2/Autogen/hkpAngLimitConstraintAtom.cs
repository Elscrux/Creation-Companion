using System.Xml.Linq;
namespace HKX2
{
    // hkpAngLimitConstraintAtom Signatire: 0x9be0d9d size: 16 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_limitAxis m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_minAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_maxAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_angularLimitsTauFactor m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkpAngLimitConstraintAtom : hkpConstraintAtom, IEquatable<hkpAngLimitConstraintAtom?>
    {
        public byte m_isEnabled { set; get; }
        public byte m_limitAxis { set; get; }
        public float m_minAngle { set; get; }
        public float m_maxAngle { set; get; }
        public float m_angularLimitsTauFactor { set; get; }

        public override uint Signature { set; get; } = 0x9be0d9d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_limitAxis = br.ReadByte();
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
            m_angularLimitsTauFactor = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_limitAxis);
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
            bw.WriteSingle(m_angularLimitsTauFactor);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadByte(xe, nameof(m_isEnabled));
            m_limitAxis = xd.ReadByte(xe, nameof(m_limitAxis));
            m_minAngle = xd.ReadSingle(xe, nameof(m_minAngle));
            m_maxAngle = xd.ReadSingle(xe, nameof(m_maxAngle));
            m_angularLimitsTauFactor = xd.ReadSingle(xe, nameof(m_angularLimitsTauFactor));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_limitAxis), m_limitAxis);
            xs.WriteFloat(xe, nameof(m_minAngle), m_minAngle);
            xs.WriteFloat(xe, nameof(m_maxAngle), m_maxAngle);
            xs.WriteFloat(xe, nameof(m_angularLimitsTauFactor), m_angularLimitsTauFactor);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAngLimitConstraintAtom);
        }

        public bool Equals(hkpAngLimitConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_limitAxis.Equals(other.m_limitAxis) &&
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
            hashcode.Add(m_limitAxis);
            hashcode.Add(m_minAngle);
            hashcode.Add(m_maxAngle);
            hashcode.Add(m_angularLimitsTauFactor);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

