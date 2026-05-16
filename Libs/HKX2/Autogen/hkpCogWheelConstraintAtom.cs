using System.Xml.Linq;
namespace HKX2
{
    // hkpCogWheelConstraintAtom Signatire: 0xf2b1f399 size: 16 flags: FLAGS_NONE

    // m_cogWheelRadiusA m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_cogWheelRadiusB m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_isScrew m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_memOffsetToInitialAngleOffset m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 13 flags: FLAGS_NONE enum: 
    // m_memOffsetToPrevAngle m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 14 flags: FLAGS_NONE enum: 
    // m_memOffsetToRevolutionCounter m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 15 flags: FLAGS_NONE enum: 
    public partial class hkpCogWheelConstraintAtom : hkpConstraintAtom, IEquatable<hkpCogWheelConstraintAtom?>
    {
        public float m_cogWheelRadiusA { set; get; }
        public float m_cogWheelRadiusB { set; get; }
        public bool m_isScrew { set; get; }
        public sbyte m_memOffsetToInitialAngleOffset { set; get; }
        public sbyte m_memOffsetToPrevAngle { set; get; }
        public sbyte m_memOffsetToRevolutionCounter { set; get; }

        public override uint Signature { set; get; } = 0xf2b1f399;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 2;
            m_cogWheelRadiusA = br.ReadSingle();
            m_cogWheelRadiusB = br.ReadSingle();
            m_isScrew = br.ReadBoolean();
            m_memOffsetToInitialAngleOffset = br.ReadSByte();
            m_memOffsetToPrevAngle = br.ReadSByte();
            m_memOffsetToRevolutionCounter = br.ReadSByte();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 2;
            bw.WriteSingle(m_cogWheelRadiusA);
            bw.WriteSingle(m_cogWheelRadiusB);
            bw.WriteBoolean(m_isScrew);
            bw.WriteSByte(m_memOffsetToInitialAngleOffset);
            bw.WriteSByte(m_memOffsetToPrevAngle);
            bw.WriteSByte(m_memOffsetToRevolutionCounter);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_cogWheelRadiusA = xd.ReadSingle(xe, nameof(m_cogWheelRadiusA));
            m_cogWheelRadiusB = xd.ReadSingle(xe, nameof(m_cogWheelRadiusB));
            m_isScrew = xd.ReadBoolean(xe, nameof(m_isScrew));
            m_memOffsetToInitialAngleOffset = xd.ReadSByte(xe, nameof(m_memOffsetToInitialAngleOffset));
            m_memOffsetToPrevAngle = xd.ReadSByte(xe, nameof(m_memOffsetToPrevAngle));
            m_memOffsetToRevolutionCounter = xd.ReadSByte(xe, nameof(m_memOffsetToRevolutionCounter));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_cogWheelRadiusA), m_cogWheelRadiusA);
            xs.WriteFloat(xe, nameof(m_cogWheelRadiusB), m_cogWheelRadiusB);
            xs.WriteBoolean(xe, nameof(m_isScrew), m_isScrew);
            xs.WriteNumber(xe, nameof(m_memOffsetToInitialAngleOffset), m_memOffsetToInitialAngleOffset);
            xs.WriteNumber(xe, nameof(m_memOffsetToPrevAngle), m_memOffsetToPrevAngle);
            xs.WriteNumber(xe, nameof(m_memOffsetToRevolutionCounter), m_memOffsetToRevolutionCounter);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCogWheelConstraintAtom);
        }

        public bool Equals(hkpCogWheelConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_cogWheelRadiusA.Equals(other.m_cogWheelRadiusA) &&
                   m_cogWheelRadiusB.Equals(other.m_cogWheelRadiusB) &&
                   m_isScrew.Equals(other.m_isScrew) &&
                   m_memOffsetToInitialAngleOffset.Equals(other.m_memOffsetToInitialAngleOffset) &&
                   m_memOffsetToPrevAngle.Equals(other.m_memOffsetToPrevAngle) &&
                   m_memOffsetToRevolutionCounter.Equals(other.m_memOffsetToRevolutionCounter) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_cogWheelRadiusA);
            hashcode.Add(m_cogWheelRadiusB);
            hashcode.Add(m_isScrew);
            hashcode.Add(m_memOffsetToInitialAngleOffset);
            hashcode.Add(m_memOffsetToPrevAngle);
            hashcode.Add(m_memOffsetToRevolutionCounter);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

