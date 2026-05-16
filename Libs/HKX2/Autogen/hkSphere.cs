using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkSphere Signatire: 0x143dff99 size: 16 flags: FLAGS_NONE

    // m_pos m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkSphere : IHavokObject, IEquatable<hkSphere?>
    {
        public Vector4 m_pos { set; get; }

        public virtual uint Signature { set; get; } = 0x143dff99;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pos = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_pos);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_pos = xd.ReadVector4(xe, nameof(m_pos));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_pos), m_pos);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkSphere);
        }

        public bool Equals(hkSphere? other)
        {
            return other is not null &&
                   m_pos.Equals(other.m_pos) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_pos);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

