using System.Xml.Linq;
namespace HKX2
{
    // hkbPoweredRagdollControlsModifier Signatire: 0x7cb54065 size: 144 flags: FLAGS_NONE

    // m_controlData m_class: hkbPoweredRagdollControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_bones m_class: hkbBoneIndexArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_worldFromModelModeData m_class: hkbWorldFromModelModeData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_boneWeights m_class: hkbBoneWeightArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    public partial class hkbPoweredRagdollControlsModifier : hkbModifier, IEquatable<hkbPoweredRagdollControlsModifier?>
    {
        public hkbPoweredRagdollControlData m_controlData { set; get; } = new();
        public hkbBoneIndexArray? m_bones { set; get; }
        public hkbWorldFromModelModeData m_worldFromModelModeData { set; get; } = new();
        public hkbBoneWeightArray? m_boneWeights { set; get; }

        public override uint Signature { set; get; } = 0x7cb54065;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controlData.Read(des, br);
            m_bones = des.ReadClassPointer<hkbBoneIndexArray>(br);
            m_worldFromModelModeData.Read(des, br);
            m_boneWeights = des.ReadClassPointer<hkbBoneWeightArray>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_controlData.Write(s, bw);
            s.WriteClassPointer(bw, m_bones);
            m_worldFromModelModeData.Write(s, bw);
            s.WriteClassPointer(bw, m_boneWeights);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_controlData = xd.ReadClass<hkbPoweredRagdollControlData>(xe, nameof(m_controlData));
            m_bones = xd.ReadClassPointer<hkbBoneIndexArray>(xe, nameof(m_bones));
            m_worldFromModelModeData = xd.ReadClass<hkbWorldFromModelModeData>(xe, nameof(m_worldFromModelModeData));
            m_boneWeights = xd.ReadClassPointer<hkbBoneWeightArray>(xe, nameof(m_boneWeights));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbPoweredRagdollControlData>(xe, nameof(m_controlData), m_controlData);
            xs.WriteClassPointer(xe, nameof(m_bones), m_bones);
            xs.WriteClass(xe, nameof(m_worldFromModelModeData), m_worldFromModelModeData);
            xs.WriteClassPointer(xe, nameof(m_boneWeights), m_boneWeights);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbPoweredRagdollControlsModifier);
        }

        public bool Equals(hkbPoweredRagdollControlsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_controlData is null && other.m_controlData is null) || (m_controlData is not null && other.m_controlData is not null && m_controlData.Equals((IHavokObject)other.m_controlData))) &&
                   ((m_bones is null && other.m_bones is null) || (m_bones is not null && other.m_bones is not null && m_bones.Equals((IHavokObject)other.m_bones))) &&
                   ((m_worldFromModelModeData is null && other.m_worldFromModelModeData is null) || (m_worldFromModelModeData is not null && other.m_worldFromModelModeData is not null && m_worldFromModelModeData.Equals((IHavokObject)other.m_worldFromModelModeData))) &&
                   ((m_boneWeights is null && other.m_boneWeights is null) || (m_boneWeights is not null && other.m_boneWeights is not null && m_boneWeights.Equals((IHavokObject)other.m_boneWeights))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_controlData);
            hashcode.Add(m_bones);
            hashcode.Add(m_worldFromModelModeData);
            hashcode.Add(m_boneWeights);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

