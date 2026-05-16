using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedTrack1nInfo Signatire: 0xf12d48d9 size: 32 flags: FLAGS_NONE

    // m_sectors m_class: hkpAgent1nSector Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_subTracks m_class: hkpSerializedSubTrack1nInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedTrack1nInfo : IHavokObject, IEquatable<hkpSerializedTrack1nInfo?>
    {
        public IList<hkpAgent1nSector> m_sectors { set; get; } = Array.Empty<hkpAgent1nSector>();
        public IList<hkpSerializedSubTrack1nInfo> m_subTracks { set; get; } = Array.Empty<hkpSerializedSubTrack1nInfo>();

        public virtual uint Signature { set; get; } = 0xf12d48d9;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_sectors = des.ReadClassPointerArray<hkpAgent1nSector>(br);
            m_subTracks = des.ReadClassPointerArray<hkpSerializedSubTrack1nInfo>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointerArray(bw, m_sectors);
            s.WriteClassPointerArray(bw, m_subTracks);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_sectors = xd.ReadClassPointerArray<hkpAgent1nSector>(xe, nameof(m_sectors));
            m_subTracks = xd.ReadClassPointerArray<hkpSerializedSubTrack1nInfo>(xe, nameof(m_subTracks));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointerArray(xe, nameof(m_sectors), m_sectors);
            xs.WriteClassPointerArray(xe, nameof(m_subTracks), m_subTracks);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedTrack1nInfo);
        }

        public bool Equals(hkpSerializedTrack1nInfo? other)
        {
            return other is not null &&
                   m_sectors.SequenceEqual(other.m_sectors) &&
                   m_subTracks.SequenceEqual(other.m_subTracks) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_sectors.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_subTracks.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

