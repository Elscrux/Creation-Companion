using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexSelectionChannel Signatire: 0x866ec6d0 size: 32 flags: FLAGS_NONE

    // m_selectedVertices m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxVertexSelectionChannel : hkReferencedObject, IEquatable<hkxVertexSelectionChannel?>
    {
        public IList<int> m_selectedVertices { set; get; } = Array.Empty<int>();

        public override uint Signature { set; get; } = 0x866ec6d0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_selectedVertices = des.ReadInt32Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteInt32Array(bw, m_selectedVertices);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_selectedVertices = xd.ReadInt32Array(xe, nameof(m_selectedVertices));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_selectedVertices), m_selectedVertices);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexSelectionChannel);
        }

        public bool Equals(hkxVertexSelectionChannel? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_selectedVertices.SequenceEqual(other.m_selectedVertices) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_selectedVertices.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

