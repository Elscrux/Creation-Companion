using System.Xml.Linq;
namespace HKX2
{
    // hkxTextureFile Signatire: 0x1e289259 size: 40 flags: FLAGS_NONE

    // m_filename m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_originalFilename m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkxTextureFile : hkReferencedObject, IEquatable<hkxTextureFile?>
    {
        public string m_filename { set; get; } = "";
        public string m_name { set; get; } = "";
        public string m_originalFilename { set; get; } = "";

        public override uint Signature { set; get; } = 0x1e289259;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_filename = des.ReadStringPointer(br);
            m_name = des.ReadStringPointer(br);
            m_originalFilename = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_filename);
            s.WriteStringPointer(bw, m_name);
            s.WriteStringPointer(bw, m_originalFilename);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_filename = xd.ReadString(xe, nameof(m_filename));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_originalFilename = xd.ReadString(xe, nameof(m_originalFilename));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_filename), m_filename);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteString(xe, nameof(m_originalFilename), m_originalFilename);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxTextureFile);
        }

        public bool Equals(hkxTextureFile? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_filename is null && other.m_filename is null || m_filename == other.m_filename || m_filename is null && other.m_filename == "" || m_filename == "" && other.m_filename is null) &&
                   m_name == other.m_name &&
                   (m_originalFilename is null && other.m_originalFilename is null || m_originalFilename == other.m_originalFilename || m_originalFilename is null && other.m_originalFilename == "" || m_originalFilename == "" && other.m_originalFilename is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_filename);
            hashcode.Add(m_name);
            hashcode.Add(m_originalFilename);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

