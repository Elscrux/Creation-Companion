using System.Xml.Linq;
namespace HKX2
{
    // hkMemoryResourceContainer Signatire: 0x4762f92a size: 64 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_parent m_class: hkMemoryResourceContainer Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_resourceHandles m_class: hkMemoryResourceHandle Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_children m_class: hkMemoryResourceContainer Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkMemoryResourceContainer : hkResourceContainer, IEquatable<hkMemoryResourceContainer?>
    {
        public string m_name { set; get; } = "";
        private hkMemoryResourceContainer? m_parent { set; get; }
        public IList<hkMemoryResourceHandle> m_resourceHandles { set; get; } = Array.Empty<hkMemoryResourceHandle>();
        public IList<hkMemoryResourceContainer> m_children { set; get; } = Array.Empty<hkMemoryResourceContainer>();

        public override uint Signature { set; get; } = 0x4762f92a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_parent = des.ReadClassPointer<hkMemoryResourceContainer>(br);
            m_resourceHandles = des.ReadClassPointerArray<hkMemoryResourceHandle>(br);
            m_children = des.ReadClassPointerArray<hkMemoryResourceContainer>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            s.WriteClassPointer(bw, m_parent);
            s.WriteClassPointerArray(bw, m_resourceHandles);
            s.WriteClassPointerArray(bw, m_children);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_resourceHandles = xd.ReadClassPointerArray<hkMemoryResourceHandle>(xe, nameof(m_resourceHandles));
            m_children = xd.ReadClassPointerArray<hkMemoryResourceContainer>(xe, nameof(m_children));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteSerializeIgnored(xe, nameof(m_parent));
            xs.WriteClassPointerArray(xe, nameof(m_resourceHandles), m_resourceHandles);
            xs.WriteClassPointerArray(xe, nameof(m_children), m_children);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMemoryResourceContainer);
        }

        public bool Equals(hkMemoryResourceContainer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   m_resourceHandles.SequenceEqual(other.m_resourceHandles) &&
                   m_children.SequenceEqual(other.m_children) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_resourceHandles.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_children.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

