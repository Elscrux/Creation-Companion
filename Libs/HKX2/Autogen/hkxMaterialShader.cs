using System.Xml.Linq;
namespace HKX2
{
    // hkxMaterialShader Signatire: 0x28515eff size: 72 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: ShaderType
    // m_vertexEntryName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_geomEntryName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_pixelEntryName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    public partial class hkxMaterialShader : hkReferencedObject, IEquatable<hkxMaterialShader?>
    {
        public string m_name { set; get; } = "";
        public byte m_type { set; get; }
        public string m_vertexEntryName { set; get; } = "";
        public string m_geomEntryName { set; get; } = "";
        public string m_pixelEntryName { set; get; } = "";
        public IList<byte> m_data { set; get; } = Array.Empty<byte>();

        public override uint Signature { set; get; } = 0x28515eff;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_type = br.ReadByte();
            br.Position += 7;
            m_vertexEntryName = des.ReadStringPointer(br);
            m_geomEntryName = des.ReadStringPointer(br);
            m_pixelEntryName = des.ReadStringPointer(br);
            m_data = des.ReadByteArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            bw.WriteByte(m_type);
            bw.Position += 7;
            s.WriteStringPointer(bw, m_vertexEntryName);
            s.WriteStringPointer(bw, m_geomEntryName);
            s.WriteStringPointer(bw, m_pixelEntryName);
            s.WriteByteArray(bw, m_data);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_type = xd.ReadFlag<ShaderType, byte>(xe, nameof(m_type));
            m_vertexEntryName = xd.ReadString(xe, nameof(m_vertexEntryName));
            m_geomEntryName = xd.ReadString(xe, nameof(m_geomEntryName));
            m_pixelEntryName = xd.ReadString(xe, nameof(m_pixelEntryName));
            m_data = xd.ReadByteArray(xe, nameof(m_data));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteEnum<ShaderType, byte>(xe, nameof(m_type), m_type);
            xs.WriteString(xe, nameof(m_vertexEntryName), m_vertexEntryName);
            xs.WriteString(xe, nameof(m_geomEntryName), m_geomEntryName);
            xs.WriteString(xe, nameof(m_pixelEntryName), m_pixelEntryName);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMaterialShader);
        }

        public bool Equals(hkxMaterialShader? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   m_type.Equals(other.m_type) &&
                   (m_vertexEntryName is null && other.m_vertexEntryName is null || m_vertexEntryName == other.m_vertexEntryName || m_vertexEntryName is null && other.m_vertexEntryName == "" || m_vertexEntryName == "" && other.m_vertexEntryName is null) &&
                   (m_geomEntryName is null && other.m_geomEntryName is null || m_geomEntryName == other.m_geomEntryName || m_geomEntryName is null && other.m_geomEntryName == "" || m_geomEntryName == "" && other.m_geomEntryName is null) &&
                   (m_pixelEntryName is null && other.m_pixelEntryName is null || m_pixelEntryName == other.m_pixelEntryName || m_pixelEntryName is null && other.m_pixelEntryName == "" || m_pixelEntryName == "" && other.m_pixelEntryName is null) &&
                   m_data.SequenceEqual(other.m_data) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_type);
            hashcode.Add(m_vertexEntryName);
            hashcode.Add(m_geomEntryName);
            hashcode.Add(m_pixelEntryName);
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

