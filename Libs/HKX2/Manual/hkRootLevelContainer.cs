using System.Xml.Linq;
namespace HKX2
{
    // hkRootLevelContainer Signatire: 0x2772c11e size: 16 flags: FLAGS_NONE

    // m_namedVariants m_class: hkRootLevelContainerNamedVariant Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 

    public partial class hkRootLevelContainer : IHavokObject, IEquatable<hkRootLevelContainer?>
    {

        public IList<hkRootLevelContainerNamedVariant> m_namedVariants { get; set; } = Array.Empty<hkRootLevelContainerNamedVariant>();

        public uint Signature { set; get; } = 0x2772c11e;

        public void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_namedVariants = des.ReadClassArray<hkRootLevelContainerNamedVariant>(br);
        }

        public void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassArray(bw, m_namedVariants);
        }

        public void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_namedVariants = xd.ReadClassArray<hkRootLevelContainerNamedVariant>(xe, nameof(m_namedVariants));
        }

        public void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassArray(xe, nameof(m_namedVariants), m_namedVariants);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkRootLevelContainer);
        }

        public bool Equals(hkRootLevelContainer? other)
        {
            return other is not null &&
                   m_namedVariants.SequenceEqual(other.m_namedVariants) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_namedVariants.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

