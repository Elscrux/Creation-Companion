using System.Xml.Linq;
namespace HKX2
{
    // hkpAgent1nSector Signatire: 0x626e55a size: 512 flags: FLAGS_NONE

    // m_bytesAllocated m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_pad0 m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_pad1 m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_pad2 m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 496 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpAgent1nSector : IHavokObject, IEquatable<hkpAgent1nSector?>
    {
        public uint m_bytesAllocated { set; get; }
        public uint m_pad0 { set; get; }
        public uint m_pad1 { set; get; }
        public uint m_pad2 { set; get; }
        public byte[] m_data = new byte[496];

        public virtual uint Signature { set; get; } = 0x626e55a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_bytesAllocated = br.ReadUInt32();
            m_pad0 = br.ReadUInt32();
            m_pad1 = br.ReadUInt32();
            m_pad2 = br.ReadUInt32();
            m_data = des.ReadByteCStyleArray(br, 496);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_bytesAllocated);
            bw.WriteUInt32(m_pad0);
            bw.WriteUInt32(m_pad1);
            bw.WriteUInt32(m_pad2);
            s.WriteByteCStyleArray(bw, m_data);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_bytesAllocated = xd.ReadUInt32(xe, nameof(m_bytesAllocated));
            m_pad0 = xd.ReadUInt32(xe, nameof(m_pad0));
            m_pad1 = xd.ReadUInt32(xe, nameof(m_pad1));
            m_pad2 = xd.ReadUInt32(xe, nameof(m_pad2));
            m_data = xd.ReadByteCStyleArray(xe, nameof(m_data), 496);
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_bytesAllocated), m_bytesAllocated);
            xs.WriteNumber(xe, nameof(m_pad0), m_pad0);
            xs.WriteNumber(xe, nameof(m_pad1), m_pad1);
            xs.WriteNumber(xe, nameof(m_pad2), m_pad2);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAgent1nSector);
        }

        public bool Equals(hkpAgent1nSector? other)
        {
            return other is not null &&
                   m_bytesAllocated.Equals(other.m_bytesAllocated) &&
                   m_pad0.Equals(other.m_pad0) &&
                   m_pad1.Equals(other.m_pad1) &&
                   m_pad2.Equals(other.m_pad2) &&
                   m_data.SequenceEqual(other.m_data) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_bytesAllocated);
            hashcode.Add(m_pad0);
            hashcode.Add(m_pad1);
            hashcode.Add(m_pad2);
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

