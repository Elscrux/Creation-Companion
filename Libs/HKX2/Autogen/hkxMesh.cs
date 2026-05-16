using System.Xml.Linq;
namespace HKX2
{
    // hkxMesh Signatire: 0xf2edcc5f size: 48 flags: FLAGS_NONE

    // m_sections m_class: hkxMeshSection Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_userChannelInfos m_class: hkxMeshUserChannelInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkxMesh : hkReferencedObject, IEquatable<hkxMesh?>
    {
        public IList<hkxMeshSection> m_sections { set; get; } = Array.Empty<hkxMeshSection>();
        public IList<hkxMeshUserChannelInfo> m_userChannelInfos { set; get; } = Array.Empty<hkxMeshUserChannelInfo>();

        public override uint Signature { set; get; } = 0xf2edcc5f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sections = des.ReadClassPointerArray<hkxMeshSection>(br);
            m_userChannelInfos = des.ReadClassPointerArray<hkxMeshUserChannelInfo>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_sections);
            s.WriteClassPointerArray(bw, m_userChannelInfos);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_sections = xd.ReadClassPointerArray<hkxMeshSection>(xe, nameof(m_sections));
            m_userChannelInfos = xd.ReadClassPointerArray<hkxMeshUserChannelInfo>(xe, nameof(m_userChannelInfos));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_sections), m_sections);
            xs.WriteClassPointerArray(xe, nameof(m_userChannelInfos), m_userChannelInfos);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMesh);
        }

        public bool Equals(hkxMesh? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_sections.SequenceEqual(other.m_sections) &&
                   m_userChannelInfos.SequenceEqual(other.m_userChannelInfos) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_sections.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_userChannelInfos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

