using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexBufferVertexData Signatire: 0xd72b6fd0 size: 104 flags: FLAGS_NONE

    // m_vectorData m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_floatData m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_uint32Data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_uint16Data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_uint8Data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_numVerts m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_vectorStride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_floatStride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_uint32Stride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_uint16Stride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_uint8Stride m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    public partial class hkxVertexBufferVertexData : IHavokObject, IEquatable<hkxVertexBufferVertexData?>
    {
        public IList<Vector4> m_vectorData { set; get; } = Array.Empty<Vector4>();
        public IList<float> m_floatData { set; get; } = Array.Empty<float>();
        public IList<uint> m_uint32Data { set; get; } = Array.Empty<uint>();
        public IList<ushort> m_uint16Data { set; get; } = Array.Empty<ushort>();
        public IList<byte> m_uint8Data { set; get; } = Array.Empty<byte>();
        public uint m_numVerts { set; get; }
        public uint m_vectorStride { set; get; }
        public uint m_floatStride { set; get; }
        public uint m_uint32Stride { set; get; }
        public uint m_uint16Stride { set; get; }
        public uint m_uint8Stride { set; get; }

        public virtual uint Signature { set; get; } = 0xd72b6fd0;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vectorData = des.ReadVector4Array(br);
            m_floatData = des.ReadSingleArray(br);
            m_uint32Data = des.ReadUInt32Array(br);
            m_uint16Data = des.ReadUInt16Array(br);
            m_uint8Data = des.ReadByteArray(br);
            m_numVerts = br.ReadUInt32();
            m_vectorStride = br.ReadUInt32();
            m_floatStride = br.ReadUInt32();
            m_uint32Stride = br.ReadUInt32();
            m_uint16Stride = br.ReadUInt32();
            m_uint8Stride = br.ReadUInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVector4Array(bw, m_vectorData);
            s.WriteSingleArray(bw, m_floatData);
            s.WriteUInt32Array(bw, m_uint32Data);
            s.WriteUInt16Array(bw, m_uint16Data);
            s.WriteByteArray(bw, m_uint8Data);
            bw.WriteUInt32(m_numVerts);
            bw.WriteUInt32(m_vectorStride);
            bw.WriteUInt32(m_floatStride);
            bw.WriteUInt32(m_uint32Stride);
            bw.WriteUInt32(m_uint16Stride);
            bw.WriteUInt32(m_uint8Stride);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_vectorData = xd.ReadVector4Array(xe, nameof(m_vectorData));
            m_floatData = xd.ReadSingleArray(xe, nameof(m_floatData));
            m_uint32Data = xd.ReadUInt32Array(xe, nameof(m_uint32Data));
            m_uint16Data = xd.ReadUInt16Array(xe, nameof(m_uint16Data));
            m_uint8Data = xd.ReadByteArray(xe, nameof(m_uint8Data));
            m_numVerts = xd.ReadUInt32(xe, nameof(m_numVerts));
            m_vectorStride = xd.ReadUInt32(xe, nameof(m_vectorStride));
            m_floatStride = xd.ReadUInt32(xe, nameof(m_floatStride));
            m_uint32Stride = xd.ReadUInt32(xe, nameof(m_uint32Stride));
            m_uint16Stride = xd.ReadUInt32(xe, nameof(m_uint16Stride));
            m_uint8Stride = xd.ReadUInt32(xe, nameof(m_uint8Stride));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4Array(xe, nameof(m_vectorData), m_vectorData);
            xs.WriteFloatArray(xe, nameof(m_floatData), m_floatData);
            xs.WriteNumberArray(xe, nameof(m_uint32Data), m_uint32Data);
            xs.WriteNumberArray(xe, nameof(m_uint16Data), m_uint16Data);
            xs.WriteNumberArray(xe, nameof(m_uint8Data), m_uint8Data);
            xs.WriteNumber(xe, nameof(m_numVerts), m_numVerts);
            xs.WriteNumber(xe, nameof(m_vectorStride), m_vectorStride);
            xs.WriteNumber(xe, nameof(m_floatStride), m_floatStride);
            xs.WriteNumber(xe, nameof(m_uint32Stride), m_uint32Stride);
            xs.WriteNumber(xe, nameof(m_uint16Stride), m_uint16Stride);
            xs.WriteNumber(xe, nameof(m_uint8Stride), m_uint8Stride);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexBufferVertexData);
        }

        public bool Equals(hkxVertexBufferVertexData? other)
        {
            return other is not null &&
                   m_vectorData.SequenceEqual(other.m_vectorData) &&
                   m_floatData.SequenceEqual(other.m_floatData) &&
                   m_uint32Data.SequenceEqual(other.m_uint32Data) &&
                   m_uint16Data.SequenceEqual(other.m_uint16Data) &&
                   m_uint8Data.SequenceEqual(other.m_uint8Data) &&
                   m_numVerts.Equals(other.m_numVerts) &&
                   m_vectorStride.Equals(other.m_vectorStride) &&
                   m_floatStride.Equals(other.m_floatStride) &&
                   m_uint32Stride.Equals(other.m_uint32Stride) &&
                   m_uint16Stride.Equals(other.m_uint16Stride) &&
                   m_uint8Stride.Equals(other.m_uint8Stride) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_vectorData.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floatData.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_uint32Data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_uint16Data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_uint8Data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_numVerts);
            hashcode.Add(m_vectorStride);
            hashcode.Add(m_floatStride);
            hashcode.Add(m_uint32Stride);
            hashcode.Add(m_uint16Stride);
            hashcode.Add(m_uint8Stride);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

