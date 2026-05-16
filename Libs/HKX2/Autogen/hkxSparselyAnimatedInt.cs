using System.Xml.Linq;
namespace HKX2
{
    // hkxSparselyAnimatedInt Signatire: 0xca961951 size: 48 flags: FLAGS_NONE

    // m_ints m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_times m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkxSparselyAnimatedInt : hkReferencedObject, IEquatable<hkxSparselyAnimatedInt?>
    {
        public IList<int> m_ints { set; get; } = Array.Empty<int>();
        public IList<float> m_times { set; get; } = Array.Empty<float>();

        public override uint Signature { set; get; } = 0xca961951;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_ints = des.ReadInt32Array(br);
            m_times = des.ReadSingleArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteInt32Array(bw, m_ints);
            s.WriteSingleArray(bw, m_times);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_ints = xd.ReadInt32Array(xe, nameof(m_ints));
            m_times = xd.ReadSingleArray(xe, nameof(m_times));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_ints), m_ints);
            xs.WriteFloatArray(xe, nameof(m_times), m_times);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxSparselyAnimatedInt);
        }

        public bool Equals(hkxSparselyAnimatedInt? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_ints.SequenceEqual(other.m_ints) &&
                   m_times.SequenceEqual(other.m_times) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_ints.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_times.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

