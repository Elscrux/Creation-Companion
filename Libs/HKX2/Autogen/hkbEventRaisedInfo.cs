using System.Xml.Linq;
namespace HKX2
{
    // hkbEventRaisedInfo Signatire: 0xc02da3 size: 48 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_eventName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_raisedBySdk m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_senderId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_padding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkbEventRaisedInfo : hkReferencedObject, IEquatable<hkbEventRaisedInfo?>
    {
        public ulong m_characterId { set; get; }
        public string m_eventName { set; get; } = "";
        public bool m_raisedBySdk { set; get; }
        public int m_senderId { set; get; }
        public int m_padding { set; get; }

        public override uint Signature { set; get; } = 0xc02da3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_eventName = des.ReadStringPointer(br);
            m_raisedBySdk = br.ReadBoolean();
            br.Position += 3;
            m_senderId = br.ReadInt32();
            m_padding = br.ReadInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            s.WriteStringPointer(bw, m_eventName);
            bw.WriteBoolean(m_raisedBySdk);
            bw.Position += 3;
            bw.WriteInt32(m_senderId);
            bw.WriteInt32(m_padding);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_eventName = xd.ReadString(xe, nameof(m_eventName));
            m_raisedBySdk = xd.ReadBoolean(xe, nameof(m_raisedBySdk));
            m_senderId = xd.ReadInt32(xe, nameof(m_senderId));
            m_padding = xd.ReadInt32(xe, nameof(m_padding));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteString(xe, nameof(m_eventName), m_eventName);
            xs.WriteBoolean(xe, nameof(m_raisedBySdk), m_raisedBySdk);
            xs.WriteNumber(xe, nameof(m_senderId), m_senderId);
            xs.WriteNumber(xe, nameof(m_padding), m_padding);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventRaisedInfo);
        }

        public bool Equals(hkbEventRaisedInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   (m_eventName is null && other.m_eventName is null || m_eventName == other.m_eventName || m_eventName is null && other.m_eventName == "" || m_eventName == "" && other.m_eventName is null) &&
                   m_raisedBySdk.Equals(other.m_raisedBySdk) &&
                   m_senderId.Equals(other.m_senderId) &&
                   m_padding.Equals(other.m_padding) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_eventName);
            hashcode.Add(m_raisedBySdk);
            hashcode.Add(m_senderId);
            hashcode.Add(m_padding);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

