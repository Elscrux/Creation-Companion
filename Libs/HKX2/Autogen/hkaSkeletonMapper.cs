using System.Xml.Linq;
namespace HKX2
{
    // hkaSkeletonMapper Signatire: 0x12df42a5 size: 144 flags: FLAGS_NONE

    // m_mapping m_class: hkaSkeletonMapperData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkaSkeletonMapper : hkReferencedObject, IEquatable<hkaSkeletonMapper?>
    {
        public hkaSkeletonMapperData m_mapping { set; get; } = new();

        public override uint Signature { set; get; } = 0x12df42a5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_mapping.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_mapping.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_mapping = xd.ReadClass<hkaSkeletonMapperData>(xe, nameof(m_mapping));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkaSkeletonMapperData>(xe, nameof(m_mapping), m_mapping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSkeletonMapper);
        }

        public bool Equals(hkaSkeletonMapper? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_mapping is null && other.m_mapping is null) || (m_mapping is not null && other.m_mapping is not null && m_mapping.Equals((IHavokObject)other.m_mapping))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_mapping);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

