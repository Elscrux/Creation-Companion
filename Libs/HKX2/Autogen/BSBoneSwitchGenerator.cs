using System.Xml.Linq;
namespace HKX2
{
    // BSBoneSwitchGenerator Signatire: 0xf33d3eea size: 112 flags: FLAGS_NONE

    // m_pDefaultGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_ChildrenA m_class: BSBoneSwitchGeneratorBoneData Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    public partial class BSBoneSwitchGenerator : hkbGenerator, IEquatable<BSBoneSwitchGenerator?>
    {
        public hkbGenerator? m_pDefaultGenerator { set; get; }
        public IList<BSBoneSwitchGeneratorBoneData> m_ChildrenA { set; get; } = Array.Empty<BSBoneSwitchGeneratorBoneData>();

        public override uint Signature { set; get; } = 0xf33d3eea;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_pDefaultGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_ChildrenA = des.ReadClassPointerArray<BSBoneSwitchGeneratorBoneData>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pDefaultGenerator);
            s.WriteClassPointerArray(bw, m_ChildrenA);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pDefaultGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pDefaultGenerator));
            m_ChildrenA = xd.ReadClassPointerArray<BSBoneSwitchGeneratorBoneData>(xe, nameof(m_ChildrenA));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pDefaultGenerator), m_pDefaultGenerator);
            xs.WriteClassPointerArray(xe, nameof(m_ChildrenA), m_ChildrenA);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSBoneSwitchGenerator);
        }

        public bool Equals(BSBoneSwitchGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pDefaultGenerator is null && other.m_pDefaultGenerator is null) || (m_pDefaultGenerator is not null && other.m_pDefaultGenerator is not null && m_pDefaultGenerator.Equals((IHavokObject)other.m_pDefaultGenerator))) &&
                   m_ChildrenA.SequenceEqual(other.m_ChildrenA) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pDefaultGenerator);
            hashcode.Add(m_ChildrenA.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

