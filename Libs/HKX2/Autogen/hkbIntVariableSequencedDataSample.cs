using System.Xml.Linq;
namespace HKX2
{
    // hkbIntVariableSequencedDataSample Signatire: 0xbe7ac63c size: 8 flags: FLAGS_NONE

    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_value m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkbIntVariableSequencedDataSample : IHavokObject, IEquatable<hkbIntVariableSequencedDataSample?>
    {
        public float m_time { set; get; }
        public int m_value { set; get; }

        public virtual uint Signature { set; get; } = 0xbe7ac63c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            m_value = br.ReadInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteInt32(m_value);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_time = xd.ReadSingle(xe, nameof(m_time));
            m_value = xd.ReadInt32(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_time), m_time);
            xs.WriteNumber(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbIntVariableSequencedDataSample);
        }

        public bool Equals(hkbIntVariableSequencedDataSample? other)
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

