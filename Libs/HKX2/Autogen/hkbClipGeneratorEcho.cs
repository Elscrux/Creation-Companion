using System.Xml.Linq;
namespace HKX2
{
    // hkbClipGeneratorEcho Signatire: 0x750edf40 size: 16 flags: FLAGS_NONE

    // m_offsetLocalTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_weight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_dwdt m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkbClipGeneratorEcho : IHavokObject, IEquatable<hkbClipGeneratorEcho?>
    {
        public float m_offsetLocalTime { set; get; }
        public float m_weight { set; get; }
        public float m_dwdt { set; get; }

        public virtual uint Signature { set; get; } = 0x750edf40;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offsetLocalTime = br.ReadSingle();
            m_weight = br.ReadSingle();
            m_dwdt = br.ReadSingle();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_offsetLocalTime);
            bw.WriteSingle(m_weight);
            bw.WriteSingle(m_dwdt);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_offsetLocalTime = xd.ReadSingle(xe, nameof(m_offsetLocalTime));
            m_weight = xd.ReadSingle(xe, nameof(m_weight));
            m_dwdt = xd.ReadSingle(xe, nameof(m_dwdt));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_offsetLocalTime), m_offsetLocalTime);
            xs.WriteFloat(xe, nameof(m_weight), m_weight);
            xs.WriteFloat(xe, nameof(m_dwdt), m_dwdt);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbClipGeneratorEcho);
        }

        public bool Equals(hkbClipGeneratorEcho? other)
        {
            return other is not null &&
                   m_offsetLocalTime.Equals(other.m_offsetLocalTime) &&
                   m_weight.Equals(other.m_weight) &&
                   m_dwdt.Equals(other.m_dwdt) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_offsetLocalTime);
            hashcode.Add(m_weight);
            hashcode.Add(m_dwdt);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

