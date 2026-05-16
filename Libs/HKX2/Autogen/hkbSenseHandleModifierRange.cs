using System.Xml.Linq;
namespace HKX2
{
    // hkbSenseHandleModifierRange Signatire: 0xfb56b692 size: 32 flags: FLAGS_NONE

    // m_event m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_minDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_maxDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_ignoreHandle m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    public partial class hkbSenseHandleModifierRange : IHavokObject, IEquatable<hkbSenseHandleModifierRange?>
    {
        public hkbEventProperty m_event { set; get; } = new();
        public float m_minDistance { set; get; }
        public float m_maxDistance { set; get; }
        public bool m_ignoreHandle { set; get; }

        public virtual uint Signature { set; get; } = 0xfb56b692;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_event.Read(des, br);
            m_minDistance = br.ReadSingle();
            m_maxDistance = br.ReadSingle();
            m_ignoreHandle = br.ReadBoolean();
            br.Position += 7;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_event.Write(s, bw);
            bw.WriteSingle(m_minDistance);
            bw.WriteSingle(m_maxDistance);
            bw.WriteBoolean(m_ignoreHandle);
            bw.Position += 7;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_event = xd.ReadClass<hkbEventProperty>(xe, nameof(m_event));
            m_minDistance = xd.ReadSingle(xe, nameof(m_minDistance));
            m_maxDistance = xd.ReadSingle(xe, nameof(m_maxDistance));
            m_ignoreHandle = xd.ReadBoolean(xe, nameof(m_ignoreHandle));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_event), m_event);
            xs.WriteFloat(xe, nameof(m_minDistance), m_minDistance);
            xs.WriteFloat(xe, nameof(m_maxDistance), m_maxDistance);
            xs.WriteBoolean(xe, nameof(m_ignoreHandle), m_ignoreHandle);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSenseHandleModifierRange);
        }

        public bool Equals(hkbSenseHandleModifierRange? other)
        {
            return other is not null &&
                   ((m_event is null && other.m_event is null) || (m_event is not null && other.m_event is not null && m_event.Equals((IHavokObject)other.m_event))) &&
                   m_minDistance.Equals(other.m_minDistance) &&
                   m_maxDistance.Equals(other.m_maxDistance) &&
                   m_ignoreHandle.Equals(other.m_ignoreHandle) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_event);
            hashcode.Add(m_minDistance);
            hashcode.Add(m_maxDistance);
            hashcode.Add(m_ignoreHandle);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

