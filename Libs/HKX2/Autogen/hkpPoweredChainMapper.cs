using System.Xml.Linq;
namespace HKX2
{
    // hkpPoweredChainMapper Signatire: 0x7a77ef5 size: 64 flags: FLAGS_NONE

    // m_links m_class: hkpPoweredChainMapperLinkInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_targets m_class: hkpPoweredChainMapperTarget Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_chains m_class: hkpConstraintChainInstance Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpPoweredChainMapper : hkReferencedObject, IEquatable<hkpPoweredChainMapper?>
    {
        public IList<hkpPoweredChainMapperLinkInfo> m_links { set; get; } = Array.Empty<hkpPoweredChainMapperLinkInfo>();
        public IList<hkpPoweredChainMapperTarget> m_targets { set; get; } = Array.Empty<hkpPoweredChainMapperTarget>();
        public IList<hkpConstraintChainInstance> m_chains { set; get; } = Array.Empty<hkpConstraintChainInstance>();

        public override uint Signature { set; get; } = 0x7a77ef5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_links = des.ReadClassArray<hkpPoweredChainMapperLinkInfo>(br);
            m_targets = des.ReadClassArray<hkpPoweredChainMapperTarget>(br);
            m_chains = des.ReadClassPointerArray<hkpConstraintChainInstance>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_links);
            s.WriteClassArray(bw, m_targets);
            s.WriteClassPointerArray(bw, m_chains);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_links = xd.ReadClassArray<hkpPoweredChainMapperLinkInfo>(xe, nameof(m_links));
            m_targets = xd.ReadClassArray<hkpPoweredChainMapperTarget>(xe, nameof(m_targets));
            m_chains = xd.ReadClassPointerArray<hkpConstraintChainInstance>(xe, nameof(m_chains));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_links), m_links);
            xs.WriteClassArray(xe, nameof(m_targets), m_targets);
            xs.WriteClassPointerArray(xe, nameof(m_chains), m_chains);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPoweredChainMapper);
        }

        public bool Equals(hkpPoweredChainMapper? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_links.SequenceEqual(other.m_links) &&
                   m_targets.SequenceEqual(other.m_targets) &&
                   m_chains.SequenceEqual(other.m_chains) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_links.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_targets.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_chains.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

