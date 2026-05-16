using System.Xml.Linq;
namespace HKX2
{
    // hkxAttributeHolder Signatire: 0x7468cc44 size: 32 flags: FLAGS_NONE

    // m_attributeGroups m_class: hkxAttributeGroup Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxAttributeHolder : hkReferencedObject, IEquatable<hkxAttributeHolder?>
    {
        public IList<hkxAttributeGroup> m_attributeGroups { set; get; } = Array.Empty<hkxAttributeGroup>();

        public override uint Signature { set; get; } = 0x7468cc44;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_attributeGroups = des.ReadClassArray<hkxAttributeGroup>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_attributeGroups);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_attributeGroups = xd.ReadClassArray<hkxAttributeGroup>(xe, nameof(m_attributeGroups));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_attributeGroups), m_attributeGroups);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAttributeHolder);
        }

        public bool Equals(hkxAttributeHolder? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_attributeGroups.SequenceEqual(other.m_attributeGroups) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_attributeGroups.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

