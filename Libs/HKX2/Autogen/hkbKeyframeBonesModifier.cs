using System.Xml.Linq;
namespace HKX2
{
    // hkbKeyframeBonesModifier Signatire: 0x95f66629 size: 104 flags: FLAGS_NONE

    // m_keyframeInfo m_class: hkbKeyframeBonesModifierKeyframeInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_keyframedBonesList m_class: hkbBoneIndexArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkbKeyframeBonesModifier : hkbModifier, IEquatable<hkbKeyframeBonesModifier?>
    {
        public IList<hkbKeyframeBonesModifierKeyframeInfo> m_keyframeInfo { set; get; } = Array.Empty<hkbKeyframeBonesModifierKeyframeInfo>();
        public hkbBoneIndexArray? m_keyframedBonesList { set; get; }

        public override uint Signature { set; get; } = 0x95f66629;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_keyframeInfo = des.ReadClassArray<hkbKeyframeBonesModifierKeyframeInfo>(br);
            m_keyframedBonesList = des.ReadClassPointer<hkbBoneIndexArray>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_keyframeInfo);
            s.WriteClassPointer(bw, m_keyframedBonesList);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_keyframeInfo = xd.ReadClassArray<hkbKeyframeBonesModifierKeyframeInfo>(xe, nameof(m_keyframeInfo));
            m_keyframedBonesList = xd.ReadClassPointer<hkbBoneIndexArray>(xe, nameof(m_keyframedBonesList));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_keyframeInfo), m_keyframeInfo);
            xs.WriteClassPointer(xe, nameof(m_keyframedBonesList), m_keyframedBonesList);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbKeyframeBonesModifier);
        }

        public bool Equals(hkbKeyframeBonesModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_keyframeInfo.SequenceEqual(other.m_keyframeInfo) &&
                   ((m_keyframedBonesList is null && other.m_keyframedBonesList is null) || (m_keyframedBonesList is not null && other.m_keyframedBonesList is not null && m_keyframedBonesList.Equals((IHavokObject)other.m_keyframedBonesList))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_keyframeInfo.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_keyframedBonesList);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

