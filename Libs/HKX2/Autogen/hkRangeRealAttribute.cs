using System.Xml.Linq;
namespace HKX2
{
    // hkRangeRealAttribute Signatire: 0x949db24f size: 16 flags: FLAGS_NONE

    // m_absmin m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_absmax m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_softmin m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_softmax m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkRangeRealAttribute : IHavokObject, IEquatable<hkRangeRealAttribute?>
    {
        public float m_absmin { set; get; }
        public float m_absmax { set; get; }
        public float m_softmin { set; get; }
        public float m_softmax { set; get; }

        public virtual uint Signature { set; get; } = 0x949db24f;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_absmin = br.ReadSingle();
            m_absmax = br.ReadSingle();
            m_softmin = br.ReadSingle();
            m_softmax = br.ReadSingle();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_absmin);
            bw.WriteSingle(m_absmax);
            bw.WriteSingle(m_softmin);
            bw.WriteSingle(m_softmax);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_absmin = xd.ReadSingle(xe, nameof(m_absmin));
            m_absmax = xd.ReadSingle(xe, nameof(m_absmax));
            m_softmin = xd.ReadSingle(xe, nameof(m_softmin));
            m_softmax = xd.ReadSingle(xe, nameof(m_softmax));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_absmin), m_absmin);
            xs.WriteFloat(xe, nameof(m_absmax), m_absmax);
            xs.WriteFloat(xe, nameof(m_softmin), m_softmin);
            xs.WriteFloat(xe, nameof(m_softmax), m_softmax);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkRangeRealAttribute);
        }

        public bool Equals(hkRangeRealAttribute? other)
        {
            return other is not null &&
                   m_absmin.Equals(other.m_absmin) &&
                   m_absmax.Equals(other.m_absmax) &&
                   m_softmin.Equals(other.m_softmin) &&
                   m_softmax.Equals(other.m_softmax) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_absmin);
            hashcode.Add(m_absmax);
            hashcode.Add(m_softmin);
            hashcode.Add(m_softmax);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

