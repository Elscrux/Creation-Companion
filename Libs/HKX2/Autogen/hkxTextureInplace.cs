using System.Xml.Linq;
namespace HKX2
{
    // hkxTextureInplace Signatire: 0xd45841d6 size: 56 flags: FLAGS_NONE

    // m_fileType m_class:  Type.TYPE_CHAR Type.TYPE_VOID arrSize: 4 offset: 16 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_originalFilename m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkxTextureInplace : hkReferencedObject, IEquatable<hkxTextureInplace?>
    {
        public string m_fileType { set; get; } = "";
        public IList<byte> m_data { set; get; } = Array.Empty<byte>();
        public string m_name { set; get; } = "";
        public string m_originalFilename { set; get; } = "";

        public override uint Signature { set; get; } = 0xd45841d6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_fileType = br.ReadASCII(4);
            br.Position += 4;
            m_data = des.ReadByteArray(br);
            m_name = des.ReadStringPointer(br);
            m_originalFilename = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteASCII(m_fileType);
            bw.Position += 4;
            s.WriteByteArray(bw, m_data);
            s.WriteStringPointer(bw, m_name);
            s.WriteStringPointer(bw, m_originalFilename);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_fileType = xd.ReadString(xe, nameof(m_fileType));
            m_data = xd.ReadByteArray(xe, nameof(m_data));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_originalFilename = xd.ReadString(xe, nameof(m_originalFilename));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_fileType), m_fileType);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteString(xe, nameof(m_originalFilename), m_originalFilename);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxTextureInplace);
        }

        public bool Equals(hkxTextureInplace? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_fileType.SequenceEqual(other.m_fileType) &&
                   m_data.SequenceEqual(other.m_data) &&
                   m_name == other.m_name &&
                   (m_originalFilename is null && other.m_originalFilename is null || m_originalFilename == other.m_originalFilename || m_originalFilename is null && other.m_originalFilename == "" || m_originalFilename == "" && other.m_originalFilename is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_fileType.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_name);
            hashcode.Add(m_originalFilename);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

