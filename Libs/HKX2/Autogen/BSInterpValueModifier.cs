using System.Xml.Linq;
namespace HKX2
{
    // BSInterpValueModifier Signatire: 0x29adc802 size: 104 flags: FLAGS_NONE

    // m_source m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_target m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_result m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_gain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_timeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSInterpValueModifier : hkbModifier, IEquatable<BSInterpValueModifier?>
    {
        public float m_source { set; get; }
        public float m_target { set; get; }
        public float m_result { set; get; }
        public float m_gain { set; get; }
        private float m_timeStep { set; get; }

        public override uint Signature { set; get; } = 0x29adc802;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_source = br.ReadSingle();
            m_target = br.ReadSingle();
            m_result = br.ReadSingle();
            m_gain = br.ReadSingle();
            m_timeStep = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_source);
            bw.WriteSingle(m_target);
            bw.WriteSingle(m_result);
            bw.WriteSingle(m_gain);
            bw.WriteSingle(m_timeStep);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_source = xd.ReadSingle(xe, nameof(m_source));
            m_target = xd.ReadSingle(xe, nameof(m_target));
            m_result = xd.ReadSingle(xe, nameof(m_result));
            m_gain = xd.ReadSingle(xe, nameof(m_gain));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_source), m_source);
            xs.WriteFloat(xe, nameof(m_target), m_target);
            xs.WriteFloat(xe, nameof(m_result), m_result);
            xs.WriteFloat(xe, nameof(m_gain), m_gain);
            xs.WriteSerializeIgnored(xe, nameof(m_timeStep));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSInterpValueModifier);
        }

        public bool Equals(BSInterpValueModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_source.Equals(other.m_source) &&
                   m_target.Equals(other.m_target) &&
                   m_result.Equals(other.m_result) &&
                   m_gain.Equals(other.m_gain) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_source);
            hashcode.Add(m_target);
            hashcode.Add(m_result);
            hashcode.Add(m_gain);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

