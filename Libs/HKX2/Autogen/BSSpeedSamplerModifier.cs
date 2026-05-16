using System.Xml.Linq;
namespace HKX2
{
    // BSSpeedSamplerModifier Signatire: 0xd297fda9 size: 96 flags: FLAGS_NONE

    // m_state m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_direction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_goalSpeed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_speedOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    public partial class BSSpeedSamplerModifier : hkbModifier, IEquatable<BSSpeedSamplerModifier?>
    {
        public int m_state { set; get; }
        public float m_direction { set; get; }
        public float m_goalSpeed { set; get; }
        public float m_speedOut { set; get; }

        public override uint Signature { set; get; } = 0xd297fda9;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_state = br.ReadInt32();
            m_direction = br.ReadSingle();
            m_goalSpeed = br.ReadSingle();
            m_speedOut = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_state);
            bw.WriteSingle(m_direction);
            bw.WriteSingle(m_goalSpeed);
            bw.WriteSingle(m_speedOut);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_state = xd.ReadInt32(xe, nameof(m_state));
            m_direction = xd.ReadSingle(xe, nameof(m_direction));
            m_goalSpeed = xd.ReadSingle(xe, nameof(m_goalSpeed));
            m_speedOut = xd.ReadSingle(xe, nameof(m_speedOut));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_state), m_state);
            xs.WriteFloat(xe, nameof(m_direction), m_direction);
            xs.WriteFloat(xe, nameof(m_goalSpeed), m_goalSpeed);
            xs.WriteFloat(xe, nameof(m_speedOut), m_speedOut);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSSpeedSamplerModifier);
        }

        public bool Equals(BSSpeedSamplerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_state.Equals(other.m_state) &&
                   m_direction.Equals(other.m_direction) &&
                   m_goalSpeed.Equals(other.m_goalSpeed) &&
                   m_speedOut.Equals(other.m_speedOut) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_state);
            hashcode.Add(m_direction);
            hashcode.Add(m_goalSpeed);
            hashcode.Add(m_speedOut);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

