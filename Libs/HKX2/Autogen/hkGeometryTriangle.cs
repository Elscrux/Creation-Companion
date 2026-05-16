using System.Xml.Linq;
namespace HKX2
{
    // hkGeometryTriangle Signatire: 0x9687513b size: 16 flags: FLAGS_NONE

    // m_a m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_b m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_c m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_material m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkGeometryTriangle : IHavokObject, IEquatable<hkGeometryTriangle?>
    {
        public int m_a { set; get; }
        public int m_b { set; get; }
        public int m_c { set; get; }
        public int m_material { set; get; }

        public virtual uint Signature { set; get; } = 0x9687513b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_a = br.ReadInt32();
            m_b = br.ReadInt32();
            m_c = br.ReadInt32();
            m_material = br.ReadInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_a);
            bw.WriteInt32(m_b);
            bw.WriteInt32(m_c);
            bw.WriteInt32(m_material);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_a = xd.ReadInt32(xe, nameof(m_a));
            m_b = xd.ReadInt32(xe, nameof(m_b));
            m_c = xd.ReadInt32(xe, nameof(m_c));
            m_material = xd.ReadInt32(xe, nameof(m_material));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_a), m_a);
            xs.WriteNumber(xe, nameof(m_b), m_b);
            xs.WriteNumber(xe, nameof(m_c), m_c);
            xs.WriteNumber(xe, nameof(m_material), m_material);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkGeometryTriangle);
        }

        public bool Equals(hkGeometryTriangle? other)
        {
            return other is not null &&
                   m_a.Equals(other.m_a) &&
                   m_b.Equals(other.m_b) &&
                   m_c.Equals(other.m_c) &&
                   m_material.Equals(other.m_material) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_a);
            hashcode.Add(m_b);
            hashcode.Add(m_c);
            hashcode.Add(m_material);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

