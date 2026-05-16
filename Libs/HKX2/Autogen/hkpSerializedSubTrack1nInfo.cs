using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedSubTrack1nInfo Signatire: 0x10155a size: 40 flags: FLAGS_NONE

    // m_sectorIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_offsetInSector m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedSubTrack1nInfo : hkpSerializedTrack1nInfo, IEquatable<hkpSerializedSubTrack1nInfo?>
    {
        public int m_sectorIndex { set; get; }
        public int m_offsetInSector { set; get; }

        public override uint Signature { set; get; } = 0x10155a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sectorIndex = br.ReadInt32();
            m_offsetInSector = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_sectorIndex);
            bw.WriteInt32(m_offsetInSector);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_sectorIndex = xd.ReadInt32(xe, nameof(m_sectorIndex));
            m_offsetInSector = xd.ReadInt32(xe, nameof(m_offsetInSector));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_sectorIndex), m_sectorIndex);
            xs.WriteNumber(xe, nameof(m_offsetInSector), m_offsetInSector);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedSubTrack1nInfo);
        }

        public bool Equals(hkpSerializedSubTrack1nInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_sectorIndex.Equals(other.m_sectorIndex) &&
                   m_offsetInSector.Equals(other.m_offsetInSector) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_sectorIndex);
            hashcode.Add(m_offsetInSector);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

