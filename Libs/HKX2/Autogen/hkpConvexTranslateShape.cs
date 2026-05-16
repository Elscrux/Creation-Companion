using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexTranslateShape Signatire: 0x5ba0a5f7 size: 80 flags: FLAGS_NONE

    // m_translation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpConvexTranslateShape : hkpConvexTransformShapeBase, IEquatable<hkpConvexTranslateShape?>
    {
        public Vector4 m_translation { set; get; }

        public override uint Signature { set; get; } = 0x5ba0a5f7;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_translation = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_translation);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_translation = xd.ReadVector4(xe, nameof(m_translation));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_translation), m_translation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexTranslateShape);
        }

        public bool Equals(hkpConvexTranslateShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_translation.Equals(other.m_translation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_translation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

