using System.Xml.Linq;
namespace HKX2
{
    // hkxMeshUserChannelInfo Signatire: 0x270724a5 size: 48 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_className m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkxMeshUserChannelInfo : hkxAttributeHolder, IEquatable<hkxMeshUserChannelInfo?>
    {
        public string m_name { set; get; } = "";
        public string m_className { set; get; } = "";

        public override uint Signature { set; get; } = 0x270724a5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_className = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteStringPointer(bw, m_className);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_className = xd.ReadString(xe, nameof(m_className));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteString(xe, nameof(m_className), m_className);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMeshUserChannelInfo);
        }

        public bool Equals(hkxMeshUserChannelInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   (m_className is null && other.m_className is null || m_className == other.m_className || m_className is null && other.m_className == "" || m_className == "" && other.m_className is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_className);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

