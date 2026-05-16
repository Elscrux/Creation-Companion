using System.Xml.Linq;
namespace HKX2
{
    // hkbModifierList Signatire: 0xa4180ca1 size: 96 flags: FLAGS_NONE

    // m_modifiers m_class: hkbModifier Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbModifierList : hkbModifier, IEquatable<hkbModifierList?>
    {
        public IList<hkbModifier> m_modifiers { set; get; } = Array.Empty<hkbModifier>();

        public override uint Signature { set; get; } = 0xa4180ca1;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modifiers = des.ReadClassPointerArray<hkbModifier>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_modifiers);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_modifiers = xd.ReadClassPointerArray<hkbModifier>(xe, nameof(m_modifiers));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_modifiers), m_modifiers);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbModifierList);
        }

        public bool Equals(hkbModifierList? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_modifiers.SequenceEqual(other.m_modifiers) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_modifiers.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

