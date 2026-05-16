using System.Xml.Linq;
namespace HKX2
{
    // hkbSetNodePropertyCommand Signatire: 0xc5160b64 size: 48 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_nodeName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_propertyName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_propertyValue m_class: hkbVariableValue Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_padding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    public partial class hkbSetNodePropertyCommand : hkReferencedObject, IEquatable<hkbSetNodePropertyCommand?>
    {
        public ulong m_characterId { set; get; }
        public string m_nodeName { set; get; } = "";
        public string m_propertyName { set; get; } = "";
        public hkbVariableValue m_propertyValue { set; get; } = new();
        public int m_padding { set; get; }

        public override uint Signature { set; get; } = 0xc5160b64;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_nodeName = des.ReadStringPointer(br);
            m_propertyName = des.ReadStringPointer(br);
            m_propertyValue.Read(des, br);
            m_padding = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            s.WriteStringPointer(bw, m_nodeName);
            s.WriteStringPointer(bw, m_propertyName);
            m_propertyValue.Write(s, bw);
            bw.WriteInt32(m_padding);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_nodeName = xd.ReadString(xe, nameof(m_nodeName));
            m_propertyName = xd.ReadString(xe, nameof(m_propertyName));
            m_propertyValue = xd.ReadClass<hkbVariableValue>(xe, nameof(m_propertyValue));
            m_padding = xd.ReadInt32(xe, nameof(m_padding));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteString(xe, nameof(m_nodeName), m_nodeName);
            xs.WriteString(xe, nameof(m_propertyName), m_propertyName);
            xs.WriteClass<hkbVariableValue>(xe, nameof(m_propertyValue), m_propertyValue);
            xs.WriteNumber(xe, nameof(m_padding), m_padding);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSetNodePropertyCommand);
        }

        public bool Equals(hkbSetNodePropertyCommand? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   (m_nodeName is null && other.m_nodeName is null || m_nodeName == other.m_nodeName || m_nodeName is null && other.m_nodeName == "" || m_nodeName == "" && other.m_nodeName is null) &&
                   (m_propertyName is null && other.m_propertyName is null || m_propertyName == other.m_propertyName || m_propertyName is null && other.m_propertyName == "" || m_propertyName == "" && other.m_propertyName is null) &&
                   ((m_propertyValue is null && other.m_propertyValue is null) || (m_propertyValue is not null && other.m_propertyValue is not null && m_propertyValue.Equals((IHavokObject)other.m_propertyValue))) &&
                   m_padding.Equals(other.m_padding) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_nodeName);
            hashcode.Add(m_propertyName);
            hashcode.Add(m_propertyValue);
            hashcode.Add(m_padding);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

