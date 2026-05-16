using System.Xml.Linq;
namespace HKX2
{
    // hkaAnnotationTrackAnnotation Signatire: 0x623bf34f size: 16 flags: FLAGS_NONE

    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_text m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkaAnnotationTrackAnnotation : IHavokObject, IEquatable<hkaAnnotationTrackAnnotation?>
    {
        public float m_time { set; get; }
        public string m_text { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x623bf34f;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            br.Position += 4;
            m_text = des.ReadStringPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.Position += 4;
            s.WriteStringPointer(bw, m_text);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_time = xd.ReadSingle(xe, nameof(m_time));
            m_text = xd.ReadString(xe, nameof(m_text));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_time), m_time);
            xs.WriteString(xe, nameof(m_text), m_text);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaAnnotationTrackAnnotation);
        }

        public bool Equals(hkaAnnotationTrackAnnotation? other)
        {
            return other is not null &&
                   m_time.Equals(other.m_time) &&
                   (m_text is null && other.m_text is null || m_text == other.m_text || m_text is null && other.m_text == "" || m_text == "" && other.m_text is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_time);
            hashcode.Add(m_text);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

