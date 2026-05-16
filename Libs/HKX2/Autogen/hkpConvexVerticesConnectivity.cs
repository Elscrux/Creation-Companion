using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexVerticesConnectivity Signatire: 0x63d38e9c size: 48 flags: FLAGS_NONE

    // m_vertexIndices m_class:  Type.TYPE_ARRAY Type.TYPE_UINT16 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_numVerticesPerFace m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpConvexVerticesConnectivity : hkReferencedObject, IEquatable<hkpConvexVerticesConnectivity?>
    {
        public IList<ushort> m_vertexIndices { set; get; } = Array.Empty<ushort>();
        public IList<byte> m_numVerticesPerFace { set; get; } = Array.Empty<byte>();

        public override uint Signature { set; get; } = 0x63d38e9c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexIndices = des.ReadUInt16Array(br);
            m_numVerticesPerFace = des.ReadByteArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteUInt16Array(bw, m_vertexIndices);
            s.WriteByteArray(bw, m_numVerticesPerFace);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vertexIndices = xd.ReadUInt16Array(xe, nameof(m_vertexIndices));
            m_numVerticesPerFace = xd.ReadByteArray(xe, nameof(m_numVerticesPerFace));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_vertexIndices), m_vertexIndices);
            xs.WriteNumberArray(xe, nameof(m_numVerticesPerFace), m_numVerticesPerFace);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexVerticesConnectivity);
        }

        public bool Equals(hkpConvexVerticesConnectivity? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_vertexIndices.SequenceEqual(other.m_vertexIndices) &&
                   m_numVerticesPerFace.SequenceEqual(other.m_numVerticesPerFace) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vertexIndices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_numVerticesPerFace.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

