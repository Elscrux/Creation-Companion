using System.Xml.Linq;
namespace HKX2
{
    // hkaAnnotationTrack Signatire: 0xd4114fdd size: 24 flags: FLAGS_NONE

    // m_trackName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_annotations m_class: hkaAnnotationTrackAnnotation Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkaAnnotationTrack : IHavokObject, IEquatable<hkaAnnotationTrack?>
    {
        public string m_trackName { set; get; } = "";
        public IList<hkaAnnotationTrackAnnotation> m_annotations { set; get; } = Array.Empty<hkaAnnotationTrackAnnotation>();

        public virtual uint Signature { set; get; } = 0xd4114fdd;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_trackName = des.ReadStringPointer(br);
            m_annotations = des.ReadClassArray<hkaAnnotationTrackAnnotation>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_trackName);
            s.WriteClassArray(bw, m_annotations);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_trackName = xd.ReadString(xe, nameof(m_trackName));
            m_annotations = xd.ReadClassArray<hkaAnnotationTrackAnnotation>(xe, nameof(m_annotations));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_trackName), m_trackName);
            xs.WriteClassArray(xe, nameof(m_annotations), m_annotations);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaAnnotationTrack);
        }

        public bool Equals(hkaAnnotationTrack? other)
        {
            return other is not null &&
                   (m_trackName is null && other.m_trackName is null || m_trackName == other.m_trackName || m_trackName is null && other.m_trackName == "" || m_trackName == "" && other.m_trackName is null) &&
                   m_annotations.SequenceEqual(other.m_annotations) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_trackName);
            hashcode.Add(m_annotations.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

