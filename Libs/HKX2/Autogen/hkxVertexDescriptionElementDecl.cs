using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexDescriptionElementDecl Signatire: 0x483a429b size: 16 flags: FLAGS_NONE

    // m_byteOffset m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT16 arrSize: 0 offset: 4 flags: FLAGS_NONE enum: DataType
    // m_usage m_class:  Type.TYPE_ENUM Type.TYPE_UINT16 arrSize: 0 offset: 6 flags: FLAGS_NONE enum: DataUsage
    // m_byteStride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_numElements m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkxVertexDescriptionElementDecl : IHavokObject, IEquatable<hkxVertexDescriptionElementDecl?>
    {
        public uint m_byteOffset { set; get; }
        public ushort m_type { set; get; }
        public ushort m_usage { set; get; }
        public uint m_byteStride { set; get; }
        public byte m_numElements { set; get; }

        public virtual uint Signature { set; get; } = 0x483a429b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_byteOffset = br.ReadUInt32();
            m_type = br.ReadUInt16();
            m_usage = br.ReadUInt16();
            m_byteStride = br.ReadUInt32();
            m_numElements = br.ReadByte();
            br.Position += 3;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_byteOffset);
            bw.WriteUInt16(m_type);
            bw.WriteUInt16(m_usage);
            bw.WriteUInt32(m_byteStride);
            bw.WriteByte(m_numElements);
            bw.Position += 3;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_byteOffset = xd.ReadUInt32(xe, nameof(m_byteOffset));
            m_type = xd.ReadFlag<DataType, ushort>(xe, nameof(m_type));
            m_usage = xd.ReadFlag<DataUsage, ushort>(xe, nameof(m_usage));
            m_byteStride = xd.ReadUInt32(xe, nameof(m_byteStride));
            m_numElements = xd.ReadByte(xe, nameof(m_numElements));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_byteOffset), m_byteOffset);
            xs.WriteEnum<DataType, ushort>(xe, nameof(m_type), m_type);
            xs.WriteEnum<DataUsage, ushort>(xe, nameof(m_usage), m_usage);
            xs.WriteNumber(xe, nameof(m_byteStride), m_byteStride);
            xs.WriteNumber(xe, nameof(m_numElements), m_numElements);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexDescriptionElementDecl);
        }

        public bool Equals(hkxVertexDescriptionElementDecl? other)
        {
            return other is not null &&
                   m_byteOffset.Equals(other.m_byteOffset) &&
                   m_type.Equals(other.m_type) &&
                   m_usage.Equals(other.m_usage) &&
                   m_byteStride.Equals(other.m_byteStride) &&
                   m_numElements.Equals(other.m_numElements) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_byteOffset);
            hashcode.Add(m_type);
            hashcode.Add(m_usage);
            hashcode.Add(m_byteStride);
            hashcode.Add(m_numElements);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

