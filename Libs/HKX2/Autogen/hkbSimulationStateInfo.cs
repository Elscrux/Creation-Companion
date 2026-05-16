using System.Xml.Linq;
namespace HKX2
{
    // hkbSimulationStateInfo Signatire: 0xa40822b4 size: 24 flags: FLAGS_NONE

    // m_simulationState m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: SimulationState
    public partial class hkbSimulationStateInfo : hkReferencedObject, IEquatable<hkbSimulationStateInfo?>
    {
        public byte m_simulationState { set; get; }

        public override uint Signature { set; get; } = 0xa40822b4;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_simulationState = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_simulationState);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_simulationState = xd.ReadFlag<SimulationState, byte>(xe, nameof(m_simulationState));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteEnum<SimulationState, byte>(xe, nameof(m_simulationState), m_simulationState);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSimulationStateInfo);
        }

        public bool Equals(hkbSimulationStateInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_simulationState.Equals(other.m_simulationState) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_simulationState);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

