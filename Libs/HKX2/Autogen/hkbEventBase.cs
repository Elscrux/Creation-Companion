using System.Xml.Linq;
namespace HKX2
{
    // hkbEventBase Signatire: 0x76bddb31 size: 16 flags: FLAGS_NONE

    // m_id m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_payload m_class: hkbEventPayload Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkbEventBase : IHavokObject, IEquatable<hkbEventBase?>
    {
        public int m_id { set; get; }
        public hkbEventPayload? m_payload { set; get; }

        public virtual uint Signature { set; get; } = 0x76bddb31;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadInt32();
            br.Position += 4;
            m_payload = des.ReadClassPointer<hkbEventPayload>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_id);
            bw.Position += 4;
            s.WriteClassPointer(bw, m_payload);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_id = xd.ReadInt32(xe, nameof(m_id));
            m_payload = xd.ReadClassPointer<hkbEventPayload>(xe, nameof(m_payload));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_id), m_id);
            xs.WriteClassPointer(xe, nameof(m_payload), m_payload);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventBase);
        }

        public bool Equals(hkbEventBase? other)
        {
            return other is not null &&
                   m_id.Equals(other.m_id) &&
                   ((m_payload is null && other.m_payload is null) || (m_payload is not null && other.m_payload is not null && m_payload.Equals((IHavokObject)other.m_payload))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_id);
            hashcode.Add(m_payload);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

