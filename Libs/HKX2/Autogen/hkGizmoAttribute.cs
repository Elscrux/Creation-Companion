using System.Xml.Linq;
namespace HKX2
{
    // hkGizmoAttribute Signatire: 0x23aadfb6 size: 24 flags: FLAGS_NONE

    // m_visible m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_label m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: GizmoType
    public partial class hkGizmoAttribute : IHavokObject, IEquatable<hkGizmoAttribute?>
    {
        public bool m_visible { set; get; }
        public string m_label { set; get; } = "";
        public sbyte m_type { set; get; }

        public virtual uint Signature { set; get; } = 0x23aadfb6;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_visible = br.ReadBoolean();
            br.Position += 7;
            m_label = des.ReadCString(br);
            m_type = br.ReadSByte();
            br.Position += 7;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_visible);
            bw.Position += 7;
            s.WriteCString(bw, m_label);
            bw.WriteSByte(m_type);
            bw.Position += 7;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_visible = xd.ReadBoolean(xe, nameof(m_visible));
            m_label = xd.ReadString(xe, nameof(m_label));
            m_type = xd.ReadFlag<GizmoType, sbyte>(xe, nameof(m_type));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteBoolean(xe, nameof(m_visible), m_visible);
            xs.WriteString(xe, nameof(m_label), m_label);
            xs.WriteEnum<GizmoType, sbyte>(xe, nameof(m_type), m_type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkGizmoAttribute);
        }

        public bool Equals(hkGizmoAttribute? other)
        {
            return other is not null &&
                   m_visible.Equals(other.m_visible) &&
                   (m_label is null && other.m_label is null || m_label == other.m_label || m_label is null && other.m_label == "" || m_label == "" && other.m_label is null) &&
                   m_type.Equals(other.m_type) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_visible);
            hashcode.Add(m_label);
            hashcode.Add(m_type);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

