using System.Xml.Linq;
namespace HKX2
{
    // hkTrackerSerializableScanSnapshotBlock Signatire: 0xe7f23e6d size: 40 flags: FLAGS_NONE

    // m_typeIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_start m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_size m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_arraySize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_startReferenceIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    // m_numReferences m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkTrackerSerializableScanSnapshotBlock : IHavokObject, IEquatable<hkTrackerSerializableScanSnapshotBlock?>
    {
        public int m_typeIndex { set; get; }
        public ulong m_start { set; get; }
        public ulong m_size { set; get; }
        public int m_arraySize { set; get; }
        public int m_startReferenceIndex { set; get; }
        public int m_numReferences { set; get; }

        public virtual uint Signature { set; get; } = 0xe7f23e6d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_typeIndex = br.ReadInt32();
            br.Position += 4;
            m_start = br.ReadUInt64();
            m_size = br.ReadUInt64();
            m_arraySize = br.ReadInt32();
            m_startReferenceIndex = br.ReadInt32();
            m_numReferences = br.ReadInt32();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_typeIndex);
            bw.Position += 4;
            bw.WriteUInt64(m_start);
            bw.WriteUInt64(m_size);
            bw.WriteInt32(m_arraySize);
            bw.WriteInt32(m_startReferenceIndex);
            bw.WriteInt32(m_numReferences);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_typeIndex = xd.ReadInt32(xe, nameof(m_typeIndex));
            m_start = xd.ReadUInt64(xe, nameof(m_start));
            m_size = xd.ReadUInt64(xe, nameof(m_size));
            m_arraySize = xd.ReadInt32(xe, nameof(m_arraySize));
            m_startReferenceIndex = xd.ReadInt32(xe, nameof(m_startReferenceIndex));
            m_numReferences = xd.ReadInt32(xe, nameof(m_numReferences));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_typeIndex), m_typeIndex);
            xs.WriteNumber(xe, nameof(m_start), m_start);
            xs.WriteNumber(xe, nameof(m_size), m_size);
            xs.WriteNumber(xe, nameof(m_arraySize), m_arraySize);
            xs.WriteNumber(xe, nameof(m_startReferenceIndex), m_startReferenceIndex);
            xs.WriteNumber(xe, nameof(m_numReferences), m_numReferences);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkTrackerSerializableScanSnapshotBlock);
        }

        public bool Equals(hkTrackerSerializableScanSnapshotBlock? other)
        {
            return other is not null &&
                   m_typeIndex.Equals(other.m_typeIndex) &&
                   m_start.Equals(other.m_start) &&
                   m_size.Equals(other.m_size) &&
                   m_arraySize.Equals(other.m_arraySize) &&
                   m_startReferenceIndex.Equals(other.m_startReferenceIndex) &&
                   m_numReferences.Equals(other.m_numReferences) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_typeIndex);
            hashcode.Add(m_start);
            hashcode.Add(m_size);
            hashcode.Add(m_arraySize);
            hashcode.Add(m_startReferenceIndex);
            hashcode.Add(m_numReferences);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

