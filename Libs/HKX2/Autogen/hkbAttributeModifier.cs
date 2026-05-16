using System.Xml.Linq;
namespace HKX2
{
    // hkbAttributeModifier Signatire: 0x1245d97d size: 96 flags: FLAGS_NONE

    // m_assignments m_class: hkbAttributeModifierAssignment Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbAttributeModifier : hkbModifier, IEquatable<hkbAttributeModifier?>
    {
        public IList<hkbAttributeModifierAssignment> m_assignments { set; get; } = Array.Empty<hkbAttributeModifierAssignment>();

        public override uint Signature { set; get; } = 0x1245d97d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_assignments = des.ReadClassArray<hkbAttributeModifierAssignment>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_assignments);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_assignments = xd.ReadClassArray<hkbAttributeModifierAssignment>(xe, nameof(m_assignments));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_assignments), m_assignments);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbAttributeModifier);
        }

        public bool Equals(hkbAttributeModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_assignments.SequenceEqual(other.m_assignments) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_assignments.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

