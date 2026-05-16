using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedDisplayMarker Signatire: 0xd7c8c54f size: 80 flags: FLAGS_NONE

    // m_transform m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedDisplayMarker : hkReferencedObject, IEquatable<hkpSerializedDisplayMarker?>
    {
        public Matrix4x4 m_transform { set; get; }

        public override uint Signature { set; get; } = 0xd7c8c54f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transform = des.ReadTransform(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteTransform(bw, m_transform);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transform = xd.ReadTransform(xe, nameof(m_transform));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteTransform(xe, nameof(m_transform), m_transform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedDisplayMarker);
        }

        public bool Equals(hkpSerializedDisplayMarker? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transform.Equals(other.m_transform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

