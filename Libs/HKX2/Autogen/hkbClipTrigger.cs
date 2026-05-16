using System.Xml.Linq;
namespace HKX2
{
    // hkbClipTrigger Signatire: 0x7eb45cea size: 32 flags: FLAGS_NONE

    // m_localTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_event m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_relativeToEndOfClip m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_acyclic m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 25 flags: FLAGS_NONE enum: 
    // m_isAnnotation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 26 flags: FLAGS_NONE enum: 
    public partial class hkbClipTrigger : IHavokObject, IEquatable<hkbClipTrigger?>
    {
        public float m_localTime { set; get; }
        public hkbEventProperty m_event { set; get; } = new();
        public bool m_relativeToEndOfClip { set; get; }
        public bool m_acyclic { set; get; }
        public bool m_isAnnotation { set; get; }

        public virtual uint Signature { set; get; } = 0x7eb45cea;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_localTime = br.ReadSingle();
            br.Position += 4;
            m_event.Read(des, br);
            m_relativeToEndOfClip = br.ReadBoolean();
            m_acyclic = br.ReadBoolean();
            m_isAnnotation = br.ReadBoolean();
            br.Position += 5;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_localTime);
            bw.Position += 4;
            m_event.Write(s, bw);
            bw.WriteBoolean(m_relativeToEndOfClip);
            bw.WriteBoolean(m_acyclic);
            bw.WriteBoolean(m_isAnnotation);
            bw.Position += 5;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_localTime = xd.ReadSingle(xe, nameof(m_localTime));
            m_event = xd.ReadClass<hkbEventProperty>(xe, nameof(m_event));
            m_relativeToEndOfClip = xd.ReadBoolean(xe, nameof(m_relativeToEndOfClip));
            m_acyclic = xd.ReadBoolean(xe, nameof(m_acyclic));
            m_isAnnotation = xd.ReadBoolean(xe, nameof(m_isAnnotation));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_localTime), m_localTime);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_event), m_event);
            xs.WriteBoolean(xe, nameof(m_relativeToEndOfClip), m_relativeToEndOfClip);
            xs.WriteBoolean(xe, nameof(m_acyclic), m_acyclic);
            xs.WriteBoolean(xe, nameof(m_isAnnotation), m_isAnnotation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbClipTrigger);
        }

        public bool Equals(hkbClipTrigger? other)
        {
            return other is not null &&
                   m_localTime.Equals(other.m_localTime) &&
                   ((m_event is null && other.m_event is null) || (m_event is not null && other.m_event is not null && m_event.Equals((IHavokObject)other.m_event))) &&
                   m_relativeToEndOfClip.Equals(other.m_relativeToEndOfClip) &&
                   m_acyclic.Equals(other.m_acyclic) &&
                   m_isAnnotation.Equals(other.m_isAnnotation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_localTime);
            hashcode.Add(m_event);
            hashcode.Add(m_relativeToEndOfClip);
            hashcode.Add(m_acyclic);
            hashcode.Add(m_isAnnotation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

