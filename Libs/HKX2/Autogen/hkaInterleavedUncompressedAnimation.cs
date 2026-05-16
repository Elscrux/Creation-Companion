using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaInterleavedUncompressedAnimation Signatire: 0x930af031 size: 88 flags: FLAGS_NONE

    // m_transforms m_class:  Type.TYPE_ARRAY Type.TYPE_QSTRANSFORM arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_floats m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    public partial class hkaInterleavedUncompressedAnimation : hkaAnimation, IEquatable<hkaInterleavedUncompressedAnimation?>
    {
        public IList<Matrix4x4> m_transforms { set; get; } = Array.Empty<Matrix4x4>();
        public IList<float> m_floats { set; get; } = Array.Empty<float>();

        public override uint Signature { set; get; } = 0x930af031;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transforms = des.ReadQSTransformArray(br);
            m_floats = des.ReadSingleArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQSTransformArray(bw, m_transforms);
            s.WriteSingleArray(bw, m_floats);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transforms = xd.ReadQSTransformArray(xe, nameof(m_transforms));
            m_floats = xd.ReadSingleArray(xe, nameof(m_floats));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQSTransformArray(xe, nameof(m_transforms), m_transforms);
            xs.WriteFloatArray(xe, nameof(m_floats), m_floats);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaInterleavedUncompressedAnimation);
        }

        public bool Equals(hkaInterleavedUncompressedAnimation? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transforms.SequenceEqual(other.m_transforms) &&
                   m_floats.SequenceEqual(other.m_floats) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floats.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

