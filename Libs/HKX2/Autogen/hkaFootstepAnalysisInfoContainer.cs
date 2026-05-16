using System.Xml.Linq;
namespace HKX2
{
    // hkaFootstepAnalysisInfoContainer Signatire: 0x1d81207c size: 32 flags: FLAGS_NONE

    // m_previewInfo m_class: hkaFootstepAnalysisInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkaFootstepAnalysisInfoContainer : hkReferencedObject, IEquatable<hkaFootstepAnalysisInfoContainer?>
    {
        public IList<hkaFootstepAnalysisInfo> m_previewInfo { set; get; } = Array.Empty<hkaFootstepAnalysisInfo>();

        public override uint Signature { set; get; } = 0x1d81207c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_previewInfo = des.ReadClassPointerArray<hkaFootstepAnalysisInfo>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_previewInfo);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_previewInfo = xd.ReadClassPointerArray<hkaFootstepAnalysisInfo>(xe, nameof(m_previewInfo));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray<hkaFootstepAnalysisInfo>(xe, nameof(m_previewInfo), m_previewInfo);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaFootstepAnalysisInfoContainer);
        }

        public bool Equals(hkaFootstepAnalysisInfoContainer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_previewInfo.SequenceEqual(other.m_previewInfo) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_previewInfo.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

