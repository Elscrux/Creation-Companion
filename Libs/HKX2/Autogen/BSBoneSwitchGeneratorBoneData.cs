using System.Xml.Linq;
namespace HKX2
{
    // BSBoneSwitchGeneratorBoneData Signatire: 0xc1215be6 size: 64 flags: FLAGS_NONE

    // m_pGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_spBoneWeight m_class: hkbBoneWeightArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    public partial class BSBoneSwitchGeneratorBoneData : hkbBindable, IEquatable<BSBoneSwitchGeneratorBoneData?>
    {
        public hkbGenerator? m_pGenerator { set; get; }
        public hkbBoneWeightArray? m_spBoneWeight { set; get; }

        public override uint Signature { set; get; } = 0xc1215be6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_spBoneWeight = des.ReadClassPointer<hkbBoneWeightArray>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_pGenerator);
            s.WriteClassPointer(bw, m_spBoneWeight);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pGenerator));
            m_spBoneWeight = xd.ReadClassPointer<hkbBoneWeightArray>(xe, nameof(m_spBoneWeight));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pGenerator), m_pGenerator);
            xs.WriteClassPointer(xe, nameof(m_spBoneWeight), m_spBoneWeight);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSBoneSwitchGeneratorBoneData);
        }

        public bool Equals(BSBoneSwitchGeneratorBoneData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pGenerator is null && other.m_pGenerator is null) || (m_pGenerator is not null && other.m_pGenerator is not null && m_pGenerator.Equals((IHavokObject)other.m_pGenerator))) &&
                   ((m_spBoneWeight is null && other.m_spBoneWeight is null) || (m_spBoneWeight is not null && other.m_spBoneWeight is not null && m_spBoneWeight.Equals((IHavokObject)other.m_spBoneWeight))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pGenerator);
            hashcode.Add(m_spBoneWeight);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

