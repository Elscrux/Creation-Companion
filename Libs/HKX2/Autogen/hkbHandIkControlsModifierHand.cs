using System.Xml.Linq;
namespace HKX2
{
    // hkbHandIkControlsModifierHand Signatire: 0x9c72e9e3 size: 112 flags: FLAGS_NONE

    // m_controlData m_class: hkbHandIkControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_handIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_enable m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    public partial class hkbHandIkControlsModifierHand : IHavokObject, IEquatable<hkbHandIkControlsModifierHand?>
    {
        public hkbHandIkControlData m_controlData { set; get; } = new();
        public int m_handIndex { set; get; }
        public bool m_enable { set; get; }

        public virtual uint Signature { set; get; } = 0x9c72e9e3;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_controlData.Read(des, br);
            m_handIndex = br.ReadInt32();
            m_enable = br.ReadBoolean();
            br.Position += 11;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_controlData.Write(s, bw);
            bw.WriteInt32(m_handIndex);
            bw.WriteBoolean(m_enable);
            bw.Position += 11;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_controlData = xd.ReadClass<hkbHandIkControlData>(xe, nameof(m_controlData));
            m_handIndex = xd.ReadInt32(xe, nameof(m_handIndex));
            m_enable = xd.ReadBoolean(xe, nameof(m_enable));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbHandIkControlData>(xe, nameof(m_controlData), m_controlData);
            xs.WriteNumber(xe, nameof(m_handIndex), m_handIndex);
            xs.WriteBoolean(xe, nameof(m_enable), m_enable);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandIkControlsModifierHand);
        }

        public bool Equals(hkbHandIkControlsModifierHand? other)
        {
            return other is not null &&
                   ((m_controlData is null && other.m_controlData is null) || (m_controlData is not null && other.m_controlData is not null && m_controlData.Equals((IHavokObject)other.m_controlData))) &&
                   m_handIndex.Equals(other.m_handIndex) &&
                   m_enable.Equals(other.m_enable) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_controlData);
            hashcode.Add(m_handIndex);
            hashcode.Add(m_enable);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

