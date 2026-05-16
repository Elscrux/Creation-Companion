using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkQTransform Signatire: 0x471a21ee size: 32 flags: FLAGS_NONE

    // m_rotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_translation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkQTransform : IHavokObject, IEquatable<hkQTransform?>
    {
        public Quaternion m_rotation { set; get; }
        public Vector4 m_translation { set; get; }

        public virtual uint Signature { set; get; } = 0x471a21ee;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotation = des.ReadQuaternion(br);
            m_translation = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteQuaternion(bw, m_rotation);
            bw.WriteVector4(m_translation);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rotation = xd.ReadQuaternion(xe, nameof(m_rotation));
            m_translation = xd.ReadVector4(xe, nameof(m_translation));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteQuaternion(xe, nameof(m_rotation), m_rotation);
            xs.WriteVector4(xe, nameof(m_translation), m_translation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkQTransform);
        }

        public bool Equals(hkQTransform? other)
        {
            return other is not null &&
                   m_rotation.Equals(other.m_rotation) &&
                   m_translation.Equals(other.m_translation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rotation);
            hashcode.Add(m_translation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

