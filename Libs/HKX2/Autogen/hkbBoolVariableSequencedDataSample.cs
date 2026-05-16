using System.Xml.Linq;
namespace HKX2
{
    // hkbBoolVariableSequencedDataSample Signatire: 0x514763dc size: 8 flags: FLAGS_NONE

    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_value m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkbBoolVariableSequencedDataSample : IHavokObject, IEquatable<hkbBoolVariableSequencedDataSample?>
    {
        public float m_time { set; get; }
        public bool m_value { set; get; }

        public virtual uint Signature { set; get; } = 0x514763dc;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            m_value = br.ReadBoolean();
            br.Position += 3;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteBoolean(m_value);
            bw.Position += 3;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_time = xd.ReadSingle(xe, nameof(m_time));
            m_value = xd.ReadBoolean(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_time), m_time);
            xs.WriteBoolean(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBoolVariableSequencedDataSample);
        }

        public bool Equals(hkbBoolVariableSequencedDataSample? other)
        {
            return other is not null &&
                   m_time.Equals(other.m_time) &&
                   m_value.Equals(other.m_value) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_time);
            hashcode.Add(m_value);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

