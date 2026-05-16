using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorInfoIdToNamePair Signatire: 0x35a0439a size: 24 flags: FLAGS_NONE

    // m_behaviorName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_nodeName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_toolType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: ToolNodeType
    // m_id m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 18 flags: FLAGS_NONE enum: 
    public partial class hkbBehaviorInfoIdToNamePair : IHavokObject, IEquatable<hkbBehaviorInfoIdToNamePair?>
    {
        public string m_behaviorName { set; get; } = "";
        public string m_nodeName { set; get; } = "";
        public byte m_toolType { set; get; }
        public short m_id { set; get; }

        public virtual uint Signature { set; get; } = 0x35a0439a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_behaviorName = des.ReadStringPointer(br);
            m_nodeName = des.ReadStringPointer(br);
            m_toolType = br.ReadByte();
            br.Position += 1;
            m_id = br.ReadInt16();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_behaviorName);
            s.WriteStringPointer(bw, m_nodeName);
            bw.WriteByte(m_toolType);
            bw.Position += 1;
            bw.WriteInt16(m_id);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_behaviorName = xd.ReadString(xe, nameof(m_behaviorName));
            m_nodeName = xd.ReadString(xe, nameof(m_nodeName));
            m_toolType = xd.ReadFlag<ToolNodeType, byte>(xe, nameof(m_toolType));
            m_id = xd.ReadInt16(xe, nameof(m_id));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_behaviorName), m_behaviorName);
            xs.WriteString(xe, nameof(m_nodeName), m_nodeName);
            xs.WriteEnum<ToolNodeType, byte>(xe, nameof(m_toolType), m_toolType);
            xs.WriteNumber(xe, nameof(m_id), m_id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorInfoIdToNamePair);
        }

        public bool Equals(hkbBehaviorInfoIdToNamePair? other)
        {
            return other is not null &&
                   (m_behaviorName is null && other.m_behaviorName is null || m_behaviorName == other.m_behaviorName || m_behaviorName is null && other.m_behaviorName == "" || m_behaviorName == "" && other.m_behaviorName is null) &&
                   (m_nodeName is null && other.m_nodeName is null || m_nodeName == other.m_nodeName || m_nodeName is null && other.m_nodeName == "" || m_nodeName == "" && other.m_nodeName is null) &&
                   m_toolType.Equals(other.m_toolType) &&
                   m_id.Equals(other.m_id) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_behaviorName);
            hashcode.Add(m_nodeName);
            hashcode.Add(m_toolType);
            hashcode.Add(m_id);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

