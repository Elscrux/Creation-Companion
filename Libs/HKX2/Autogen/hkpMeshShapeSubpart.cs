using System.Xml.Linq;
namespace HKX2
{
    // hkpMeshShapeSubpart Signatire: 0x27336e5d size: 80 flags: FLAGS_NONE

    // m_vertexBase m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_vertexStriding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_numVertices m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_indexBase m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_stridingType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: MeshShapeIndexStridingType
    // m_materialIndexStridingType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 25 flags: FLAGS_NONE enum: MeshShapeMaterialIndexStridingType
    // m_indexStriding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    // m_flipAlternateTriangles m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_numTriangles m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_materialIndexBase m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 40 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_materialIndexStriding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_materialBase m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_materialStriding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_numMaterials m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_triangleOffset m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    public partial class hkpMeshShapeSubpart : IHavokObject, IEquatable<hkpMeshShapeSubpart?>
    {
        private object? m_vertexBase { set; get; }
        public int m_vertexStriding { set; get; }
        public int m_numVertices { set; get; }
        private object? m_indexBase { set; get; }
        public sbyte m_stridingType { set; get; }
        public sbyte m_materialIndexStridingType { set; get; }
        public int m_indexStriding { set; get; }
        public int m_flipAlternateTriangles { set; get; }
        public int m_numTriangles { set; get; }
        private object? m_materialIndexBase { set; get; }
        public int m_materialIndexStriding { set; get; }
        private object? m_materialBase { set; get; }
        public int m_materialStriding { set; get; }
        public int m_numMaterials { set; get; }
        public int m_triangleOffset { set; get; }

        public virtual uint Signature { set; get; } = 0x27336e5d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            des.ReadEmptyPointer(br);
            m_vertexStriding = br.ReadInt32();
            m_numVertices = br.ReadInt32();
            des.ReadEmptyPointer(br);
            m_stridingType = br.ReadSByte();
            m_materialIndexStridingType = br.ReadSByte();
            br.Position += 2;
            m_indexStriding = br.ReadInt32();
            m_flipAlternateTriangles = br.ReadInt32();
            m_numTriangles = br.ReadInt32();
            des.ReadEmptyPointer(br);
            m_materialIndexStriding = br.ReadInt32();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            m_materialStriding = br.ReadInt32();
            m_numMaterials = br.ReadInt32();
            m_triangleOffset = br.ReadInt32();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_vertexStriding);
            bw.WriteInt32(m_numVertices);
            s.WriteVoidPointer(bw);
            bw.WriteSByte(m_stridingType);
            bw.WriteSByte(m_materialIndexStridingType);
            bw.Position += 2;
            bw.WriteInt32(m_indexStriding);
            bw.WriteInt32(m_flipAlternateTriangles);
            bw.WriteInt32(m_numTriangles);
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_materialIndexStriding);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            bw.WriteInt32(m_materialStriding);
            bw.WriteInt32(m_numMaterials);
            bw.WriteInt32(m_triangleOffset);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_vertexStriding = xd.ReadInt32(xe, nameof(m_vertexStriding));
            m_numVertices = xd.ReadInt32(xe, nameof(m_numVertices));
            m_stridingType = xd.ReadFlag<MeshShapeIndexStridingType, sbyte>(xe, nameof(m_stridingType));
            m_materialIndexStridingType = xd.ReadFlag<MeshShapeMaterialIndexStridingType, sbyte>(xe, nameof(m_materialIndexStridingType));
            m_indexStriding = xd.ReadInt32(xe, nameof(m_indexStriding));
            m_flipAlternateTriangles = xd.ReadInt32(xe, nameof(m_flipAlternateTriangles));
            m_numTriangles = xd.ReadInt32(xe, nameof(m_numTriangles));
            m_materialIndexStriding = xd.ReadInt32(xe, nameof(m_materialIndexStriding));
            m_materialStriding = xd.ReadInt32(xe, nameof(m_materialStriding));
            m_numMaterials = xd.ReadInt32(xe, nameof(m_numMaterials));
            m_triangleOffset = xd.ReadInt32(xe, nameof(m_triangleOffset));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_vertexBase));
            xs.WriteNumber(xe, nameof(m_vertexStriding), m_vertexStriding);
            xs.WriteNumber(xe, nameof(m_numVertices), m_numVertices);
            xs.WriteSerializeIgnored(xe, nameof(m_indexBase));
            xs.WriteEnum<MeshShapeIndexStridingType, sbyte>(xe, nameof(m_stridingType), m_stridingType);
            xs.WriteEnum<MeshShapeMaterialIndexStridingType, sbyte>(xe, nameof(m_materialIndexStridingType), m_materialIndexStridingType);
            xs.WriteNumber(xe, nameof(m_indexStriding), m_indexStriding);
            xs.WriteNumber(xe, nameof(m_flipAlternateTriangles), m_flipAlternateTriangles);
            xs.WriteNumber(xe, nameof(m_numTriangles), m_numTriangles);
            xs.WriteSerializeIgnored(xe, nameof(m_materialIndexBase));
            xs.WriteNumber(xe, nameof(m_materialIndexStriding), m_materialIndexStriding);
            xs.WriteSerializeIgnored(xe, nameof(m_materialBase));
            xs.WriteNumber(xe, nameof(m_materialStriding), m_materialStriding);
            xs.WriteNumber(xe, nameof(m_numMaterials), m_numMaterials);
            xs.WriteNumber(xe, nameof(m_triangleOffset), m_triangleOffset);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMeshShapeSubpart);
        }

        public bool Equals(hkpMeshShapeSubpart? other)
        {
            return other is not null &&
                   m_vertexStriding.Equals(other.m_vertexStriding) &&
                   m_numVertices.Equals(other.m_numVertices) &&
                   m_stridingType.Equals(other.m_stridingType) &&
                   m_materialIndexStridingType.Equals(other.m_materialIndexStridingType) &&
                   m_indexStriding.Equals(other.m_indexStriding) &&
                   m_flipAlternateTriangles.Equals(other.m_flipAlternateTriangles) &&
                   m_numTriangles.Equals(other.m_numTriangles) &&
                   m_materialIndexStriding.Equals(other.m_materialIndexStriding) &&
                   m_materialStriding.Equals(other.m_materialStriding) &&
                   m_numMaterials.Equals(other.m_numMaterials) &&
                   m_triangleOffset.Equals(other.m_triangleOffset) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_vertexStriding);
            hashcode.Add(m_numVertices);
            hashcode.Add(m_stridingType);
            hashcode.Add(m_materialIndexStridingType);
            hashcode.Add(m_indexStriding);
            hashcode.Add(m_flipAlternateTriangles);
            hashcode.Add(m_numTriangles);
            hashcode.Add(m_materialIndexStriding);
            hashcode.Add(m_materialStriding);
            hashcode.Add(m_numMaterials);
            hashcode.Add(m_triangleOffset);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

