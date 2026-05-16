using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkAabb Signatire: 0x4a948b16 size: 32 flags: FLAGS_NONE

    // m_min m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_max m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkAabb : IHavokObject, IEquatable<hkAabb?>
    {
        public Vector4 m_min { set; get; }
        public Vector4 m_max { set; get; }

        public virtual uint Signature { set; get; } = 0x4a948b16;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = br.ReadVector4();
            m_max = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_min);
            bw.WriteVector4(m_max);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_min = xd.ReadVector4(xe, nameof(m_min));
            m_max = xd.ReadVector4(xe, nameof(m_max));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_min), m_min);
            xs.WriteVector4(xe, nameof(m_max), m_max);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkAabb);
        }

        public bool Equals(hkAabb? other)
        {
            return other is not null &&
                   m_min.Equals(other.m_min) &&
                   m_max.Equals(other.m_max) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_min);
            hashcode.Add(m_max);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

