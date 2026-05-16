using System.Xml.Linq;
namespace HKX2
{
    // hkbGetHandleOnBoneModifier Signatire: 0x50c34a17 size: 104 flags: FLAGS_NONE

    // m_handleOut m_class: hkbHandle Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_localFrameName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_ragdollBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_animationBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 98 flags: FLAGS_NONE enum: 
    public partial class hkbGetHandleOnBoneModifier : hkbModifier, IEquatable<hkbGetHandleOnBoneModifier?>
    {
        public hkbHandle? m_handleOut { set; get; }
        public string m_localFrameName { set; get; } = "";
        public short m_ragdollBoneIndex { set; get; }
        public short m_animationBoneIndex { set; get; }

        public override uint Signature { set; get; } = 0x50c34a17;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_handleOut = des.ReadClassPointer<hkbHandle>(br);
            m_localFrameName = des.ReadStringPointer(br);
            m_ragdollBoneIndex = br.ReadInt16();
            m_animationBoneIndex = br.ReadInt16();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_handleOut);
            s.WriteStringPointer(bw, m_localFrameName);
            bw.WriteInt16(m_ragdollBoneIndex);
            bw.WriteInt16(m_animationBoneIndex);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_handleOut = xd.ReadClassPointer<hkbHandle>(xe, nameof(m_handleOut));
            m_localFrameName = xd.ReadString(xe, nameof(m_localFrameName));
            m_ragdollBoneIndex = xd.ReadInt16(xe, nameof(m_ragdollBoneIndex));
            m_animationBoneIndex = xd.ReadInt16(xe, nameof(m_animationBoneIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_handleOut), m_handleOut);
            xs.WriteString(xe, nameof(m_localFrameName), m_localFrameName);
            xs.WriteNumber(xe, nameof(m_ragdollBoneIndex), m_ragdollBoneIndex);
            xs.WriteNumber(xe, nameof(m_animationBoneIndex), m_animationBoneIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbGetHandleOnBoneModifier);
        }

        public bool Equals(hkbGetHandleOnBoneModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_handleOut is null && other.m_handleOut is null) || (m_handleOut is not null && other.m_handleOut is not null && m_handleOut.Equals((IHavokObject)other.m_handleOut))) &&
                   (m_localFrameName is null && other.m_localFrameName is null || m_localFrameName == other.m_localFrameName || m_localFrameName is null && other.m_localFrameName == "" || m_localFrameName == "" && other.m_localFrameName is null) &&
                   m_ragdollBoneIndex.Equals(other.m_ragdollBoneIndex) &&
                   m_animationBoneIndex.Equals(other.m_animationBoneIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_handleOut);
            hashcode.Add(m_localFrameName);
            hashcode.Add(m_ragdollBoneIndex);
            hashcode.Add(m_animationBoneIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

