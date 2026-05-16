using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpCompressedMeshShape Signatire: 0xe3d1dba size: 304 flags: FLAGS_NONE

    // m_bitsPerIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_bitsPerWIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_wIndexMask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_indexMask m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_radius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_weldingType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 68 flags: FLAGS_NONE enum: WeldingType
    // m_materialType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 69 flags: FLAGS_NONE enum: MaterialType
    // m_materials m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_materials16 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_materials8 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_transforms m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_bigVertices m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_bigTriangles m_class: hkpCompressedMeshShapeBigTriangle Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_chunks m_class: hkpCompressedMeshShapeChunk Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_convexPieces m_class: hkpCompressedMeshShapeConvexPiece Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_error m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 200 flags: FLAGS_NONE enum: 
    // m_bounds m_class: hkAabb Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 208 flags: FLAGS_NONE enum: 
    // m_defaultCollisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    // m_meshMaterials m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 248 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_materialStriding m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 256 flags: FLAGS_NONE enum: 
    // m_numMaterials m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 258 flags: FLAGS_NONE enum: 
    // m_namedMaterials m_class: hkpNamedMeshMaterial Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 264 flags: FLAGS_NONE enum: 
    // m_scaling m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 288 flags: FLAGS_NONE enum: 
    public partial class hkpCompressedMeshShape : hkpShapeCollection, IEquatable<hkpCompressedMeshShape?>
    {
        public int m_bitsPerIndex { set; get; }
        public int m_bitsPerWIndex { set; get; }
        public int m_wIndexMask { set; get; }
        public int m_indexMask { set; get; }
        public float m_radius { set; get; }
        public byte m_weldingType { set; get; }
        public byte m_materialType { set; get; }
        public IList<uint> m_materials { set; get; } = Array.Empty<uint>();
        public IList<ushort> m_materials16 { set; get; } = Array.Empty<ushort>();
        public IList<byte> m_materials8 { set; get; } = Array.Empty<byte>();
        public IList<Matrix4x4> m_transforms { set; get; } = Array.Empty<Matrix4x4>();
        public IList<Vector4> m_bigVertices { set; get; } = Array.Empty<Vector4>();
        public IList<hkpCompressedMeshShapeBigTriangle> m_bigTriangles { set; get; } = Array.Empty<hkpCompressedMeshShapeBigTriangle>();
        public IList<hkpCompressedMeshShapeChunk> m_chunks { set; get; } = Array.Empty<hkpCompressedMeshShapeChunk>();
        public IList<hkpCompressedMeshShapeConvexPiece> m_convexPieces { set; get; } = Array.Empty<hkpCompressedMeshShapeConvexPiece>();
        public float m_error { set; get; }
        public hkAabb m_bounds { set; get; } = new();
        public uint m_defaultCollisionFilterInfo { set; get; }
        private object? m_meshMaterials { set; get; }
        public ushort m_materialStriding { set; get; }
        public ushort m_numMaterials { set; get; }
        public IList<hkpNamedMeshMaterial> m_namedMaterials { set; get; } = Array.Empty<hkpNamedMeshMaterial>();
        public Vector4 m_scaling { set; get; }

        public override uint Signature { set; get; } = 0xe3d1dba;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bitsPerIndex = br.ReadInt32();
            m_bitsPerWIndex = br.ReadInt32();
            m_wIndexMask = br.ReadInt32();
            m_indexMask = br.ReadInt32();
            m_radius = br.ReadSingle();
            m_weldingType = br.ReadByte();
            m_materialType = br.ReadByte();
            br.Position += 2;
            m_materials = des.ReadUInt32Array(br);
            m_materials16 = des.ReadUInt16Array(br);
            m_materials8 = des.ReadByteArray(br);
            m_transforms = des.ReadQSTransformArray(br);
            m_bigVertices = des.ReadVector4Array(br);
            m_bigTriangles = des.ReadClassArray<hkpCompressedMeshShapeBigTriangle>(br);
            m_chunks = des.ReadClassArray<hkpCompressedMeshShapeChunk>(br);
            m_convexPieces = des.ReadClassArray<hkpCompressedMeshShapeConvexPiece>(br);
            m_error = br.ReadSingle();
            br.Position += 4;
            m_bounds.Read(des, br);
            m_defaultCollisionFilterInfo = br.ReadUInt32();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            m_materialStriding = br.ReadUInt16();
            m_numMaterials = br.ReadUInt16();
            br.Position += 4;
            m_namedMaterials = des.ReadClassArray<hkpNamedMeshMaterial>(br);
            br.Position += 8;
            m_scaling = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_bitsPerIndex);
            bw.WriteInt32(m_bitsPerWIndex);
            bw.WriteInt32(m_wIndexMask);
            bw.WriteInt32(m_indexMask);
            bw.WriteSingle(m_radius);
            bw.WriteByte(m_weldingType);
            bw.WriteByte(m_materialType);
            bw.Position += 2;
            s.WriteUInt32Array(bw, m_materials);
            s.WriteUInt16Array(bw, m_materials16);
            s.WriteByteArray(bw, m_materials8);
            s.WriteQSTransformArray(bw, m_transforms);
            s.WriteVector4Array(bw, m_bigVertices);
            s.WriteClassArray(bw, m_bigTriangles);
            s.WriteClassArray(bw, m_chunks);
            s.WriteClassArray(bw, m_convexPieces);
            bw.WriteSingle(m_error);
            bw.Position += 4;
            m_bounds.Write(s, bw);
            bw.WriteUInt32(m_defaultCollisionFilterInfo);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            bw.WriteUInt16(m_materialStriding);
            bw.WriteUInt16(m_numMaterials);
            bw.Position += 4;
            s.WriteClassArray(bw, m_namedMaterials);
            bw.Position += 8;
            bw.WriteVector4(m_scaling);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bitsPerIndex = xd.ReadInt32(xe, nameof(m_bitsPerIndex));
            m_bitsPerWIndex = xd.ReadInt32(xe, nameof(m_bitsPerWIndex));
            m_wIndexMask = xd.ReadInt32(xe, nameof(m_wIndexMask));
            m_indexMask = xd.ReadInt32(xe, nameof(m_indexMask));
            m_radius = xd.ReadSingle(xe, nameof(m_radius));
            m_weldingType = xd.ReadFlag<WeldingType, byte>(xe, nameof(m_weldingType));
            m_materialType = xd.ReadFlag<MaterialType, byte>(xe, nameof(m_materialType));
            m_materials = xd.ReadUInt32Array(xe, nameof(m_materials));
            m_materials16 = xd.ReadUInt16Array(xe, nameof(m_materials16));
            m_materials8 = xd.ReadByteArray(xe, nameof(m_materials8));
            m_transforms = xd.ReadQSTransformArray(xe, nameof(m_transforms));
            m_bigVertices = xd.ReadVector4Array(xe, nameof(m_bigVertices));
            m_bigTriangles = xd.ReadClassArray<hkpCompressedMeshShapeBigTriangle>(xe, nameof(m_bigTriangles));
            m_chunks = xd.ReadClassArray<hkpCompressedMeshShapeChunk>(xe, nameof(m_chunks));
            m_convexPieces = xd.ReadClassArray<hkpCompressedMeshShapeConvexPiece>(xe, nameof(m_convexPieces));
            m_error = xd.ReadSingle(xe, nameof(m_error));
            m_bounds = xd.ReadClass<hkAabb>(xe, nameof(m_bounds));
            m_defaultCollisionFilterInfo = xd.ReadUInt32(xe, nameof(m_defaultCollisionFilterInfo));
            m_materialStriding = xd.ReadUInt16(xe, nameof(m_materialStriding));
            m_numMaterials = xd.ReadUInt16(xe, nameof(m_numMaterials));
            m_namedMaterials = xd.ReadClassArray<hkpNamedMeshMaterial>(xe, nameof(m_namedMaterials));
            m_scaling = xd.ReadVector4(xe, nameof(m_scaling));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_bitsPerIndex), m_bitsPerIndex);
            xs.WriteNumber(xe, nameof(m_bitsPerWIndex), m_bitsPerWIndex);
            xs.WriteNumber(xe, nameof(m_wIndexMask), m_wIndexMask);
            xs.WriteNumber(xe, nameof(m_indexMask), m_indexMask);
            xs.WriteFloat(xe, nameof(m_radius), m_radius);
            xs.WriteEnum<WeldingType, byte>(xe, nameof(m_weldingType), m_weldingType);
            xs.WriteEnum<MaterialType, byte>(xe, nameof(m_materialType), m_materialType);
            xs.WriteNumberArray(xe, nameof(m_materials), m_materials);
            xs.WriteNumberArray(xe, nameof(m_materials16), m_materials16);
            xs.WriteNumberArray(xe, nameof(m_materials8), m_materials8);
            xs.WriteQSTransformArray(xe, nameof(m_transforms), m_transforms);
            xs.WriteVector4Array(xe, nameof(m_bigVertices), m_bigVertices);
            xs.WriteClassArray(xe, nameof(m_bigTriangles), m_bigTriangles);
            xs.WriteClassArray(xe, nameof(m_chunks), m_chunks);
            xs.WriteClassArray(xe, nameof(m_convexPieces), m_convexPieces);
            xs.WriteFloat(xe, nameof(m_error), m_error);
            xs.WriteClass<hkAabb>(xe, nameof(m_bounds), m_bounds);
            xs.WriteNumber(xe, nameof(m_defaultCollisionFilterInfo), m_defaultCollisionFilterInfo);
            xs.WriteSerializeIgnored(xe, nameof(m_meshMaterials));
            xs.WriteNumber(xe, nameof(m_materialStriding), m_materialStriding);
            xs.WriteNumber(xe, nameof(m_numMaterials), m_numMaterials);
            xs.WriteClassArray(xe, nameof(m_namedMaterials), m_namedMaterials);
            xs.WriteVector4(xe, nameof(m_scaling), m_scaling);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCompressedMeshShape);
        }

        public bool Equals(hkpCompressedMeshShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_bitsPerIndex.Equals(other.m_bitsPerIndex) &&
                   m_bitsPerWIndex.Equals(other.m_bitsPerWIndex) &&
                   m_wIndexMask.Equals(other.m_wIndexMask) &&
                   m_indexMask.Equals(other.m_indexMask) &&
                   m_radius.Equals(other.m_radius) &&
                   m_weldingType.Equals(other.m_weldingType) &&
                   m_materialType.Equals(other.m_materialType) &&
                   m_materials.SequenceEqual(other.m_materials) &&
                   m_materials16.SequenceEqual(other.m_materials16) &&
                   m_materials8.SequenceEqual(other.m_materials8) &&
                   m_transforms.SequenceEqual(other.m_transforms) &&
                   m_bigVertices.SequenceEqual(other.m_bigVertices) &&
                   m_bigTriangles.SequenceEqual(other.m_bigTriangles) &&
                   m_chunks.SequenceEqual(other.m_chunks) &&
                   m_convexPieces.SequenceEqual(other.m_convexPieces) &&
                   m_error.Equals(other.m_error) &&
                   ((m_bounds is null && other.m_bounds is null) || (m_bounds is not null && other.m_bounds is not null && m_bounds.Equals((IHavokObject)other.m_bounds))) &&
                   m_defaultCollisionFilterInfo.Equals(other.m_defaultCollisionFilterInfo) &&
                   m_materialStriding.Equals(other.m_materialStriding) &&
                   m_numMaterials.Equals(other.m_numMaterials) &&
                   m_namedMaterials.SequenceEqual(other.m_namedMaterials) &&
                   m_scaling.Equals(other.m_scaling) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bitsPerIndex);
            hashcode.Add(m_bitsPerWIndex);
            hashcode.Add(m_wIndexMask);
            hashcode.Add(m_indexMask);
            hashcode.Add(m_radius);
            hashcode.Add(m_weldingType);
            hashcode.Add(m_materialType);
            hashcode.Add(m_materials.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_materials16.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_materials8.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_transforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_bigVertices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_bigTriangles.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_chunks.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_convexPieces.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_error);
            hashcode.Add(m_bounds);
            hashcode.Add(m_defaultCollisionFilterInfo);
            hashcode.Add(m_materialStriding);
            hashcode.Add(m_numMaterials);
            hashcode.Add(m_namedMaterials.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_scaling);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

