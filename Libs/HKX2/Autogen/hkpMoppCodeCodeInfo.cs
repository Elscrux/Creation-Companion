using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMoppCodeCodeInfo Signatire: 0xd8fdbb08 size: 16 flags: FLAGS_NONE

    // m_offset m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkpMoppCodeCodeInfo : IHavokObject, IEquatable<hkpMoppCodeCodeInfo?>
    {
        public Vector4 m_offset { set; get; }

        public virtual uint Signature { set; get; } = 0xd8fdbb08;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_offset);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_offset = xd.ReadVector4(xe, nameof(m_offset));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_offset), m_offset);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMoppCodeCodeInfo);
        }

        public bool Equals(hkpMoppCodeCodeInfo? other)
        {
            return other is not null &&
                   m_offset.Equals(other.m_offset) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_offset);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

