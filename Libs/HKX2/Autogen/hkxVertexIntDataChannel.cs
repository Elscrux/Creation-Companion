using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexIntDataChannel Signatire: 0x5a50e673 size: 32 flags: FLAGS_NONE

    // m_perVertexInts m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxVertexIntDataChannel : hkReferencedObject, IEquatable<hkxVertexIntDataChannel?>
    {
        public IList<int> m_perVertexInts { set; get; } = Array.Empty<int>();

        public override uint Signature { set; get; } = 0x5a50e673;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_perVertexInts = des.ReadInt32Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteInt32Array(bw, m_perVertexInts);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_perVertexInts = xd.ReadInt32Array(xe, nameof(m_perVertexInts));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_perVertexInts), m_perVertexInts);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexIntDataChannel);
        }

        public bool Equals(hkxVertexIntDataChannel? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_perVertexInts.SequenceEqual(other.m_perVertexInts) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_perVertexInts.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

