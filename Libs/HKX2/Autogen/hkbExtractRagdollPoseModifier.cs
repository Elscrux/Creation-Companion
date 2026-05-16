using System.Xml.Linq;
namespace HKX2
{
    // hkbExtractRagdollPoseModifier Signatire: 0x804dcbab size: 88 flags: FLAGS_NONE

    // m_poseMatchingBone0 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_poseMatchingBone1 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 82 flags: FLAGS_NONE enum: 
    // m_poseMatchingBone2 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_enableComputeWorldFromModel m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 86 flags: FLAGS_NONE enum: 
    public partial class hkbExtractRagdollPoseModifier : hkbModifier, IEquatable<hkbExtractRagdollPoseModifier?>
    {
        public short m_poseMatchingBone0 { set; get; }
        public short m_poseMatchingBone1 { set; get; }
        public short m_poseMatchingBone2 { set; get; }
        public bool m_enableComputeWorldFromModel { set; get; }

        public override uint Signature { set; get; } = 0x804dcbab;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_poseMatchingBone0 = br.ReadInt16();
            m_poseMatchingBone1 = br.ReadInt16();
            m_poseMatchingBone2 = br.ReadInt16();
            m_enableComputeWorldFromModel = br.ReadBoolean();
            br.Position += 1;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt16(m_poseMatchingBone0);
            bw.WriteInt16(m_poseMatchingBone1);
            bw.WriteInt16(m_poseMatchingBone2);
            bw.WriteBoolean(m_enableComputeWorldFromModel);
            bw.Position += 1;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_poseMatchingBone0 = xd.ReadInt16(xe, nameof(m_poseMatchingBone0));
            m_poseMatchingBone1 = xd.ReadInt16(xe, nameof(m_poseMatchingBone1));
            m_poseMatchingBone2 = xd.ReadInt16(xe, nameof(m_poseMatchingBone2));
            m_enableComputeWorldFromModel = xd.ReadBoolean(xe, nameof(m_enableComputeWorldFromModel));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_poseMatchingBone0), m_poseMatchingBone0);
            xs.WriteNumber(xe, nameof(m_poseMatchingBone1), m_poseMatchingBone1);
            xs.WriteNumber(xe, nameof(m_poseMatchingBone2), m_poseMatchingBone2);
            xs.WriteBoolean(xe, nameof(m_enableComputeWorldFromModel), m_enableComputeWorldFromModel);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbExtractRagdollPoseModifier);
        }

        public bool Equals(hkbExtractRagdollPoseModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_poseMatchingBone0.Equals(other.m_poseMatchingBone0) &&
                   m_poseMatchingBone1.Equals(other.m_poseMatchingBone1) &&
                   m_poseMatchingBone2.Equals(other.m_poseMatchingBone2) &&
                   m_enableComputeWorldFromModel.Equals(other.m_enableComputeWorldFromModel) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_poseMatchingBone0);
            hashcode.Add(m_poseMatchingBone1);
            hashcode.Add(m_poseMatchingBone2);
            hashcode.Add(m_enableComputeWorldFromModel);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

