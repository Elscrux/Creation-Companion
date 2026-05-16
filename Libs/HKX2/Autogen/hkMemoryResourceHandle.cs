using System.Xml.Linq;
namespace HKX2
{
    // hkMemoryResourceHandle Signatire: 0xbffac086 size: 48 flags: FLAGS_NONE

    // m_variant m_class: hkReferencedObject Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_references m_class: hkMemoryResourceHandleExternalLink Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkMemoryResourceHandle : hkResourceHandle, IEquatable<hkMemoryResourceHandle?>
    {
        public hkReferencedObject? m_variant { set; get; }
        public string m_name { set; get; } = "";
        public IList<hkMemoryResourceHandleExternalLink> m_references { set; get; } = Array.Empty<hkMemoryResourceHandleExternalLink>();

        public override uint Signature { set; get; } = 0xbffac086;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_variant = des.ReadClassPointer<hkReferencedObject>(br);
            m_name = des.ReadStringPointer(br);
            m_references = des.ReadClassArray<hkMemoryResourceHandleExternalLink>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_variant);
            s.WriteStringPointer(bw, m_name);
            s.WriteClassArray(bw, m_references);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_variant = xd.ReadClassPointer<hkReferencedObject>(xe, nameof(m_variant));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_references = xd.ReadClassArray<hkMemoryResourceHandleExternalLink>(xe, nameof(m_references));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_variant), m_variant);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteClassArray(xe, nameof(m_references), m_references);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMemoryResourceHandle);
        }

        public bool Equals(hkMemoryResourceHandle? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_variant is null && other.m_variant is null) || (m_variant is not null && other.m_variant is not null && m_variant.Equals((IHavokObject)other.m_variant))) &&
                   m_name == other.m_name &&
                   m_references.SequenceEqual(other.m_references) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_variant);
            hashcode.Add(m_name);
            hashcode.Add(m_references.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

