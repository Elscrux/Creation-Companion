using System.Xml.Linq;
namespace HKX2
{
    // hkTrackerSerializableScanSnapshotAllocation Signatire: 0x9ab3a6ac size: 24 flags: FLAGS_NONE

    // m_start m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_size m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_traceId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkTrackerSerializableScanSnapshotAllocation : IHavokObject, IEquatable<hkTrackerSerializableScanSnapshotAllocation?>
    {
        public ulong m_start { set; get; }
        public ulong m_size { set; get; }
        public int m_traceId { set; get; }

        public virtual uint Signature { set; get; } = 0x9ab3a6ac;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_start = br.ReadUInt64();
            m_size = br.ReadUInt64();
            m_traceId = br.ReadInt32();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_start);
            bw.WriteUInt64(m_size);
            bw.WriteInt32(m_traceId);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_start = xd.ReadUInt64(xe, nameof(m_start));
            m_size = xd.ReadUInt64(xe, nameof(m_size));
            m_traceId = xd.ReadInt32(xe, nameof(m_traceId));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_start), m_start);
            xs.WriteNumber(xe, nameof(m_size), m_size);
            xs.WriteNumber(xe, nameof(m_traceId), m_traceId);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkTrackerSerializableScanSnapshotAllocation);
        }

        public bool Equals(hkTrackerSerializableScanSnapshotAllocation? other)
        {
            return other is not null &&
                   m_start.Equals(other.m_start) &&
                   m_size.Equals(other.m_size) &&
                   m_traceId.Equals(other.m_traceId) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_start);
            hashcode.Add(m_size);
            hashcode.Add(m_traceId);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

