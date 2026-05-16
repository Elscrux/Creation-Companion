using System.Xml.Linq;
namespace HKX2
{
    // hkxIndexBuffer Signatire: 0xc12c8197 size: 64 flags: FLAGS_NONE

    // m_indexType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: IndexType
    // m_indices16 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_indices32 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_vertexBaseOffset m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_length m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    public partial class hkxIndexBuffer : hkReferencedObject, IEquatable<hkxIndexBuffer?>
    {
        public sbyte m_indexType { set; get; }
        public IList<ushort> m_indices16 { set; get; } = Array.Empty<ushort>();
        public IList<uint> m_indices32 { set; get; } = Array.Empty<uint>();
        public uint m_vertexBaseOffset { set; get; }
        public uint m_length { set; get; }

        public override uint Signature { set; get; } = 0xc12c8197;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_indexType = br.ReadSByte();
            br.Position += 7;
            m_indices16 = des.ReadUInt16Array(br);
            m_indices32 = des.ReadUInt32Array(br);
            m_vertexBaseOffset = br.ReadUInt32();
            m_length = br.ReadUInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSByte(m_indexType);
            bw.Position += 7;
            s.WriteUInt16Array(bw, m_indices16);
            s.WriteUInt32Array(bw, m_indices32);
            bw.WriteUInt32(m_vertexBaseOffset);
            bw.WriteUInt32(m_length);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_indexType = xd.ReadFlag<IndexType, sbyte>(xe, nameof(m_indexType));
            m_indices16 = xd.ReadUInt16Array(xe, nameof(m_indices16));
            m_indices32 = xd.ReadUInt32Array(xe, nameof(m_indices32));
            m_vertexBaseOffset = xd.ReadUInt32(xe, nameof(m_vertexBaseOffset));
            m_length = xd.ReadUInt32(xe, nameof(m_length));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteEnum<IndexType, sbyte>(xe, nameof(m_indexType), m_indexType);
            xs.WriteNumberArray(xe, nameof(m_indices16), m_indices16);
            xs.WriteNumberArray(xe, nameof(m_indices32), m_indices32);
            xs.WriteNumber(xe, nameof(m_vertexBaseOffset), m_vertexBaseOffset);
            xs.WriteNumber(xe, nameof(m_length), m_length);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxIndexBuffer);
        }

        public bool Equals(hkxIndexBuffer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_indexType.Equals(other.m_indexType) &&
                   m_indices16.SequenceEqual(other.m_indices16) &&
                   m_indices32.SequenceEqual(other.m_indices32) &&
                   m_vertexBaseOffset.Equals(other.m_vertexBaseOffset) &&
                   m_length.Equals(other.m_length) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_indexType);
            hashcode.Add(m_indices16.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_indices32.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_vertexBaseOffset);
            hashcode.Add(m_length);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

