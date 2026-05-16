using System.Xml.Linq;
namespace HKX2
{
    // hkbRigidBodyRagdollControlData Signatire: 0x1e0bc068 size: 64 flags: FLAGS_NONE

    // m_keyFrameHierarchyControlData m_class: hkaKeyFrameHierarchyUtilityControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_durationToBlend m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkbRigidBodyRagdollControlData : IHavokObject, IEquatable<hkbRigidBodyRagdollControlData?>
    {
        public hkaKeyFrameHierarchyUtilityControlData m_keyFrameHierarchyControlData { set; get; } = new();
        public float m_durationToBlend { set; get; }

        public virtual uint Signature { set; get; } = 0x1e0bc068;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_keyFrameHierarchyControlData.Read(des, br);
            m_durationToBlend = br.ReadSingle();
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_keyFrameHierarchyControlData.Write(s, bw);
            bw.WriteSingle(m_durationToBlend);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_keyFrameHierarchyControlData = xd.ReadClass<hkaKeyFrameHierarchyUtilityControlData>(xe, nameof(m_keyFrameHierarchyControlData));
            m_durationToBlend = xd.ReadSingle(xe, nameof(m_durationToBlend));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkaKeyFrameHierarchyUtilityControlData>(xe, nameof(m_keyFrameHierarchyControlData), m_keyFrameHierarchyControlData);
            xs.WriteFloat(xe, nameof(m_durationToBlend), m_durationToBlend);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbRigidBodyRagdollControlData);
        }

        public bool Equals(hkbRigidBodyRagdollControlData? other)
        {
            return other is not null &&
                   ((m_keyFrameHierarchyControlData is null && other.m_keyFrameHierarchyControlData is null) || (m_keyFrameHierarchyControlData is not null && other.m_keyFrameHierarchyControlData is not null && m_keyFrameHierarchyControlData.Equals((IHavokObject)other.m_keyFrameHierarchyControlData))) &&
                   m_durationToBlend.Equals(other.m_durationToBlend) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_keyFrameHierarchyControlData);
            hashcode.Add(m_durationToBlend);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

