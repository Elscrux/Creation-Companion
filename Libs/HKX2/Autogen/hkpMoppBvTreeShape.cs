using System.Xml.Linq;
namespace HKX2
{
    // hkpMoppBvTreeShape Signatire: 0x90b29d39 size: 112 flags: FLAGS_NONE

    // m_child m_class: hkpSingleShapeContainer Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_childSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpMoppBvTreeShape : hkMoppBvTreeShapeBase, IEquatable<hkpMoppBvTreeShape?>
    {
        public hkpSingleShapeContainer m_child { set; get; } = new();
        private int m_childSize { set; get; }

        public override uint Signature { set; get; } = 0x90b29d39;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_child.Read(des, br);
            m_childSize = br.ReadInt32();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_child.Write(s, bw);
            bw.WriteInt32(m_childSize);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_child = xd.ReadClass<hkpSingleShapeContainer>(xe, nameof(m_child));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpSingleShapeContainer>(xe, nameof(m_child), m_child);
            xs.WriteSerializeIgnored(xe, nameof(m_childSize));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMoppBvTreeShape);
        }

        public bool Equals(hkpMoppBvTreeShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_child is null && other.m_child is null) || (m_child is not null && other.m_child is not null && m_child.Equals((IHavokObject)other.m_child))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_child);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

