using System.Xml.Linq;
namespace HKX2
{
    // hkpPhysicsSystemWithContacts Signatire: 0xd0fd4bbe size: 120 flags: FLAGS_NONE

    // m_contacts m_class: hkpSerializedAgentNnEntry Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    public partial class hkpPhysicsSystemWithContacts : hkpPhysicsSystem, IEquatable<hkpPhysicsSystemWithContacts?>
    {
        public IList<hkpSerializedAgentNnEntry> m_contacts { set; get; } = Array.Empty<hkpSerializedAgentNnEntry>();

        public override uint Signature { set; get; } = 0xd0fd4bbe;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_contacts = des.ReadClassPointerArray<hkpSerializedAgentNnEntry>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_contacts);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_contacts = xd.ReadClassPointerArray<hkpSerializedAgentNnEntry>(xe, nameof(m_contacts));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_contacts), m_contacts);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPhysicsSystemWithContacts);
        }

        public bool Equals(hkpPhysicsSystemWithContacts? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_contacts.SequenceEqual(other.m_contacts) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_contacts.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

