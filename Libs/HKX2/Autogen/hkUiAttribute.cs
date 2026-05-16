using System.Xml.Linq;
namespace HKX2
{
    // hkUiAttribute Signatire: 0xeb6e96e3 size: 40 flags: FLAGS_NONE

    // m_visible m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_hideInModeler m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 1 flags: FLAGS_NONE enum: HideInModeler
    // m_label m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_group m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_hideBaseClassMembers m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_endGroup m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_endGroup2 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 33 flags: FLAGS_NONE enum: 
    // m_advanced m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 34 flags: FLAGS_NONE enum: 
    public partial class hkUiAttribute : IHavokObject, IEquatable<hkUiAttribute?>
    {
        public bool m_visible { set; get; }
        public sbyte m_hideInModeler { set; get; }
        public string m_label { set; get; } = "";
        public string m_group { set; get; } = "";
        public string m_hideBaseClassMembers { set; get; } = "";
        public bool m_endGroup { set; get; }
        public bool m_endGroup2 { set; get; }
        public bool m_advanced { set; get; }

        public virtual uint Signature { set; get; } = 0xeb6e96e3;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_visible = br.ReadBoolean();
            m_hideInModeler = br.ReadSByte();
            br.Position += 6;
            m_label = des.ReadCString(br);
            m_group = des.ReadCString(br);
            m_hideBaseClassMembers = des.ReadCString(br);
            m_endGroup = br.ReadBoolean();
            m_endGroup2 = br.ReadBoolean();
            m_advanced = br.ReadBoolean();
            br.Position += 5;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_visible);
            bw.WriteSByte(m_hideInModeler);
            bw.Position += 6;
            s.WriteCString(bw, m_label);
            s.WriteCString(bw, m_group);
            s.WriteCString(bw, m_hideBaseClassMembers);
            bw.WriteBoolean(m_endGroup);
            bw.WriteBoolean(m_endGroup2);
            bw.WriteBoolean(m_advanced);
            bw.Position += 5;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_visible = xd.ReadBoolean(xe, nameof(m_visible));
            m_hideInModeler = xd.ReadFlag<HideInModeler, sbyte>(xe, nameof(m_hideInModeler));
            m_label = xd.ReadString(xe, nameof(m_label));
            m_group = xd.ReadString(xe, nameof(m_group));
            m_hideBaseClassMembers = xd.ReadString(xe, nameof(m_hideBaseClassMembers));
            m_endGroup = xd.ReadBoolean(xe, nameof(m_endGroup));
            m_endGroup2 = xd.ReadBoolean(xe, nameof(m_endGroup2));
            m_advanced = xd.ReadBoolean(xe, nameof(m_advanced));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteBoolean(xe, nameof(m_visible), m_visible);
            xs.WriteEnum<HideInModeler, sbyte>(xe, nameof(m_hideInModeler), m_hideInModeler);
            xs.WriteString(xe, nameof(m_label), m_label);
            xs.WriteString(xe, nameof(m_group), m_group);
            xs.WriteString(xe, nameof(m_hideBaseClassMembers), m_hideBaseClassMembers);
            xs.WriteBoolean(xe, nameof(m_endGroup), m_endGroup);
            xs.WriteBoolean(xe, nameof(m_endGroup2), m_endGroup2);
            xs.WriteBoolean(xe, nameof(m_advanced), m_advanced);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkUiAttribute);
        }

        public bool Equals(hkUiAttribute? other)
        {
            return other is not null &&
                   m_visible.Equals(other.m_visible) &&
                   m_hideInModeler.Equals(other.m_hideInModeler) &&
                   (m_label is null && other.m_label is null || m_label == other.m_label || m_label is null && other.m_label == "" || m_label == "" && other.m_label is null) &&
                   (m_group is null && other.m_group is null || m_group == other.m_group || m_group is null && other.m_group == "" || m_group == "" && other.m_group is null) &&
                   (m_hideBaseClassMembers is null && other.m_hideBaseClassMembers is null || m_hideBaseClassMembers == other.m_hideBaseClassMembers || m_hideBaseClassMembers is null && other.m_hideBaseClassMembers == "" || m_hideBaseClassMembers == "" && other.m_hideBaseClassMembers is null) &&
                   m_endGroup.Equals(other.m_endGroup) &&
                   m_endGroup2.Equals(other.m_endGroup2) &&
                   m_advanced.Equals(other.m_advanced) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_visible);
            hashcode.Add(m_hideInModeler);
            hashcode.Add(m_label);
            hashcode.Add(m_group);
            hashcode.Add(m_hideBaseClassMembers);
            hashcode.Add(m_endGroup);
            hashcode.Add(m_endGroup2);
            hashcode.Add(m_advanced);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

