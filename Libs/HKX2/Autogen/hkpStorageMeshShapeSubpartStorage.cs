using System.Xml.Linq;
namespace HKX2
{
    // hkpStorageMeshShapeSubpartStorage Signatire: 0xbf27438 size: 112 flags: FLAGS_NONE

    // m_vertices m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_indices16 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_indices32 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_materialIndices m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_materials m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_materialIndices16 m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkpStorageMeshShapeSubpartStorage : hkReferencedObject, IEquatable<hkpStorageMeshShapeSubpartStorage?>
    {
        public IList<float> m_vertices { set; get; } = Array.Empty<float>();
        public IList<ushort> m_indices16 { set; get; } = Array.Empty<ushort>();
        public IList<uint> m_indices32 { set; get; } = Array.Empty<uint>();
        public IList<byte> m_materialIndices { set; get; } = Array.Empty<byte>();
        public IList<uint> m_materials { set; get; } = Array.Empty<uint>();
        public IList<ushort> m_materialIndices16 { set; get; } = Array.Empty<ushort>();

        public override uint Signature { set; get; } = 0xbf27438;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertices = des.ReadSingleArray(br);
            m_indices16 = des.ReadUInt16Array(br);
            m_indices32 = des.ReadUInt32Array(br);
            m_materialIndices = des.ReadByteArray(br);
            m_materials = des.ReadUInt32Array(br);
            m_materialIndices16 = des.ReadUInt16Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_vertices);
            s.WriteUInt16Array(bw, m_indices16);
            s.WriteUInt32Array(bw, m_indices32);
            s.WriteByteArray(bw, m_materialIndices);
            s.WriteUInt32Array(bw, m_materials);
            s.WriteUInt16Array(bw, m_materialIndices16);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vertices = xd.ReadSingleArray(xe, nameof(m_vertices));
            m_indices16 = xd.ReadUInt16Array(xe, nameof(m_indices16));
            m_indices32 = xd.ReadUInt32Array(xe, nameof(m_indices32));
            m_materialIndices = xd.ReadByteArray(xe, nameof(m_materialIndices));
            m_materials = xd.ReadUInt32Array(xe, nameof(m_materials));
            m_materialIndices16 = xd.ReadUInt16Array(xe, nameof(m_materialIndices16));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloatArray(xe, nameof(m_vertices), m_vertices);
            xs.WriteNumberArray(xe, nameof(m_indices16), m_indices16);
            xs.WriteNumberArray(xe, nameof(m_indices32), m_indices32);
            xs.WriteNumberArray(xe, nameof(m_materialIndices), m_materialIndices);
            xs.WriteNumberArray(xe, nameof(m_materials), m_materials);
            xs.WriteNumberArray(xe, nameof(m_materialIndices16), m_materialIndices16);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStorageMeshShapeSubpartStorage);
        }

        public bool Equals(hkpStorageMeshShapeSubpartStorage? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_vertices.SequenceEqual(other.m_vertices) &&
                   m_indices16.SequenceEqual(other.m_indices16) &&
                   m_indices32.SequenceEqual(other.m_indices32) &&
                   m_materialIndices.SequenceEqual(other.m_materialIndices) &&
                   m_materials.SequenceEqual(other.m_materials) &&
                   m_materialIndices16.SequenceEqual(other.m_materialIndices16) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vertices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_indices16.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_indices32.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_materialIndices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_materials.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_materialIndices16.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

