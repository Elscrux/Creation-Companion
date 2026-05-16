using System.Xml.Linq;
namespace HKX2
{
    // hkxMeshSection Signatire: 0xe2286cf8 size: 64 flags: FLAGS_NONE

    // m_vertexBuffer m_class: hkxVertexBuffer Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_indexBuffers m_class: hkxIndexBuffer Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_material m_class: hkxMaterial Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_userChannels m_class: hkReferencedObject Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkxMeshSection : hkReferencedObject, IEquatable<hkxMeshSection?>
    {
        public hkxVertexBuffer? m_vertexBuffer { set; get; }
        public IList<hkxIndexBuffer> m_indexBuffers { set; get; } = Array.Empty<hkxIndexBuffer>();
        public hkxMaterial? m_material { set; get; }
        public IList<hkReferencedObject> m_userChannels { set; get; } = Array.Empty<hkReferencedObject>();

        public override uint Signature { set; get; } = 0xe2286cf8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexBuffer = des.ReadClassPointer<hkxVertexBuffer>(br);
            m_indexBuffers = des.ReadClassPointerArray<hkxIndexBuffer>(br);
            m_material = des.ReadClassPointer<hkxMaterial>(br);
            m_userChannels = des.ReadClassPointerArray<hkReferencedObject>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_vertexBuffer);
            s.WriteClassPointerArray(bw, m_indexBuffers);
            s.WriteClassPointer(bw, m_material);
            s.WriteClassPointerArray(bw, m_userChannels);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vertexBuffer = xd.ReadClassPointer<hkxVertexBuffer>(xe, nameof(m_vertexBuffer));
            m_indexBuffers = xd.ReadClassPointerArray<hkxIndexBuffer>(xe, nameof(m_indexBuffers));
            m_material = xd.ReadClassPointer<hkxMaterial>(xe, nameof(m_material));
            m_userChannels = xd.ReadClassPointerArray<hkReferencedObject>(xe, nameof(m_userChannels));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_vertexBuffer), m_vertexBuffer);
            xs.WriteClassPointerArray(xe, nameof(m_indexBuffers), m_indexBuffers);
            xs.WriteClassPointer(xe, nameof(m_material), m_material);
            xs.WriteClassPointerArray(xe, nameof(m_userChannels), m_userChannels);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMeshSection);
        }

        public bool Equals(hkxMeshSection? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_vertexBuffer is null && other.m_vertexBuffer is null) || (m_vertexBuffer is not null && other.m_vertexBuffer is not null && m_vertexBuffer.Equals((IHavokObject)other.m_vertexBuffer))) &&
                   m_indexBuffers.SequenceEqual(other.m_indexBuffers) &&
                   ((m_material is null && other.m_material is null) || (m_material is not null && other.m_material is not null && m_material.Equals((IHavokObject)other.m_material))) &&
                   m_userChannels.SequenceEqual(other.m_userChannels) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vertexBuffer);
            hashcode.Add(m_indexBuffers.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_material);
            hashcode.Add(m_userChannels.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

