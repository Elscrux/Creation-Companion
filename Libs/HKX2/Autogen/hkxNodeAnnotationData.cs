using System.Xml.Linq;
namespace HKX2
{
    // hkxNodeAnnotationData Signatire: 0x433dee92 size: 16 flags: FLAGS_NONE

    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_description m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkxNodeAnnotationData : IHavokObject, IEquatable<hkxNodeAnnotationData?>
    {
        public float m_time { set; get; }
        public string m_description { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x433dee92;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            br.Position += 4;
            m_description = des.ReadStringPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.Position += 4;
            s.WriteStringPointer(bw, m_description);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_time = xd.ReadSingle(xe, nameof(m_time));
            m_description = xd.ReadString(xe, nameof(m_description));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_time), m_time);
            xs.WriteString(xe, nameof(m_description), m_description);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxNodeAnnotationData);
        }

        public bool Equals(hkxNodeAnnotationData? other)
        {
            return other is not null &&
                   m_time.Equals(other.m_time) &&
                   (m_description is null && other.m_description is null || m_description == other.m_description || m_description is null && other.m_description == "" || m_description == "" && other.m_description is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_time);
            hashcode.Add(m_description);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

