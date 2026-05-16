using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedDisplayRbTransforms Signatire: 0xc18650ac size: 32 flags: FLAGS_NONE

    // m_transforms m_class: hkpSerializedDisplayRbTransformsDisplayTransformPair Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedDisplayRbTransforms : hkReferencedObject, IEquatable<hkpSerializedDisplayRbTransforms?>
    {
        public IList<hkpSerializedDisplayRbTransformsDisplayTransformPair> m_transforms { set; get; } = Array.Empty<hkpSerializedDisplayRbTransformsDisplayTransformPair>();

        public override uint Signature { set; get; } = 0xc18650ac;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transforms = des.ReadClassArray<hkpSerializedDisplayRbTransformsDisplayTransformPair>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_transforms);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transforms = xd.ReadClassArray<hkpSerializedDisplayRbTransformsDisplayTransformPair>(xe, nameof(m_transforms));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_transforms), m_transforms);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedDisplayRbTransforms);
        }

        public bool Equals(hkpSerializedDisplayRbTransforms? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transforms.SequenceEqual(other.m_transforms) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transforms.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

