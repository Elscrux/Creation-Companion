using System.Xml.Linq;
namespace HKX2
{
    // hkpConeLimitConstraintAtom Signatire: 0xf19443c8 size: 20 flags: FLAGS_NONE

    // m_isEnabled m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_twistAxisInA m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 3 flags: FLAGS_NONE enum: 
    // m_refAxisInB m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_angleMeasurementMode m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 5 flags: FLAGS_NONE enum: MeasurementMode
    // m_memOffsetToAngleOffset m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_minAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_maxAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_angularLimitsTauFactor m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpConeLimitConstraintAtom : hkpConstraintAtom, IEquatable<hkpConeLimitConstraintAtom?>
    {
        public byte m_isEnabled { set; get; }
        public byte m_twistAxisInA { set; get; }
        public byte m_refAxisInB { set; get; }
        public byte m_angleMeasurementMode { set; get; }
        public byte m_memOffsetToAngleOffset { set; get; }
        public float m_minAngle { set; get; }
        public float m_maxAngle { set; get; }
        public float m_angularLimitsTauFactor { set; get; }

        public override uint Signature { set; get; } = 0xf19443c8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_twistAxisInA = br.ReadByte();
            m_refAxisInB = br.ReadByte();
            m_angleMeasurementMode = br.ReadByte();
            m_memOffsetToAngleOffset = br.ReadByte();
            br.Position += 1;
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
            m_angularLimitsTauFactor = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_twistAxisInA);
            bw.WriteByte(m_refAxisInB);
            bw.WriteByte(m_angleMeasurementMode);
            bw.WriteByte(m_memOffsetToAngleOffset);
            bw.Position += 1;
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
            bw.WriteSingle(m_angularLimitsTauFactor);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isEnabled = xd.ReadByte(xe, nameof(m_isEnabled));
            m_twistAxisInA = xd.ReadByte(xe, nameof(m_twistAxisInA));
            m_refAxisInB = xd.ReadByte(xe, nameof(m_refAxisInB));
            m_angleMeasurementMode = xd.ReadFlag<MeasurementMode, byte>(xe, nameof(m_angleMeasurementMode));
            m_memOffsetToAngleOffset = xd.ReadByte(xe, nameof(m_memOffsetToAngleOffset));
            m_minAngle = xd.ReadSingle(xe, nameof(m_minAngle));
            m_maxAngle = xd.ReadSingle(xe, nameof(m_maxAngle));
            m_angularLimitsTauFactor = xd.ReadSingle(xe, nameof(m_angularLimitsTauFactor));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_isEnabled), m_isEnabled);
            xs.WriteNumber(xe, nameof(m_twistAxisInA), m_twistAxisInA);
            xs.WriteNumber(xe, nameof(m_refAxisInB), m_refAxisInB);
            xs.WriteEnum<MeasurementMode, byte>(xe, nameof(m_angleMeasurementMode), m_angleMeasurementMode);
            xs.WriteNumber(xe, nameof(m_memOffsetToAngleOffset), m_memOffsetToAngleOffset);
            xs.WriteFloat(xe, nameof(m_minAngle), m_minAngle);
            xs.WriteFloat(xe, nameof(m_maxAngle), m_maxAngle);
            xs.WriteFloat(xe, nameof(m_angularLimitsTauFactor), m_angularLimitsTauFactor);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConeLimitConstraintAtom);
        }

        public bool Equals(hkpConeLimitConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isEnabled.Equals(other.m_isEnabled) &&
                   m_twistAxisInA.Equals(other.m_twistAxisInA) &&
                   m_refAxisInB.Equals(other.m_refAxisInB) &&
                   m_angleMeasurementMode.Equals(other.m_angleMeasurementMode) &&
                   m_memOffsetToAngleOffset.Equals(other.m_memOffsetToAngleOffset) &&
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
            hashcode.Add(m_twistAxisInA);
            hashcode.Add(m_refAxisInB);
            hashcode.Add(m_angleMeasurementMode);
            hashcode.Add(m_memOffsetToAngleOffset);
            hashcode.Add(m_minAngle);
            hashcode.Add(m_maxAngle);
            hashcode.Add(m_angularLimitsTauFactor);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

