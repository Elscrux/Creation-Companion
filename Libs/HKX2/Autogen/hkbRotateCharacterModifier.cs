using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbRotateCharacterModifier Signatire: 0x877ebc0b size: 128 flags: FLAGS_NONE

    // m_degreesPerSecond m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_speedMultiplier m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_axisOfRotation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_angle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbRotateCharacterModifier : hkbModifier, IEquatable<hkbRotateCharacterModifier?>
    {
        public float m_degreesPerSecond { set; get; }
        public float m_speedMultiplier { set; get; }
        public Vector4 m_axisOfRotation { set; get; }
        private float m_angle { set; get; }

        public override uint Signature { set; get; } = 0x877ebc0b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_degreesPerSecond = br.ReadSingle();
            m_speedMultiplier = br.ReadSingle();
            br.Position += 8;
            m_axisOfRotation = br.ReadVector4();
            m_angle = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_degreesPerSecond);
            bw.WriteSingle(m_speedMultiplier);
            bw.Position += 8;
            bw.WriteVector4(m_axisOfRotation);
            bw.WriteSingle(m_angle);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_degreesPerSecond = xd.ReadSingle(xe, nameof(m_degreesPerSecond));
            m_speedMultiplier = xd.ReadSingle(xe, nameof(m_speedMultiplier));
            m_axisOfRotation = xd.ReadVector4(xe, nameof(m_axisOfRotation));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_degreesPerSecond), m_degreesPerSecond);
            xs.WriteFloat(xe, nameof(m_speedMultiplier), m_speedMultiplier);
            xs.WriteVector4(xe, nameof(m_axisOfRotation), m_axisOfRotation);
            xs.WriteSerializeIgnored(xe, nameof(m_angle));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbRotateCharacterModifier);
        }

        public bool Equals(hkbRotateCharacterModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_degreesPerSecond.Equals(other.m_degreesPerSecond) &&
                   m_speedMultiplier.Equals(other.m_speedMultiplier) &&
                   m_axisOfRotation.Equals(other.m_axisOfRotation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_degreesPerSecond);
            hashcode.Add(m_speedMultiplier);
            hashcode.Add(m_axisOfRotation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

