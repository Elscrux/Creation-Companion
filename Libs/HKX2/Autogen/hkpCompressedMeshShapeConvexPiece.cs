using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpCompressedMeshShapeConvexPiece Signatire: 0x385bb842 size: 80 flags: FLAGS_NONE

    // m_offset m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_vertices m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_faceVertices m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_faceOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_reference m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_transformIndex m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 66 flags: FLAGS_NONE enum: 
    public partial class hkpCompressedMeshShapeConvexPiece : IHavokObject, IEquatable<hkpCompressedMeshShapeConvexPiece?>
    {
        public Vector4 m_offset { set; get; }
        public IList<ushort> m_vertices { set; get; } = Array.Empty<ushort>();
        public IList<byte> m_faceVertices { set; get; } = Array.Empty<byte>();
        public IList<ushort> m_faceOffsets { set; get; } = Array.Empty<ushort>();
        public ushort m_reference { set; get; }
        public ushort m_transformIndex { set; get; }

        public virtual uint Signature { set; get; } = 0x385bb842;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = br.ReadVector4();
            m_vertices = des.ReadUInt16Array(br);
            m_faceVertices = des.ReadByteArray(br);
            m_faceOffsets = des.ReadUInt16Array(br);
            m_reference = br.ReadUInt16();
            m_transformIndex = br.ReadUInt16();
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_offset);
            s.WriteUInt16Array(bw, m_vertices);
            s.WriteByteArray(bw, m_faceVertices);
            s.WriteUInt16Array(bw, m_faceOffsets);
            bw.WriteUInt16(m_reference);
            bw.WriteUInt16(m_transformIndex);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_offset = xd.ReadVector4(xe, nameof(m_offset));
            m_vertices = xd.ReadUInt16Array(xe, nameof(m_vertices));
            m_faceVertices = xd.ReadByteArray(xe, nameof(m_faceVertices));
            m_faceOffsets = xd.ReadUInt16Array(xe, nameof(m_faceOffsets));
            m_reference = xd.ReadUInt16(xe, nameof(m_reference));
            m_transformIndex = xd.ReadUInt16(xe, nameof(m_transformIndex));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_offset), m_offset);
            xs.WriteNumberArray(xe, nameof(m_vertices), m_vertices);
            xs.WriteNumberArray(xe, nameof(m_faceVertices), m_faceVertices);
            xs.WriteNumberArray(xe, nameof(m_faceOffsets), m_faceOffsets);
            xs.WriteNumber(xe, nameof(m_reference), m_reference);
            xs.WriteNumber(xe, nameof(m_transformIndex), m_transformIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCompressedMeshShapeConvexPiece);
        }

        public bool Equals(hkpCompressedMeshShapeConvexPiece? other)
        {
            return other is not null &&
                   m_offset.Equals(other.m_offset) &&
                   m_vertices.SequenceEqual(other.m_vertices) &&
                   m_faceVertices.SequenceEqual(other.m_faceVertices) &&
                   m_faceOffsets.SequenceEqual(other.m_faceOffsets) &&
                   m_reference.Equals(other.m_reference) &&
                   m_transformIndex.Equals(other.m_transformIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_offset);
            hashcode.Add(m_vertices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_faceVertices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_faceOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_reference);
            hashcode.Add(m_transformIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

