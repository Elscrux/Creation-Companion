using System.Xml.Linq;
namespace HKX2
{
    // hkbRigidBodyRagdollControlsModifier Signatire: 0xaa87d1eb size: 160 flags: FLAGS_NONE

    // m_controlData m_class: hkbRigidBodyRagdollControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_bones m_class: hkbBoneIndexArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    public partial class hkbRigidBodyRagdollControlsModifier : hkbModifier, IEquatable<hkbRigidBodyRagdollControlsModifier?>
    {
        public hkbRigidBodyRagdollControlData m_controlData { set; get; } = new();
        public hkbBoneIndexArray? m_bones { set; get; }

        public override uint Signature { set; get; } = 0xaa87d1eb;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controlData.Read(des, br);
            m_bones = des.ReadClassPointer<hkbBoneIndexArray>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_controlData.Write(s, bw);
            s.WriteClassPointer(bw, m_bones);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_controlData = xd.ReadClass<hkbRigidBodyRagdollControlData>(xe, nameof(m_controlData));
            m_bones = xd.ReadClassPointer<hkbBoneIndexArray>(xe, nameof(m_bones));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbRigidBodyRagdollControlData>(xe, nameof(m_controlData), m_controlData);
            xs.WriteClassPointer(xe, nameof(m_bones), m_bones);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbRigidBodyRagdollControlsModifier);
        }

        public bool Equals(hkbRigidBodyRagdollControlsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_controlData is null && other.m_controlData is null) || (m_controlData is not null && other.m_controlData is not null && m_controlData.Equals((IHavokObject)other.m_controlData))) &&
                   ((m_bones is null && other.m_bones is null) || (m_bones is not null && other.m_bones is not null && m_bones.Equals((IHavokObject)other.m_bones))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_controlData);
            hashcode.Add(m_bones);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

