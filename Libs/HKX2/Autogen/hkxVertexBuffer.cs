using System.Xml.Linq;
namespace HKX2
{
    // hkxVertexBuffer Signatire: 0x4ab10615 size: 136 flags: FLAGS_NONE

    // m_data m_class: hkxVertexBufferVertexData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_desc m_class: hkxVertexDescription Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    public partial class hkxVertexBuffer : hkReferencedObject, IEquatable<hkxVertexBuffer?>
    {
        public hkxVertexBufferVertexData m_data { set; get; } = new();
        public hkxVertexDescription m_desc { set; get; } = new();

        public override uint Signature { set; get; } = 0x4ab10615;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data.Read(des, br);
            m_desc.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_data.Write(s, bw);
            m_desc.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_data = xd.ReadClass<hkxVertexBufferVertexData>(xe, nameof(m_data));
            m_desc = xd.ReadClass<hkxVertexDescription>(xe, nameof(m_desc));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkxVertexBufferVertexData>(xe, nameof(m_data), m_data);
            xs.WriteClass(xe, nameof(m_desc), m_desc);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxVertexBuffer);
        }

        public bool Equals(hkxVertexBuffer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_data is null && other.m_data is null) || (m_data is not null && other.m_data is not null && m_data.Equals((IHavokObject)other.m_data))) &&
                   ((m_desc is null && other.m_desc is null) || (m_desc is not null && other.m_desc is not null && m_desc.Equals((IHavokObject)other.m_desc))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_data);
            hashcode.Add(m_desc);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

